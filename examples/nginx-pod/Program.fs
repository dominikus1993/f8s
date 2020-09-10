// Learn more about F# at http://fsharp.org

open System
open FSharpNetes


[<EntryPoint>]
let main argv =
    let container = container {
        name "test"
    }
    
    printfn "Hello World from F#! %A" container
    0 // return an integer exit code
