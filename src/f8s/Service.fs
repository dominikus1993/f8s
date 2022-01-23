namespace FSharpNetes
open k8s.Models
open k8s

[<AutoOpen>]
module ServicePortSpec =

    type ServicePortBuilder internal() =
        member this.Yield(_) =
            V1ServicePort()

        member this.Run(state: V1ServicePort) = state

        [<CustomOperation("name")>]
        member _.Name(state: V1ServicePort, name: string) = 
            state.Name <- name
            state

        [<CustomOperation("porotocol")>]
        member _.Protocol(state: V1ServicePort, porotocol: string) = 
            state.AppProtocol <- porotocol
            state

        [<CustomOperation("nodePort")>]
        member _.Protocol(state: V1ServicePort, port: int) = 
            state.NodePort <- port
            state

        [<CustomOperation("port")>]
        member _.Port(state: V1ServicePort, port: int) = 
            state.Port <- port
            state

        [<CustomOperation("targetPort")>]
        member _.TargetPort(state: V1ServicePort, port: int) = 
            state.TargetPort <- port
            state      
            
    let servicePort = ServicePortBuilder()

[<AutoOpen>]
module ServiceSpec =
    open System.Collections.Generic

    type ServiceType =
    | NodePort
    | ClusterIp
    | LoadBalancer with
        static member ToKubernetesString(policy: ServiceType) =
            match policy with
            | NodePort -> "NodePort"
            | ClusterIp -> "ClusterIP"
            | LoadBalancer -> "LoadBalancer"

    type ServiceSelector = ServiceSelector of name: string * value: string

    let private mapSelector (selector: ServiceSelector) (map) : Map<string, string> =
        let (ServiceSelector(name, value)) = selector
        Map.add name value map
          
    let private mapSelectors (selector: ServiceSelector list) =
        selector |> List.fold(fun acc s -> mapSelector s acc) Map.empty

    type ServiceSpecBuilder internal() =
        member this.Yield(_) =
            V1ServiceSpec()

        member this.Run(state: V1ServiceSpec) = state   

        [<CustomOperation("clusterIP")>]
        member this.ClusterIp (state: V1ServiceSpec, ip: string) =
            state.ClusterIP <- ip
            state          

        [<CustomOperation("type")>]
        member this.Type (state: V1ServiceSpec, t: ServiceType) =
            state.Type <- (t |> ServiceType.ToKubernetesString)
            state    

        [<CustomOperation("selector")>]
        member this.Selector (state: V1ServiceSpec, selector: ServiceSelector) =
            this.Selectors(state, [selector])

        [<CustomOperation("selectors")>]
        member this.Selectors (state: V1ServiceSpec, selectors: ServiceSelector list) =
            if isNull state.Selector then state.Selector <- Dictionary()
            let s = selectors |> mapSelectors
            for selector in s do 
                state.Selector.Add(selector)
            state
        
        [<CustomOperation("port")>]
        member _.Port(state: V1ServiceSpec, port: V1ServicePort) =
            if isNull state.Ports then  state.Ports <- ResizeArray()
            state.Ports.Add(port)
            state 

    let serviceSpec = ServiceSpecBuilder()

[<AutoOpen>]
module Service =
      
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

    let service = ServiceBuilder()
            