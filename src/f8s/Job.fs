namespace FSharpNetes

open k8s.Models

[<AutoOpen>]
module CronJobSpec =
    open System

    type ConcurrencyPolicy = | Allow | Forbid | Replace with 
        static member ToKubernetesString(policy: ConcurrencyPolicy) =
            match policy with
            | Allow -> "Allow"
            | Forbid -> "Forbid"
            | Replace -> "Replace"

    type CronJobSpecBuilder internal () =
        member this.Yield(_) = V1CronJobSpec()

        member this.Run(state: V1CronJobSpec) = state

        [<CustomOperation("metadata")>]
        member _.Name(state: V1CronJobSpec, meta: V1ObjectMeta) =
            if state.JobTemplate |> isNull then
                state.JobTemplate <- V1JobTemplateSpec()
            state.JobTemplate.Metadata <- meta
            state

        [<CustomOperation("concurrencyPolicy")>]
        member _.ConcurrencyPolicy(state: V1CronJobSpec, policy: ConcurrencyPolicy) =
            state.ConcurrencyPolicy <- policy |> ConcurrencyPolicy.ToKubernetesString
            state

        [<CustomOperation("failedJobsHistoryLimit")>]
        member _.FailedJobsHistoryLimit(state: V1CronJobSpec, limit: int) =
            state.FailedJobsHistoryLimit <- limit
            state

        [<CustomOperation("startingDeadlineSeconds")>]
        member _.StartingDeadlineSeconds(state: V1CronJobSpec, seconds: int64) =
            state.StartingDeadlineSeconds <- seconds
            state

        [<CustomOperation("suspend")>]
        member _.Suspend(state: V1CronJobSpec, suspend: bool) =
            state.Suspend <- suspend
            state

        [<CustomOperation("successfulJobsHistoryLimit")>]
        member _.SuccessfulJobsHistoryLimit(state: V1CronJobSpec, limit: int) =
            state.SuccessfulJobsHistoryLimit <- limit
            state

        [<CustomOperation("schedule")>]
        member _.Name(state: V1CronJobSpec, schedule: string) =
            match schedule
                  |> Option.ofObj
                  |> Option.filter (fun x -> x |> String.IsNullOrEmpty |> not)
                with
            | Some (sch) ->
                state.Schedule <- sch
                state
            | None -> state

    let cronJobSpec = CronJobSpecBuilder()

[<AutoOpen>]
module CronJob =

    type private SelectorState = { MatchLabels: Map<string, string> }

    type Selector = MatchLabels of key: string * value: string

    type CronJobBuilder internal () =
        member this.Yield(_) =
            V1CronJob(apiVersion = "batch/v1", kind = "CronJob")

        member this.Run(state: V1CronJob) = state

        [<CustomOperation("metadata")>]
        member _.Name(state: V1CronJob, meta: V1ObjectMeta) =
            if isNull state.Metadata then
                state.Metadata <- meta
            state

        [<CustomOperation("spec")>]
        member _.Spec(state: V1CronJob, spec: V1CronJobSpec) =
            if isNull state.Spec then
                state.Spec <- spec
            state

    let cronjob = CronJobBuilder()
