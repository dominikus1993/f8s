// Learn more about F# at http://fsharp.org
open FSharpNetes
open FsharpNetes


[<EntryPoint>]
let main argv =
    let meta = metadata {
        name "test"
    }
    
    let nspc = nmspc {
        metadata meta
    }
    
    let container = container {
        name "nginx"
        image_name "nginx:latest"
        image_pull_policy Always
    }
    
  
    printfn "Hello World from F#! %A" (container |> Serialization.toJson)
    0 // return an integer exit code
