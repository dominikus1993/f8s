namespace FSharpNetes

open k8s.Models
open k8s
open System
[<AutoOpen>]
module CronJob =
    type CronJobState = { Spec: V1PodSpec option; MetaData: V1ObjectMeta option; Schedule: string option; FailedJobsHistoryLimit: int option }
    
    let private getCronJobSchedule schedule =
        match schedule with
        | Some (cron) -> cron
        | _ -> failwith "No cron schedule provided"        
    
    type CronJobBuilder internal () =
        member _.Yield(_) =
            { Spec = None; MetaData = None; Schedule = None; FailedJobsHistoryLimit = None}
        
        member _.Run(state: CronJobState) = 
            let podSpec = defaultArg state.Spec (V1PodSpec(containers = ResizeArray()))
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            let schedule = getCronJobSchedule(state.Schedule)
            let jobSpec = V1JobSpec(V1PodTemplateSpec(spec = podSpec))
            let jobTemplate = V1beta1JobTemplateSpec(spec = jobSpec)
            let failedJobsHistoryLimit = state.FailedJobsHistoryLimit |> Option.toNullable
            let spec = V1beta1CronJobSpec(jobTemplate, schedule, failedJobsHistoryLimit = failedJobsHistoryLimit)
            V1beta1CronJob(metadata = meta, spec = spec, apiVersion = "batch/v1beta1")

        [<CustomOperation("metadata")>]
        member _.Name (state: CronJobState, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }

        [<CustomOperation("schedule")>]
        member _.Schedule (state: CronJobState, schedule: string) =
            { state with Schedule = schedule |> Option.ofObj |> Option.filter(fun x -> String.IsNullOrEmpty(x) |> not) }

        [<CustomOperation("failedJobsHistoryLimit")>]
        member _.AddFailedJobsHistoryLimit (state: CronJobState, fjhl: int) =
            { state with FailedJobsHistoryLimit = Some(fjhl) }                

        [<CustomOperation("spec")>]
        member _.AddSpec (state: CronJobState, podSpec: V1PodSpec) =
            { state with Spec = Some(podSpec) }   

        [<CustomOperation("container")>]
        member _.AddContainer (state: CronJobState, container: V1Container) =
            match state.Spec with
            | Some(spec) ->
                spec.Containers.Add(container)
                state
            | None ->
                let containers = ResizeArray()
                containers.Add(container)
                { state with Spec = Some(V1PodSpec(containers)) }
            
    
    let cronJob = CronJobBuilder()

        