// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
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
   
    let cron =
        cronJob {
            metadata meta
            schedule "40 * * * *"
        }
    let yaml = cron |> Serialization.toYaml
    printfn $"{yaml}"    
    0 // return an integer exit code