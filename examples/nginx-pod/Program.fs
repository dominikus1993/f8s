open FSharpNetes
open k8s
open k8s.Models

[<EntryPoint>]
let main argv =
    let meta = metadata {
        name "test"
    }
    
    let nspc = nmspc {
        metadata meta
    }
    
    let enviroment = env {
        add_var (NameValue("Test", "22312"))
        add_var (NameValue("Test2", "22312"))
    }
    
    let container = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy Always
        env enviroment
    }
    
    let podMeta =  metadata {
        name "nginx-pod"
        nmspc "test"
    }
    
    let nginxPod = pod {
        api V1
        metadata podMeta
        add_container container
    }
    let config = KubernetesClientConfiguration.BuildConfigFromConfigFile()
    let k8s = new Kubernetes(config)
    let a = Namespace.create k8s nspc |> Async.RunSynchronously
    Pod.create(k8s)(nginxPod) |> Async.RunSynchronously
    printfn "Hello World from F#! %A" (nginxPod |> Serialization.toYaml)
    0 // return an integer exit code
