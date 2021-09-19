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

    

    let yaml = myNamespace |> Serialization.toYaml
    printfn $"{yaml}"
    0 // return an integer exit code
