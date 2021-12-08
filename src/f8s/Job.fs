namespace FSharpNetes

open k8s.Models

[<AutoOpen>]
module Job =

    type private SelectorState = { MatchLabels: Map<string, string> }
    
    type Selector = 
        | MatchLabels of key: string * value: string

    type JobState = { MetaData: V1ObjectMeta option; JobSpec: V1JobSpec option; }


    type PodBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; Containers = None }
        
        member this.Run(state: PodState) = 
            V1Job(apiVersion = "v1", kind = "Pod")