namespace FSharpNetes

open k8s.Models

[<AutoOpen>]
module JobSpec =
    type V1JobSpecBuilder internal () =
        member this.Yield(_) =
            V1JobSpec()

        member this.Run(state: V1JobSpec) = state

        [<CustomOperation("activeDeadlineSeconds")>]
        member _.ActiveDeadlineSeconds(state: V1JobSpec, sec: int64) =
            if not state.ActiveDeadlineSeconds.HasValue then
                state.ActiveDeadlineSeconds <- sec
            state

        [<CustomOperation("suspend")>]
        member _.Suspend(state: V1JobSpec, suspend: bool) =
            state.Suspend <- suspend
            state

        [<CustomOperation("parallelism")>]
        member _.Parallelism(state: V1JobSpec, p: int) =
            if not state.Parallelism.HasValue then
                state.Parallelism <- p
            state

        [<CustomOperation("backoffLimit")>]
        member _.BackoffLimit(state: V1JobSpec, p: int) =
            if not state.BackoffLimit.HasValue then
                state.BackoffLimit <- p
            state

        [<CustomOperation("pod")>]
        member _.Pod(state: V1JobSpec, pod: V1PodSpec) =
            if isNull state.Template || isNull state.Template.Spec then
                state.Template <- V1PodTemplateSpec(spec = pod)
            state           

    let jobSpec = V1JobSpecBuilder()

[<AutoOpen>]
module CronJobTemplate =
    type CronJobTemplateBuilder internal () =
        member this.Yield(_) =
            V1JobTemplateSpec()

        member this.Run(state: V1JobTemplateSpec) = state

        [<CustomOperation("metadata")>]
        member _.Name(state: V1JobTemplateSpec, meta: V1ObjectMeta) =
            if isNull state.Metadata then
                state.Metadata <- meta
            state

        [<CustomOperation("spec")>]
        member _.Spec(state: V1JobTemplateSpec, spec: V1JobSpec) =
            if isNull state.Spec then
                state.Spec <- spec
            state
    let cronjobTemplate = CronJobTemplateBuilder()


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
    open System

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

        [<CustomOperation("concurrencyPolicy")>]
        member _.ConcurrencyPolicy(state: V1CronJob, policy: ConcurrencyPolicy) =
            if isNull state.Spec then state.Spec <- V1CronJobSpec()
            state.Spec.ConcurrencyPolicy <- policy |> ConcurrencyPolicy.ToKubernetesString
            state

        [<CustomOperation("failedJobsHistoryLimit")>]
        member _.FailedJobsHistoryLimit(state: V1CronJob, limit: int) =
            if isNull state.Spec then state.Spec <- V1CronJobSpec()
            state.Spec.FailedJobsHistoryLimit <- limit
            state

        [<CustomOperation("startingDeadlineSeconds")>]
        member _.StartingDeadlineSeconds(state: V1CronJob, seconds: int64) =
            if isNull state.Spec then state.Spec <- V1CronJobSpec()
            state.Spec.StartingDeadlineSeconds <- seconds
            state

        [<CustomOperation("suspend")>]
        member _.Suspend(state: V1CronJob, suspend: bool) =
            if isNull state.Spec then state.Spec <- V1CronJobSpec()
            state.Spec.Suspend <- suspend
            state

        [<CustomOperation("successfulJobsHistoryLimit")>]
        member _.SuccessfulJobsHistoryLimit(state: V1CronJob, limit: int) =
            if isNull state.Spec then state.Spec <- V1CronJobSpec()
            state.Spec.SuccessfulJobsHistoryLimit <- limit
            state

        [<CustomOperation("schedule")>]
        member _.Name(state: V1CronJob, schedule: string) =
            if isNull state.Spec then state.Spec <- V1CronJobSpec()
            match schedule
                  |> Option.ofObj
                  |> Option.filter (fun x -> x |> String.IsNullOrEmpty |> not)
                with
            | Some (sch) ->
                state.Spec.Schedule <- sch
                state
            | None -> state

        [<CustomOperation("activeDeadlineSeconds")>]
        member _.ActiveDeadlineSeconds(state: V1CronJob, sec: int64) =
            if isNull state.Spec then state.Spec <- V1CronJobSpec()
            if isNull state.Spec.JobTemplate then state.Spec.JobTemplate <- V1JobTemplateSpec()
            if isNull state.Spec.JobTemplate.Spec then state.Spec.JobTemplate.Spec <- V1JobSpec()

            if not state.Spec.JobTemplate.Spec.ActiveDeadlineSeconds.HasValue then
                state.Spec.JobTemplate.Spec.ActiveDeadlineSeconds <- sec
            state

        [<CustomOperation("parallelism")>]
        member _.Parallelism(state: V1CronJob, p: int) =
            if isNull state.Spec then state.Spec <- V1CronJobSpec()
            if isNull state.Spec.JobTemplate then state.Spec.JobTemplate <- V1JobTemplateSpec()
            if isNull state.Spec.JobTemplate.Spec then state.Spec.JobTemplate.Spec <- V1JobSpec()
            if not state.Spec.JobTemplate.Spec.Parallelism.HasValue then
                state.Spec.JobTemplate.Spec.Parallelism <- p
            state

        [<CustomOperation("backoffLimit")>]
        member _.BackoffLimit(state: V1CronJob, p: int) =
            if isNull state.Spec then state.Spec <- V1CronJobSpec()
            if isNull state.Spec.JobTemplate then state.Spec.JobTemplate <- V1JobTemplateSpec()
            if isNull state.Spec.JobTemplate.Spec then state.Spec.JobTemplate.Spec <- V1JobSpec()
            if not state.Spec.JobTemplate.Spec.BackoffLimit.HasValue then
                state.Spec.JobTemplate.Spec.BackoffLimit <- p
            state

        [<CustomOperation("pod")>]
        member _.Pod(state: V1CronJob, pod: V1PodSpec) =
            if isNull state.Spec then state.Spec <- V1CronJobSpec()
            if isNull state.Spec.JobTemplate then state.Spec.JobTemplate <- V1JobTemplateSpec()
            if isNull state.Spec.JobTemplate.Spec then state.Spec.JobTemplate.Spec <- V1JobSpec()
            if isNull state.Spec.JobTemplate.Spec then state.Spec.JobTemplate.Spec <- V1JobSpec()
            if isNull state.Spec.JobTemplate.Spec.Template then state.Spec.JobTemplate.Spec.Template <- V1PodTemplateSpec()
            state.Spec.JobTemplate.Spec.Template <- V1PodTemplateSpec(spec = pod)
            state      

    let cronjob = CronJobBuilder()
