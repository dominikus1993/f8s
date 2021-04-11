// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FSharpNetes
open k8s
open k8s.Models

[<EntryPoint>]
let main argv =
    let meta = metadata {
        name "test"
        label (Label("test", "test"))
    }
    
    let nspc = nmspc {
        metadata meta
    }
    
    0 // return an integer exit code