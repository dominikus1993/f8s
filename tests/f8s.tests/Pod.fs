module PodTests
open FSharpNetes
open Xunit
open FsUnit.Xunit


[<Fact>]
let ``Test pod with spec`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let nginxCont = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy (IfNotPresent)
        command ["nginx"; "-g"; "daemon off;"]
        env [NameValue("PORT", "8080")]
        ports [TCP(8080)]
    }   

    let nginxPodSpec = podSpec {
        container nginxCont
    }

    let nginxPod = pod {
        metadata meta
        spec nginxPodSpec
    }

    nginxPod.ApiVersion |> should equal "v1"
    nginxPod.Kind |> should equal "Pod"
    nginxPod.Metadata.Name |> should equal meta.Name
    nginxPod.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    nginxPod.Spec.Containers |> should haveCount 1
    let cont = nginxPod.Spec.Containers.[0]
    cont.Name |> should equal "nginx"
    cont.Image |> should equal "nginx:latest"
    cont.ImagePullPolicy |> should equal "IfNotPresent"
    cont.Env |> should haveCount 1
    let env = cont.Env.[0]
    env.Name |> should equal "PORT"
    env.Value |> should equal "8080"
    cont.Ports |> should haveCount 1
    let port = cont.Ports.[0]
    port.Protocol |> should equal "TCP"
    port.ContainerPort |> should equal 8080
    cont.Command |> Seq.toList |> should matchList (["nginx"; "-g"; "daemon off;"])



[<Fact>]
let ``Test pod`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let nginxCont = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy (IfNotPresent)
        command ["nginx"; "-g"; "daemon off;"]
        env [NameValue("PORT", "8080")]
        ports [TCP(8080)]
    }   

    let nginxPod = pod {
        metadata meta
        container nginxCont
    }

    nginxPod.ApiVersion |> should equal "v1"
    nginxPod.Kind |> should equal "Pod"
    nginxPod.Metadata.Name |> should equal meta.Name
    nginxPod.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    nginxPod.Spec.Containers |> should haveCount 1
    let cont = nginxPod.Spec.Containers.[0]
    cont.Name |> should equal "nginx"
    cont.Image |> should equal "nginx:latest"
    cont.ImagePullPolicy |> should equal "IfNotPresent"
    cont.Env |> should haveCount 1
    let env = cont.Env.[0]
    env.Name |> should equal "PORT"
    env.Value |> should equal "8080"
    cont.Ports |> should haveCount 1
    let port = cont.Ports.[0]
    port.Protocol |> should equal "TCP"
    port.ContainerPort |> should equal 8080
    cont.Command |> Seq.toList |> should matchList (["nginx"; "-g"; "daemon off;"])


[<Fact>]
let ``Test pod with image pull secret`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let nginxCont = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy (IfNotPresent)
        command ["nginx"; "-g"; "daemon off;"]
        env [NameValue("PORT", "8080")]
        ports [TCP(8080)]
    }   

    let nginxPodSpec = podSpec {
        container nginxCont
        imagePullSecret "test"
    }

    let nginxPod = pod {
        metadata meta
        spec nginxPodSpec
    }

    nginxPod.ApiVersion |> should equal "v1"
    nginxPod.Kind |> should equal "Pod"
    nginxPod.Metadata.Name |> should equal meta.Name
    nginxPod.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    nginxPod.Spec.Containers |> should haveCount 1
    nginxPod.Spec.ImagePullSecrets |> should haveCount 1
    nginxPod.Spec.ImagePullSecrets[0].Name |> should equal "test" 
    let cont = nginxPod.Spec.Containers.[0]
    cont.Name |> should equal "nginx"
    cont.Image |> should equal "nginx:latest"
    cont.ImagePullPolicy |> should equal "IfNotPresent"
    cont.Env |> should haveCount 1
    let env = cont.Env.[0]
    env.Name |> should equal "PORT"
    env.Value |> should equal "8080"
    cont.Ports |> should haveCount 1
    let port = cont.Ports.[0]
    port.Protocol |> should equal "TCP"
    port.ContainerPort |> should equal 8080
    cont.Command |> Seq.toList |> should matchList (["nginx"; "-g"; "daemon off;"])
    



[<Fact>]
let ``Test pod 2`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }

    let nginxCont = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy (IfNotPresent)
        command ["nginx"; "-g"; "daemon off;"]
        env [NameValue("PORT", "8080")]
        ports [TCP(8080)]
    }   

    let nginxPod = pod {
        metadata meta
        container nginxCont
    }

    nginxPod.ApiVersion |> should equal "v1"
    nginxPod.Kind |> should equal "Pod"
    nginxPod.Metadata.Name |> should equal meta.Name
    nginxPod.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    nginxPod.Spec.Containers |> should haveCount 1
    let cont = nginxPod.Spec.Containers.[0]
    cont.Name |> should equal "nginx"
    cont.Image |> should equal "nginx:latest"
    cont.ImagePullPolicy |> should equal "IfNotPresent"
    cont.Env |> should haveCount 1
    let env = cont.Env.[0]
    env.Name |> should equal "PORT"
    env.Value |> should equal "8080"
    cont.Ports |> should haveCount 1
    let port = cont.Ports.[0]
    port.Protocol |> should equal "TCP"
    port.ContainerPort |> should equal 8080
    cont.Command |> Seq.toList |> should matchList (["nginx"; "-g"; "daemon off;"])