module ConfigTests
open FSharpNetes
open Xunit
open FsUnit.Xunit
open k8s.Models


[<Fact>]
let ``Test ConfigMap`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let subject = 
        configMap {
            metadata meta
            data Map[("test", "xddd")]
            immutable true
        }

    subject.ApiVersion |> should equal "v1"
    subject.Kind |> should equal "ConfigMap"
    subject.Metadata.Name |> should equal meta.Name
    subject.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    subject.Data |> should haveCount 1
    subject.Data["test"] |> should equal "xddd"
    subject.Immutable |> should be True