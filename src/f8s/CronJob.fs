namespace FSharpNetes

open k8s.Models
open k8s

[<AutoOpen>]
module CronJob =
    type CronJobSchedule = | CronJobSchedule of string
    type CronJobState = { MetaData: V1ObjectMeta option; Schedule: CronJobSchedule option }
    
    let private getCronJobSchedule schedule =
        match schedule with
        | Some (CronJobSchedule(cron)) -> cron
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
            
    
    let nmspc = CronJobBuilder()

        