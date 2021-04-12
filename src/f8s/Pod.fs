namespace FSharpNetes 
open System
open FSharpNetes.Utils

[<AutoOpen>]
module PodSpec = 
    open k8s
    open k8s.Models

    type PodSpecState = { Containers: V1Container list }

    type PodSpecBuilder internal () =
        member this.Yield(_) =
            { Containers = []; }
        
        member this.Run(state: PodSpecState) =
            let containers = state.Containers |> toList
            V1PodSpec(containers = containers)                      
                
        [<CustomOperation("container")>]
        member this.AddContainer (state: PodSpecState, container: V1Container) =
            { state with Containers = container :: state.Containers }                   

    let pod = PodSpecBuilder()    

[<AutoOpen>]
module Pod = 
    open k8s
    open k8s.Models

    type PodState = { Spec: V1PodSpec option; MetaData: V1ObjectMeta option}

    type PodBuilder internal () =
        member this.Yield(_) =
            { Spec = None; MetaData = None }
        
        member this.Run(state: PodState) =
            let spec = defaultArg state.Spec (V1PodSpec(containers = ResizeArray()))
            let meta = defaultArg state.MetaData null
            V1Pod(spec = spec, apiVersion = "v1", metadata = meta, kind = "Pod")                

        [<CustomOperation("spec")>]
        member this.AddSpec (state: PodState, podSpec: V1PodSpec) =
            { state with Spec = Some(podSpec) }     

        [<CustomOperation("metadata")>]
        member this.AddMetadata (state: PodState, meta: V1ObjectMeta) =
            let metaOpt = meta |> Option.ofObj
            { state with MetaData = metaOpt}              
                       
        [<CustomOperation("container")>]
        member this.AddContainer (state: PodState, container: V1Container) =
            match state.Spec with
            | Some(spec) ->
                spec.Containers.Add(container)
                state
            | None ->
                let containers = ResizeArray()
                containers.Add(container)
                { state with Spec = Some(V1PodSpec(containers)) }
              
    let pod = PodBuilder()

    let create (client: Kubernetes)(pod: V1Pod) =
        client.CreateNamespacedPodAsync(pod, pod.Namespace()) |> Async.AwaitTask |> Async.Ignore
        