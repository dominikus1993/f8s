namespace FSharpNetes

open System
open System.Linq

[<AutoOpen>]
module Container =
    open k8s
    open k8s.Models

    type ImagePullPolicy =
        | Always
        | IfNotPresent
        | Never
        member this.ToKubeValue() =
            match this with
            | Always -> "Always"
            | IfNotPresent -> "IfNotPresent"
            | Never -> "Never"

    type ContainerState =
        { Name: string option
          Image: string option
          ImagePullPolicy: ImagePullPolicy
          Env: Choice<V1EnvVar, V1EnvFromSource> list }

    type ContainerBuilder internal () =
        member this.Yield(_) =
            { Name = None
              Image = None
              ImagePullPolicy = Always
              Env = [] }

        member this.Run(state: ContainerState) =
            let name =
                defaultArg state.Name (Guid.NewGuid().ToString())

            let pp = state.ImagePullPolicy.ToKubeValue()
            let imageName = defaultArg state.Image null

            let envs =
                state.Env
                |> List.filter (fun e ->
                    match e with
                    | Choice1Of2 (_) -> true
                    | _ -> false)
                |> List.map (fun env ->
                    match env with
                    | Choice1Of2 (e) -> e)
                |> List.toSeq

            V1Container(name = name, imagePullPolicy = pp, image = imageName, env = envs.ToList())

        [<CustomOperation("name")>]
        member this.Name(state: ContainerState, name: string) = { state with Name = Some(name) }

        [<CustomOperation("image")>]
        member this.ImageName(state: ContainerState, name: string) = { state with Image = Some(name) }

        [<CustomOperation("image_pull_policy")>]
        member this.ImagePullPolicy(state: ContainerState, policy: ImagePullPolicy) =
            { state with ImagePullPolicy = policy }

        [<CustomOperation("env")>]
        member this.Env(state: ContainerState, env: Choice<V1EnvVar, V1EnvFromSource> list) = { state with Env = env }

    let container = ContainerBuilder()
