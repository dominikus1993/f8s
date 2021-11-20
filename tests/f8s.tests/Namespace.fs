module NamespaceTests

open System
open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes

[<Fact>]
let ``Test one name value env`` () =
    let meta = metadata {
        name "test"
        label (Label("test", "test"))
    }
    let testNamespace = nmspc {
        metadata meta
    }
    
    testNamespace.ApiVersion |> should equal "v1"
    testNamespace.Kind |> should equal "Namespace"
    testNamespace.Metadata.Name |> should equal "test"
    let label = testNamespace.Metadata.Labels.["test"]
    label |> should equal "test"
