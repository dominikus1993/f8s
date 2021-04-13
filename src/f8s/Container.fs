namespace FSharpNetes

open System
open System.Linq
open FSharpNetes.Utils

[<AutoOpen>]
module Container =
    open k8s
    open k8s.Models

    type Arg = Arg of string
    type ImagePullPolicy =
        | Always
        | IfNotPresent
        | Never
        member internal this.ToKubeValue() =
            match this with
            | Always -> "Always"
            | IfNotPresent -> "IfNotPresent"
            | Never -> "Never"

    let private getArgs(args: Arg list option) : ResizeArray<string> =
        let getArg(a: Arg) =
            let (Arg(arg)) = a
            arg

        match args with
        | Some(argList) -> 
            argList |> List.map(getArg) |> toList
        | None ->
            null

    type ContainerState =
        { Name: string option
          Image: Image option
          ImagePullPolicy: ImagePullPolicy
          Args: Arg list option
          Env: Choice<V1EnvVar, V1EnvFromSource> list }

    type ContainerBuilder internal () =
        member this.Yield(_) =
            { Name = None
              Image = None
              ImagePullPolicy = Always
              Env = []
              Args = None }

        member this.Run(state: ContainerState) =
            let name =
                defaultArg state.Name (Guid.NewGuid().ToString())

            let ipp = state.ImagePullPolicy.ToKubeValue()

            let image =
                match state.Image with
                | Some (i) -> i
                | _ -> failwith "No image provided"

            let imageName = image |> Image.imageName

            let envs =
                state.Env
                |> List.choose
                    (fun e ->
                        match e with
                        | Choice1Of2 (e) -> Some(e)
                        | _ -> None)
                |> toList

            let envsFrom =
                state.Env
                |> List.choose
                    (fun e ->
                        match e with
                        | Choice2Of2 (e) -> Some(e)
                        | _ -> None)
                |> toList
            
            let args = state.Args |> getArgs

            V1Container(name = name, imagePullPolicy = ipp, image = imageName, env = envs, envFrom = envsFrom, args = args)

        [<CustomOperation("name")>]
        member this.Name(state: ContainerState, name: string) =
            { state with
                  Name =
                      name
                      |> Option.ofObj
                      |> Option.filter (fun x -> not (String.IsNullOrEmpty(x))) }

        [<CustomOperation("image")>]
        member this.Image(state: ContainerState, image: Image) = { state with Image = Some(image) }

        [<CustomOperation("args")>]
        member this.AddArgs(state: ContainerState, arg: Arg) =
            match state.Args with
            | Some(arguments) -> 
                { state with Args = Some(arg :: arguments)}
            | None -> 
                { state with Args = Some([arg])}

        [<CustomOperation("image_pull_policy")>]
        member this.ImagePullPolicy(state: ContainerState, policy: ImagePullPolicy) =
            { state with ImagePullPolicy = policy }

        [<CustomOperation("env")>]
        member this.EnvVar(state: ContainerState, env: EnvironmentVariables) =
            { state with
                  Env =
                      env
                      |> List.map (fun e -> Environment.mapConfig (e)) }

    let container = ContainerBuilder()
