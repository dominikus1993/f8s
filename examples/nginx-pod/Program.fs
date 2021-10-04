open FSharpNetes
open k8s
open k8s.Models

let myNamespace =
    let meta = metadata {
        name "sample"
        labels ([Label("env", "prod"); Label("app", "nginx-example")])
    }
    nmspc {
        metadata meta
    }

[<EntryPoint>]
let main argv =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels ([Label("app", "test"); Label("server", "nginx")])
    }

    let nginxCont = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy (IfNotPresent)
        env ([NameValue("PORT", "8080")])
    }

    

    let yaml = nginxCont |> Serialization.toYaml
    printfn $"{yaml}"
    0 // return an integer exit code
