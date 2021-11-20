namespace FSharpNetes
open k8s.Models
open k8s

[<AutoOpen>]
module Service =
    type ServiceType =
        | NodePort
        | ClusterIp
        | LoadBalancer

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

    type ServiceState = { MetaData: V1ObjectMeta option; Ports: ServicePort list option; Selector: ServiceSelector list option  }

    type ServiceBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; Ports = None; Selector = None }
        
        member this.Run(state: ServiceState) = 
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            let ports = defaultArg state.Ports [] |> List.map(mapPort) |> Utils.toList
            let selectors = defaultArg state.Selector [] |> mapSelectors 
            V1Service(metadata = meta, spec = V1ServiceSpec(ports = ports, selector = selectors), kind = "Service", apiVersion = "v1")

        [<CustomOperation("metadata")>]
        member this.Name (state: ServiceState, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }

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
            