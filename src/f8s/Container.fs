namespace FSharpNetes

open System
open System.Linq
open FSharpNetes.Utils

[<AutoOpen>]
module Container =
    open k8s
    open k8s.Models

    type ContainerPort =
        | TCP of int
        | UDP of int

    module private ContainerPort =
        let toKubeValue (port: ContainerPort) =
            match port with
            | TCP (port) -> V1ContainerPort(containerPort = port, protocol = "TCP")
            | UDP (port) -> V1ContainerPort(containerPort = port, protocol = "UDP")

    type Arg = Arg of string
    type Command = Command of string

    let private getArgs (args: Arg list option) : ResizeArray<string> =
        match args with
        | Some (argList) ->
            argList
            |> List.map (fun (Arg arg) -> arg)
            |> toList
        | None -> null

    let private getCommand (args: Command list option) : ResizeArray<string> =
        match args with
        | Some (argList) ->
            argList
            |> List.map (fun (Command arg) -> arg)
            |> toList
        | None -> null

    type ContainerState =
        { Name: string option
          Image: Image option
          ImagePullPolicy: ImagePullPolicy
          Args: Arg list option
          Command: Command list option
          Ports: ContainerPort list option
          Env: Choice<V1EnvVar, V1EnvFromSource> list }

    type ContainerBuilder internal () =
        member this.Yield(_) =
            { Name = None
              Image = None
              ImagePullPolicy = Always
              Env = []
              Command = None
              Ports = None
              Args = None }

        member this.Run(state: ContainerState) =
            let name =
                defaultArg state.Name (Guid.NewGuid().ToString())

            let ipp =
                state.ImagePullPolicy
                |> ImagePullPolicy.toKubeValue

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
            let command = state.Command |> getCommand

            let ports =
                state.Ports
                |> Option.map
                    (fun ports ->
                        ports
                        |> List.map (fun p -> p |> ContainerPort.toKubeValue)
                        |> toList)

            V1Container(
                name = name,
                imagePullPolicy = ipp,
                image = imageName,
                env = envs,
                command = command,
                envFrom = envsFrom,
                args = args,
                ports = defaultArg ports (null)
            )

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
            | Some (arguments) ->
                { state with
                      Args = Some(arguments @ [ arg ]) }
            | None -> { state with Args = Some([ arg ]) }

        [<CustomOperation("command")>]
        member this.AddCommand(state: ContainerState, cmd: string list) =
            let cmds = cmd |> List.map (fun c -> c |> Command)
            match state.Command with
            | Some (command) ->
                { state with
                      Command = Some(command @ cmds) }
            | None -> { state with Command = Some(cmds) }

        [<CustomOperation("ports")>]
        member this.AddPorts(state: ContainerState, ports: ContainerPort list) =
            match state.Ports with
            | Some (p) -> { state with Ports = Some(p @ ports) }
            | None -> { state with Ports = Some(ports) }

        [<CustomOperation("image_pull_policy")>]
        member this.ImagePullPolicy(state: ContainerState, policy: ImagePullPolicy) =
            { state with ImagePullPolicy = policy }

        [<CustomOperation("env")>]
        member this.EnvVar(state: ContainerState, env: EnvironmentVariables) =
            { state with
                  Env = env |> List.map (mapConfig) }

    let container = ContainerBuilder()
