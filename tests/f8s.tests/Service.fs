module ServiceTests
open FSharpNetes
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Test Service`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let subject = service {
        metadata meta
        selector (ServiceSelector("app", "test"))
        port (ServicePort.TCP("http", 8080, 8080))
    }

    subject.ApiVersion |> should equal "v1"
    subject.Kind |> should equal "Service"
    subject.Spec.Selector |> should haveCount 1
    subject.Spec.Selector["app"] |> should equal "test"
    subject.Spec.Ports |> should haveCount 1
    subject.Spec.Ports[0].Protocol |> should equal "TCP"
    subject.Spec.Ports[0].Name |> should equal "http"
    subject.Spec.Ports[0].Port |> should equal 8080
    subject.Spec.Ports[0].TargetPort |> should equal (k8s.Models.IntstrIntOrString("8080"))

let ``Test ClusterIP Service`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let subject = service {
        metadata meta
        selector (ServiceSelector("app", "test"))
        port (ServicePort.TCP("http", 8080, 8080))
        
    }

    subject.ApiVersion |> should equal "v1"
    subject.Kind |> should equal "Service"
    subject.Spec.Selector |> should haveCount 1
    subject.Spec.Selector["app"] |> should equal "test"
    subject.Spec.Ports |> should haveCount 1
    subject.Spec.Ports[0].Protocol |> should equal "TCP"
    subject.Spec.Ports[0].Name |> should equal "http"
    subject.Spec.Ports[0].Port |> should equal 8080
    subject.Spec.Ports[0].TargetPort |> should equal (k8s.Models.IntstrIntOrString("8080"))