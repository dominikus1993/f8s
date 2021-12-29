module JobTests

open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes

[<Fact>]
let ``Test cron job`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }
    let nginxSpec = cronJobSpec {
        metadata meta
        concurrencyPolicy Allow
        suspend true 
        failedJobsHistoryLimit 1
        successfulJobsHistoryLimit 2
        startingDeadlineSeconds 22
        schedule "* * * * *"
    }
    let nginxJob = cronjob { 
        metadata meta
        spec nginxSpec
    }

    nginxJob.Kind |> should equal "CronJob"
    nginxJob.ApiVersion |> should equal "batch/v1"
    nginxJob.Metadata.Name |> should equal meta.Name
    nginxJob.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    nginxJob.Spec.Schedule |> should equal "* * * * *"
    nginxJob.Spec.ConcurrencyPolicy |> should equal "Allow"
    nginxJob.Spec.Suspend |> should be True
    nginxJob.Spec.FailedJobsHistoryLimit |> should equal 1
    nginxJob.Spec.SuccessfulJobsHistoryLimit |> should equal 2
    nginxJob.Spec.StartingDeadlineSeconds |> should equal 22L



[<Fact>]
let ``Test cron job wit spec`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }
    let nginxJob = cronjob { 
        metadata meta
        concurrencyPolicy Allow
        suspend true 
        failedJobsHistoryLimit 1
        successfulJobsHistoryLimit 2
        startingDeadlineSeconds 22
        schedule "* * * * *"
    }

    nginxJob.Kind |> should equal "CronJob"
    nginxJob.ApiVersion |> should equal "batch/v1"
    nginxJob.Metadata.Name |> should equal meta.Name
    nginxJob.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    nginxJob.Spec.Schedule |> should equal "* * * * *"
    nginxJob.Spec.ConcurrencyPolicy |> should equal "Allow"
    nginxJob.Spec.Suspend |> should be True
    nginxJob.Spec.FailedJobsHistoryLimit |> should equal 1
    nginxJob.Spec.SuccessfulJobsHistoryLimit |> should equal 2
    nginxJob.Spec.StartingDeadlineSeconds |> should equal 22L

