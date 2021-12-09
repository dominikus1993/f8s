namespace FSharpNetes

open k8s.Models

[<AutoOpen>]
module Job =

    type private SelectorState = { MatchLabels: Map<string, string> }
    
    type Selector = 
        | MatchLabels of key: string * value: string

    type JobState = { MetaData: V1ObjectMeta option; JobSpec: V1JobSpec option; }


    type JobBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; JobSpec = None }
        
        member this.Run(state: JobState) = 
            let metadata = defaultArg state.MetaData null
            V1Job(apiVersion = "batch/v1", kind = "Job", metadata = metadata)

        [<CustomOperation("metadata")>]
        member _.Name (state: JobState, meta: V1ObjectMeta) = 
            { state with MetaData = Some(meta) }
    

    let job = JobBuilder()