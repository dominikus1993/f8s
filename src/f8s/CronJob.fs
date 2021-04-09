namespace FSharpNetes

open k8s.Models
open k8s

[<AutoOpen>]
module CronJob =
    type CronJobState = { MetaData: V1ObjectMeta option }
    
    type CronJobBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; }
        
        member this.Run(state: CronJobState) = 
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            V1Namespace(metadata = meta)

        [<CustomOperation("metadata")>]
        member this.Name (state: CronJobState, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }
            
    
    let nmspc = CronJobBuilder()

        