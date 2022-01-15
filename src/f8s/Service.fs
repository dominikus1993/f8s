namespace FSharpNetes
open k8s.Models
open k8s

module ServiceSpec = 
    type ServiceSpecBuilder internal() =
        member this.Yield(_) =
            V1ServiceSpec()

        member this.Run(state: V1ServiceSpec) = state   

        [<CustomOperation("port")>]
        member this.Port (state: V1ServiceSpec, port: ServicePort) =
            this.Ports(state, [port])

        [<CustomOperation("ports")>]
        member this.Ports (state: V1ServiceSpec, ports: ServicePort list) =
            match state.Ports with
            | Some(cont) ->
                { state with Ports = Some(cont @ ports)}
            | None ->
                { state with Ports = Some(ports)}


[<AutoOpen>]
module Service =
    type ServiceType =
        | NodePort
        | ClusterIp
        | LoadBalancer with
            static member ToKubernetesString(policy: ServiceType) =
                match policy with
                | NodePort -> "NodePort"
                | ClusterIp -> "ClusterIP"
                | LoadBalancer -> "LoadBalancer"

    type ServicePort =
        | TCP of name: string * port: int * targetPort: int

    let private mapPort (port: ServicePort) =
        match port with 
        | TCP(name, port, targetPort) -> V1ServicePort(port, name = name, targetPort = targetPort, protocol = "TCP")

    type ServiceSelector = ServiceSelector of name: string * value: string

    let private mapSelector (selector: ServiceSelector) (map) : Map<string, string> =
        let (ServiceSelector(name, value)) = selector
        Map.add name value map

    let private mapSelectors (selector: ServiceSelector list) =
        selector |> List.fold(fun acc s -> mapSelector s acc) Map.empty
    type ServiceBuilder internal () =
        member this.Yield(_) =
            V1Service(kind = "Service", apiVersion = "v1")

        [<CustomOperation("apiVersion")>]
        member this.ApiVersion (state: V1Service, apiVersion: string) =
            state.ApiVersion <- apiVersion
            state

        member this.Run(state: V1Service) = state
            // let meta = defaultArg state.MetaData (V1ObjectMeta())
            // let ports = defaultArg state.Ports [] |> List.map(mapPort) |> Utils.toList
            // let selectors = defaultArg state.Selector [] |> mapSelectors 
            // let t = defaultArg (state.ServiceType |> Option.map(ServiceType.ToKubernetesString)) null
            // V1Service(metadata = meta, spec = V1ServiceSpec(ports = ports, selector = selectors, ``type``= t), kind = "Service", apiVersion = "v1")

        [<CustomOperation("metadata")>]
        member this.Metadata (state: V1Service, meta: V1ObjectMeta) =
            state.Metadata <- meta
            state

        [<CustomOperation("spec")>]
        member this.Spec (state: V1Service, meta: V1ServiceSpec) =
            state.Spec <- meta
            state

        [<CustomOperation("port")>]
        member this.Port (state: ServiceState, port: ServicePort) =
            this.Ports(state, [port])

        [<CustomOperation("ports")>]
        member this.Ports (state: ServiceState, ports: ServicePort list) =
            match state.Ports with
            | Some(cont) ->
                { state with Ports = Some(cont @ ports)}
            | None ->
                { state with Ports = Some(ports)}

        [<CustomOperation("type")>]
        member _.Type (state: ServiceState, t: ServiceType) =
            { state with ServiceType = Some(t)}

        [<CustomOperation("selector")>]
        member this.Selector (state: ServiceState, selector: ServiceSelector) =
            this.Selectors(state, [selector])

        [<CustomOperation("selectors")>]
        member this.Selectors (state: ServiceState, selectors: ServiceSelector list) =
            match state.Selector with
            | Some(cont) ->
                { state with Selector = Some(cont @ selectors)}
            | None ->
                { state with Selector = Some(selectors)}

    let service = ServiceBuilder()
            