module IngressTests

open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes

[<Fact>]
let ``Test Ingress`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
        annotations [Annotation("nginx.ingress.kubernetes.io/rewrite-target", "/")]
    }

    let subject = ingress {
        metadata meta
    }

    subject.ApiVersion |> should equal "networking.k8s.io/v1"
    subject.Kind |> should equal "Ingress"
    subject.Metadata.Name |> should equal meta.Name
    subject.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty