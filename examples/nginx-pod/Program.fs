open FSharpNetes
open k8s
open k8s.Models

let myNamespace =
    let meta =
        metadata {
            name "sample"

            labels (
                [ Label("env", "prod")
                  Label("app", "nginx-example") ]
            )
        }

    nmspc { metadata meta }

let meta =
    metadata {
        name "test"
        nmspc "test"

        labels [ Label("app", "test")
                 Label("server", "nginx") ]
    }

let nginxCont =
    container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy (IfNotPresent)
        command [ "nginx"; "-g"; "daemon off;" ]
        env [ NameValue("PORT", "8080") ]
        ports [ TCP(8080) ]
        request ({ Memory = Mi(512); Cpu = Cpu.M(512) })
        limit ({ Memory = Gi(1); Cpu = Cpu.M(1024) })
    }

let nginxPod =
    pod {
        metadata meta
        container nginxCont
    }

let nginxDeployemt =
    deployment {
        metadata meta
        replicas 2
        selector (MatchLabels("app", "test"))
        pod nginxPod
    }

let yaml = nginxDeployemt |> Serialization.toYaml
printfn $"Deployment: \n{yaml}"


let secret = secret {
    metadata meta
    data (Map[("test", "eEREREREMjEzNw==")])
}

let secretYaml = secret |> Serialization.toYaml

printfn $"Sample Secret: \n{secretYaml}"