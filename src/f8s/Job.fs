namespace FSharpNetes

open k8s.Models

[<AutoOpen>]
module CronJobSpec =
    open System

    type CronJobSpecState = { Schedule: string option } 
    type CronJobSpecBuilder internal () =
        member this.Yield(_) =
            { Schedule = None }
        
        member this.Run(state: CronJobSpecState) = 
            let schedule = defaultArg state.Schedule null
            let template = V1JobTemplateSpec()
            V1CronJobSpec(template, schedule = schedule)

        [<CustomOperation("schedule")>]
        member _.Name (state: CronJobSpecState, schedule: string) = 
            { state with Schedule = schedule |> Option.ofObj |> Option.filter(fun x -> x |> String.IsNullOrEmpty |> not) }

    let cronJobSpec = CronJobSpecBuilder()

[<AutoOpen>]
module CronJob =

    type private SelectorState = { MatchLabels: Map<string, string> }
    
    type Selector = 
        | MatchLabels of key: string * value: string

    type CronJobState = { MetaData: V1ObjectMeta option; JobSpec: V1CronJobSpec option; }


    type CronJobBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; JobSpec = None }
        
        member this.Run(state: CronJobState) = 
            let metadata = defaultArg state.MetaData null
            let spec = defaultArg state.JobSpec null
            V1CronJob(apiVersion = "batch/v1", kind = "CronJob", metadata = metadata, spec = spec)

        [<CustomOperation("metadata")>]
        member _.Name (state: CronJobState, meta: V1ObjectMeta) = 
            { state with MetaData = meta |> Option.ofObj }

        [<CustomOperation("spec")>]
        member _.Spec (state: CronJobState, spec: V1CronJobSpec) = 
            { state with JobSpec = (Option.ofObj spec) }
    

    let cronjob = CronJobBuilder()