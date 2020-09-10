// Learn more about F# at http://fsharp.org

open System
open FSharpNetes


[<EntryPoint>]
let main argv =
    let container = container {
        name "nginx"
        image_name "nginx:latest"
        image_pull_policy ImagePullPolicy.Always
    }
    
    
    printfn "Hello World from F#! %A" (container |> Serialization.toJson)
    0 // return an integer exit code
