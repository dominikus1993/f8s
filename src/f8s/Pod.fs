namespace FSharpNetes
open k8s.Models
open k8s

[<AutoOpen>]
module Pod =
    type PodState = { MetaData: V1ObjectMeta option }
    
    type PodBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; }
        
        member this.Run(state: NamespaceState) = 
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            V1Pod(metadata = meta)

        [<CustomOperation("metadata")>]
        member this.Name (state: NamespaceState, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }
            