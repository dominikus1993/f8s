namespace FSharpNetes

open System
open System.Linq
open FSharpNetes.Utils

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
          Image: Image option
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
            let image = match state.Image with | Some(i) -> i | _ -> failwith "No image provided"
            let imageName = image |> Image.imageName
   
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
                |> toList
            
 
            let envsFrom =
                state.Env
                |> List.filter (fun e ->
                    match e with
                    | Choice2Of2 (_) -> true
                    | _ -> false)
                |> List.map (fun env ->
                    match env with
                    | Choice2Of2 (e) -> e)
                |> List.toSeq           
                |> toList
                
            V1Container(name = name, imagePullPolicy = pp, image = imageName, env = envs, envFrom = envsFrom)

        [<CustomOperation("name")>]
        member this.Name(state: ContainerState, name: string) = { state with Name = Some(name) }

        [<CustomOperation("image")>]
        member this.Image(state: ContainerState, image: Image) = { state with Image = Some(image) }

        [<CustomOperation("image_pull_policy")>]
        member this.ImagePullPolicy(state: ContainerState, policy: ImagePullPolicy) =
            { state with ImagePullPolicy = policy }
            
        [<CustomOperation("env")>]
        member this.EnvVar(state: ContainerState, env: EnvironmentVariable list) = { state with Env = env |> List.map(fun e -> Environment.mapConfig(e))}

    let container = ContainerBuilder()
