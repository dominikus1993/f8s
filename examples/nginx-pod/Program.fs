open FSharpNetes
open k8s
open k8s.Models

[<EntryPoint>]
let main argv =
    let meta = metadata {
        name "test"
        nmspc "test"
        label (Label("test", "test"))
    }
    
    let ngix = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy Always
        env [NameValue("Test", "22312"); NameValue("Test2", "22312")]
    }
    
    
    let nginxPod = pod {
        metadata meta
        container ngix
    }

    let yaml = nginxPod |> Serialization.toYaml
    printfn $"{yaml}"
    0 // return an integer exit code
