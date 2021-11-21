module MetaDataTests

open System
open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes

[<Fact>]
let ``Test metadata`` () =
    let subject = metadata {
        name "test"
        nmspc "xD"
        label (Label("test", "test"))
    }
    
    subject.Name |> should equal "test"
    subject.NamespaceProperty |> should equal "xD"
    subject.Labels.["test"] |> should equal "test"

let ``Test metadata labels`` () =
    let subject = metadata {
        name "test"
        nmspc "xD"
        labels ([Label("test", "test"); Label("test2", "test2")])
    }
    
    subject.Name |> should equal "test"
    subject.NamespaceProperty |> should equal "xD"
    subject.Labels["test"] |> should equal "test"
    subject.Labels["test2"] |> should equal "test2"