module ServiceTests
open FSharpNetes
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Test Service selector`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let sp = serviceSpec {
        selector (ServiceSelector("app", "test"))
    }

    let subject = service {
        metadata meta
        spec sp
    }

    subject.ApiVersion |> should equal "v1"
    subject.Kind |> should equal "Service"
    subject.Spec.Selector |> should haveCount 1
    subject.Spec.Selector["app"] |> should equal "test"
