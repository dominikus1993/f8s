module ServiceTests
open FSharpNetes
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Test ServiceType ToKubernetesString NodePort`` () =
    let t = NodePort
    let subject = t |> ServiceType.ToKubernetesString
    subject |> should equal "NodePort"

[<Fact>]
let ``Test ServiceType ToKubernetesString ClusterIp`` () =
    let t = ClusterIp
    let subject = t |> ServiceType.ToKubernetesString
    subject |> should equal "ClusterIP"

[<Fact>]
let ``Test ServiceType ToKubernetesString LoadBalancer`` () =
    let t = LoadBalancer
    let subject = t |> ServiceType.ToKubernetesString
    subject |> should equal "LoadBalancer"

[<Fact>]
let ``Test Service selector`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let subject = service {
        metadata meta
        spec (serviceSpec {
            selector (ServiceSelector("app", "test"))
            ``type`` NodePort
            port (servicePort {
                name "test"
                porotocol "TCP"
                port 80
                targetPort 80
                nodePort 30007
            })
        })
    }

    subject.ApiVersion |> should equal "v1"
    subject.Kind |> should equal "Service"
    subject.Spec.Type |> should equal "NodePort"
    subject.Spec.Selector |> should haveCount 1
    subject.Spec.Selector["app"] |> should equal "test"
    subject.Spec.Ports |> should haveCount 1
    subject.Spec.Ports[0].Name |> should equal "test"
    subject.Spec.Ports[0].AppProtocol |> should equal "TCP"
    subject.Spec.Ports[0].Port |> should equal 80
    subject.Spec.Ports[0].TargetPort.Value |> should equal "80"
    subject.Spec.Ports[0].NodePort.Value |> should equal 30007