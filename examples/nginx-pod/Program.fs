// Learn more about F# at http://fsharp.org

open System
open FsharpNetes
[<EntryPoint>]
let main argv =
    let pod = Pod.createSimple "test"
    printfn "Hello World from F#! %A" pod
    0 // return an integer exit code
