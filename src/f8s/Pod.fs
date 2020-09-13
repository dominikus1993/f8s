namespace FSharpNetes 
open System
open FSharpNetes.Utils

[<AutoOpen>]
module Pod = 
    open k8s
    open k8s.Models

    type PodState = { Containers: V1Container list; Api: Api option}

    type PodBuilder internal () =
        member this.Yield(_) =
            { Containers = []; Api = None }
        
        member this.Run(state: PodState) =
            let containers = state.Containers |> toList
            let api = defaultArg state.Api V1 |> Api.value
            let spec = V1PodSpec(containers = containers)
            V1Pod(spec = spec, apiVersion = api)
            
        [<CustomOperation("api")>]
        member this.AddApi (state: PodState, api: Api) =
            { state with Api = Some(api) }           
        
        [<CustomOperation("add_container")>]
        member this.AddContainer (state: PodState, container: V1Container) =
            { state with Containers = container :: state.Containers }                   

    let pod = PodBuilder()

