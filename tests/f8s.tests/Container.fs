module ContainerTests
open FSharpNetes
open Xunit
open FsUnit.Xunit
open k8s.Models

[<Fact>]
let ``Test container`` () =
    let nginxCont = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy (IfNotPresent)
        command ["nginx"; "-g"; "daemon off;"]
        env [NameValue("PORT", "8080")]
        ports [TCP(8080)]
    }   

    nginxCont.Name |> should equal "nginx"
    nginxCont.Image |> should equal "nginx:latest"
    nginxCont.ImagePullPolicy |> should equal "IfNotPresent"
    nginxCont.Env |> should haveCount 1
    let env = nginxCont.Env[0]
    env.Name |> should equal "PORT"
    env.Value |> should equal "8080"
    nginxCont.Ports |> should haveCount 1
    let port = nginxCont.Ports[0]
    port.Protocol |> should equal "TCP"
    port.ContainerPort |> should equal 8080
    nginxCont.Command |> Seq.toList |> should matchList (["nginx"; "-g"; "daemon off;"])


[<Fact>]
let ``Test container with resources`` () =
    let nginxCont = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy (IfNotPresent)
        command ["nginx"; "-g"; "daemon off;"]
        env [NameValue("PORT", "8080")]
        ports [TCP(8080)]
        request ({ Memory = Mi(512); Cpu = Cpu.M(512) })
        limit ({ Memory = Gi(1); Cpu = Cpu.M(1024) })
    }   

    nginxCont.Resources.Limits |> should haveCount 2
    nginxCont.Resources.Requests |> should haveCount 2
    nginxCont.Resources.Requests["cpu"].Value |> should equal "512m"
    nginxCont.Resources.Requests["memory"].Value |> should equal "512Mi"
    nginxCont.Resources.Limits["cpu"].Value |> should equal "1024m"
    nginxCont.Resources.Limits["memory"].Value |> should equal "1Gi"
    nginxCont.Name |> should equal "nginx"
    nginxCont.Image |> should equal "nginx:latest"
    nginxCont.ImagePullPolicy |> should equal "IfNotPresent"
    nginxCont.Env |> should haveCount 1
    let env = nginxCont.Env[0]
    env.Name |> should equal "PORT"
    env.Value |> should equal "8080"
    nginxCont.Ports |> should haveCount 1
    let port = nginxCont.Ports[0]
    port.Protocol |> should equal "TCP"
    port.ContainerPort |> should equal 8080
    nginxCont.Command |> Seq.toList |> should matchList (["nginx"; "-g"; "daemon off;"])


[<Fact>]
let ``Test container with liveness probe`` () =
    let nginxCont = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy (IfNotPresent)
        command ["nginx"; "-g"; "daemon off;"]
        env [NameValue("PORT", "8080")]
        ports [TCP(8080)]
        request ({ Memory = Mi(512); Cpu = Cpu.M(512) })
        limit ({ Memory = Gi(1); Cpu = Cpu.M(1024) })
        livenessProbe ({ PeriodSeconds = Some(1); Probe = HttpGet({| headers = None; host = None; path = Some("/healthz"); port = 8080 |})})
    }   

    nginxCont.Resources.Limits |> should haveCount 2
    nginxCont.Resources.Requests |> should haveCount 2
    nginxCont.Resources.Requests["cpu"].Value |> should equal "512m"
    nginxCont.Resources.Requests["memory"].Value |> should equal "512Mi"
    nginxCont.Resources.Limits["cpu"].Value |> should equal "1024m"
    nginxCont.Resources.Limits["memory"].Value |> should equal "1Gi"
    nginxCont.Name |> should equal "nginx"
    nginxCont.Image |> should equal "nginx:latest"
    nginxCont.ImagePullPolicy |> should equal "IfNotPresent"
    nginxCont.Env |> should haveCount 1
    let env = nginxCont.Env[0]
    env.Name |> should equal "PORT"
    env.Value |> should equal "8080"
    nginxCont.Ports |> should haveCount 1
    let port = nginxCont.Ports[0]
    port.Protocol |> should equal "TCP"
    port.ContainerPort |> should equal 8080
    nginxCont.Command |> Seq.toList |> should matchList (["nginx"; "-g"; "daemon off;"])

    nginxCont.LivenessProbe.HttpGet.Host |> should be null
    nginxCont.LivenessProbe.HttpGet.Port.Value |> should equal "8080"
    nginxCont.LivenessProbe.HttpGet.HttpHeaders |> should be null
    nginxCont.LivenessProbe.HttpGet.Path |> should equal "/healthz"


let ``Test container with readinessProbe`` () =
    let nginxCont = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy (IfNotPresent)
        command ["nginx"; "-g"; "daemon off;"]
        env [NameValue("PORT", "8080")]
        ports [TCP(8080)]
        request ({ Memory = Mi(512); Cpu = Cpu.M(512) })
        limit ({ Memory = Gi(1); Cpu = Cpu.M(1024) })
        readinessProbe ({ PeriodSeconds = Some(1); Probe = HttpGet({| headers = None; host = None; path = Some("/healthz"); port = 8080 |})})
    }   

    nginxCont.Resources.Limits |> should haveCount 2
    nginxCont.Resources.Requests |> should haveCount 2
    nginxCont.Resources.Requests["cpu"].Value |> should equal "512m"
    nginxCont.Resources.Requests["memory"].Value |> should equal "512Mi"
    nginxCont.Resources.Limits["cpu"].Value |> should equal "1024m"
    nginxCont.Resources.Limits["memory"].Value |> should equal "1Gi"
    nginxCont.Name |> should equal "nginx"
    nginxCont.Image |> should equal "nginx:latest"
    nginxCont.ImagePullPolicy |> should equal "IfNotPresent"
    nginxCont.Env |> should haveCount 1
    let env = nginxCont.Env[0]
    env.Name |> should equal "PORT"
    env.Value |> should equal "8080"
    nginxCont.Ports |> should haveCount 1
    let port = nginxCont.Ports[0]
    port.Protocol |> should equal "TCP"
    port.ContainerPort |> should equal 8080
    nginxCont.Command |> Seq.toList |> should matchList (["nginx"; "-g"; "daemon off;"])

    nginxCont.ReadinessProbe.HttpGet.Host |> should be null
    nginxCont.ReadinessProbe.HttpGet.Port.Value |> should equal "8080"
    nginxCont.ReadinessProbe.HttpGet.HttpHeaders |> should be null
    nginxCont.ReadinessProbe.HttpGet.Path |> should equal "/healthz"