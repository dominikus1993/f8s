module JobTests

open System
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
    nginxJob.Spec.JobTemplate.Metadata.Name |> should equal meta.Name
    nginxJob.Spec.JobTemplate.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty   

