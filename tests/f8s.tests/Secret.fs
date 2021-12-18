module SecretTests

open System
open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes

[<Fact>]
let ``Test empty secret`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let subject = secret {
        metadata meta
    }

    subject.ApiVersion |> should equal "v1"
    subject.Kind |> should equal "Secret"
    subject.Metadata.Name |> should equal meta.Name
    subject.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty

[<Fact>]
let ``Test Opaque data secret`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let subject = secret {
        metadata meta
        ``type`` Opaque
        data (Map[("test", "eEREREREMjEzNw==")])
    }

    subject.ApiVersion |> should equal "v1"
    subject.Kind |> should equal "Secret"
    subject.Metadata.Name |> should equal meta.Name
    subject.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    subject.Type |> should equal "Opaque"
    subject.Data |> should haveCount 1
    subject.Data["test"] |> should not' (be Null)

[<Fact>]
let ``Test Opaque string data secret`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let subject = secret {
        metadata meta
        ``type`` Opaque
        stringData (Map[("test", "eEREREREMjEzNw==")])
    }

    subject.ApiVersion |> should equal "v1"
    subject.Kind |> should equal "Secret"
    subject.Metadata.Name |> should equal meta.Name
    subject.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    subject.Type |> should equal "Opaque"
    subject.StringData |> should haveCount 1
    subject.StringData["test"] |> should not' (be Null)
    subject.StringData["test"] |> should equal "eEREREREMjEzNw=="