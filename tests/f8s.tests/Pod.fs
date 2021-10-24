module PodTests
open FSharpNetes
open Xunit
open FsUnit.Xunit


[<Fact>]
let ``Test image with latest tag`` () =
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
    nginxPod.Metadata.Name |> should equal meta.Name