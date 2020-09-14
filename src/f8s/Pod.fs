namespace FSharpNetes 
open System
open FSharpNetes.Utils

[<AutoOpen>]
module Pod = 
    open k8s
    open k8s.Models

    type PodState = { Containers: V1Container list; Api: Api option; MetaData: V1ObjectMeta option}

    type PodBuilder internal () =
        member this.Yield(_) =
            { Containers = []; Api = None; MetaData = None }
        
        member this.Run(state: PodState) =
            let containers = state.Containers |> toList
            let api = defaultArg state.Api V1 |> Api.value
            let spec = V1PodSpec(containers = containers)
            let meta = defaultArg state.MetaData null
            V1Pod(spec = spec, apiVersion = api, metadata = meta, kind = "Pod")
            
        [<CustomOperation("api")>]
        member this.AddApi (state: PodState, api: Api) =
            { state with Api = Some(api) }           

        [<CustomOperation("metadata")>]
        member this.AddMetadata (state: PodState, meta: V1ObjectMeta) =
            let metaOpt = meta |> Option.ofObj
            { state with MetaData = metaOpt}              
                
        [<CustomOperation("add_container")>]
        member this.AddContainer (state: PodState, container: V1Container) =
            { state with Containers = container :: state.Containers }                   

    let pod = PodBuilder()

