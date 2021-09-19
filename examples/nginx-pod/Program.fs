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
    

    let yaml = meta |> Serialization.toYaml
    printfn $"{yaml}"
    0 // return an integer exit code
