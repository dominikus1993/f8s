module JobTests

open System
open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes

[<Fact>]
let ``Test image with semVer tag`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }
    let nginxJob = job { 
        metadata meta
    }

    nginxJob.Kind |> should equal "Job"
    nginxJob.ApiVersion |> should equal "batch/v1"
    nginxJob.Metadata.Name |> should equal meta.Name
    nginxJob.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty

