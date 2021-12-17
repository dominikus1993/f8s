namespace FSharpNetes

open System
open System.Linq
open FSharpNetes.Utils

[<AutoOpen>]
module Container =
    open k8s
    open k8s.Models

    type Probe =
        | HttpGet of {| path: string option; port: int; host: string option; headers: Map<string, string> option |}

    type LivenessProbe = { PeriodSeconds: int option; Probe: Probe }

    let private mapHeaders(h: Map<string, string>): V1HTTPHeader seq = 
        seq {
            for header in h do
                yield V1HTTPHeader(header.Key, header.Value)
        }

    let private mapHeadersOpt(h: Map<string, string> option): ResizeArray<V1HTTPHeader> = 
        match h with
        | Some(s) -> mapHeaders(s) |> Enumerable.toList
        | None -> null

    let mapLivenessProbe(l: LivenessProbe): V1Probe =
        match l.Probe with
        | HttpGet(arg) -> 
            let action = V1HTTPGetAction(arg.port, defaultArg arg.host null, mapHeadersOpt(arg.headers), defaultArg arg.path null)
            V1Probe(httpGet = action)

    let mapSecurityContext a = 
        V1SecurityContext(capabilities = V1Capabilities())

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
          Env: Choice<V1EnvVar, V1EnvFromSource> list
          Request: Resource option
          Limits: Resource option
          LivenessProbe: LivenessProbe option
          ReadinessProbe: LivenessProbe option
          SecurityContext: V1SecurityContext option }

    type ContainerBuilder internal () =
        member _.Yield(_) =
            { Name = None
              Image = None
              ImagePullPolicy = Always
              Env = []
              Command = None
              Ports = None
              Args = None 
              Request = None 
              Limits = None
              LivenessProbe = None
              ReadinessProbe = None
              SecurityContext = None }

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

            let limit = defaultArg (state.Limits |> Option.map(Resource.convertToK8s)) null
            let request = defaultArg (state.Request |> Option.map(Resource.convertToK8s)) null
            let livenessProbe = defaultArg (state.LivenessProbe |> Option.map(mapLivenessProbe)) null
            let readinessProbe = defaultArg (state.ReadinessProbe |> Option.map(mapLivenessProbe)) null
            let resources = V1ResourceRequirements(limits = limit, requests = request)

            V1Container(
                name = name,
                imagePullPolicy = ipp,
                image = imageName,
                env = envs,
                command = command,
                envFrom = envsFrom,
                args = args,
                livenessProbe = livenessProbe,
                readinessProbe = readinessProbe,
                ports = defaultArg ports (null),
                resources = resources
            )

        [<CustomOperation("name")>]
        member _.Name(state: ContainerState, name: string) =
            { state with
                  Name =
                      name
                      |> Option.ofObj
                      |> Option.filter (fun x -> not (String.IsNullOrEmpty(x))) }

        [<CustomOperation("image")>]
        member this.Image(state: ContainerState, image: Image) = { state with Image = Some(image) }

        [<CustomOperation("livenessProbe")>]
        member this.LivenessProbe(state: ContainerState, image: LivenessProbe) = { state with LivenessProbe = Some(image) }

        [<CustomOperation("readinessProbe")>]
        member this.ReadinessProbe(state: ContainerState, image: LivenessProbe) = { state with LivenessProbe = Some(image) }

        [<CustomOperation("args")>]
        member _.AddArgs(state: ContainerState, arg: Arg) =
            match state.Args with
            | Some (arguments) ->
                { state with
                      Args = Some(arguments @ [ arg ]) }
            | None -> { state with Args = Some([ arg ]) }

        [<CustomOperation("command")>]
        member _.AddCommand(state: ContainerState, cmd: string list) =
            let cmds = cmd |> List.map (fun c -> c |> Command)
            match state.Command with
            | Some (command) ->
                { state with
                      Command = Some(command @ cmds) }
            | None -> { state with Command = Some(cmds) }

        [<CustomOperation("ports")>]
        member _.AddPorts(state: ContainerState, ports: ContainerPort list) =
            match state.Ports with
            | Some (p) -> { state with Ports = Some(p @ ports) }
            | None -> { state with Ports = Some(ports) }

        [<CustomOperation("image_pull_policy")>]
        member _.ImagePullPolicy(state: ContainerState, policy: ImagePullPolicy) =
            { state with ImagePullPolicy = policy }

        [<CustomOperation("limit")>]
        member _.Limit(state: ContainerState, limit: Resource) =
            { state with Limits = Some(limit) }

        [<CustomOperation("request")>]
        member _.Request(state: ContainerState, req: Resource) =
            { state with Request = Some(req) }

        [<CustomOperation("securityContext")>]
        member _.SecurityContext(state: ContainerState, sec: V1SecurityContext) =
            { state with SecurityContext = Some(sec) }

        [<CustomOperation("env")>]
        member _.EnvVar(state: ContainerState, env: EnvironmentVariables) =
            { state with
                  Env = env |> List.map (mapConfig) }

    let container = ContainerBuilder()
