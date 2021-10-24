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
    let env = nginxCont.Env.[0]
    env.Name |> should equal "PORT"
    env.Value |> should equal "8080"