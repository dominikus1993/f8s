namespace FSharpNetes
open k8s.Models
open k8s

[<AutoOpen>]
module Pod =
    type PodState = { MetaData: V1ObjectMeta option; Containers: V1Container list option }
    
    type PodBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; Containers = None }
        
        member this.Run(state: PodState) = 
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            let containers = defaultArg state.Containers [] |> Utils.toList
            V1Pod(metadata = meta, spec = V1PodSpec(containers = containers))

        [<CustomOperation("metadata")>]
        member this.Name (state: PodState, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }

        [<CustomOperation("containers")>]
        member this.Containers (state: PodState, containers: V1Container list) =
            match state.Containers with
            | Some(cont) ->
                { state with Containers = Some(cont @ containers)}
            | None ->
                { state with Containers = Some(containers)}
    
    let pod = PodBuilder()
            