namespace FSharpNetes

open k8s.Models
open k8s
open System
[<AutoOpen>]
module CronJob =
    type CronJobState = { MetaData: V1ObjectMeta option; Schedule: string option }
    
    let private getCronJobSchedule schedule =
        match schedule with
        | Some (cron) -> cron
        | _ -> failwith "No cron schedule provided"        
    
    type CronJobBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; Schedule = None}
        
        member this.Run(state: CronJobState) = 
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            let schedule = getCronJobSchedule(state.Schedule)
            let jobSpec = V1JobSpec()
            let jobTemplate = V1beta1JobTemplateSpec(spec = jobSpec)
            let spec = V1beta1CronJobSpec(jobTemplate, schedule)
            V1beta1CronJob(metadata = meta, spec = spec)

        [<CustomOperation("metadata")>]
        member this.Name (state: CronJobState, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }

        [<CustomOperation("schedule")>]
        member this.Schedule (state: CronJobState, schedule: string) =
            { state with Schedule = schedule |> Option.ofObj |> Option.filter(fun x -> String.IsNullOrEmpty(x) |> not) }
            
    
    let cronJob = CronJobBuilder()

        