module EnvironmentTests

open System
open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes

[<Fact>]
let ``Test one name value env`` () =
    let environment = env {
        add_var (NameValue("xD", "xDDD"))
    }
    match environment with
    | [Choice1Of2(subject)] -> 
        subject.Name |> should equal "xD"
        subject.Value |> should equal "xDDD"
    | _ ->
        Assert.True(false)
