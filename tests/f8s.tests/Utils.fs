module UtilsTests

open System
open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes.Utils

[<Fact>]
let ``Test to list`` () =
    let list = [1;2;3;4]
    let subject = list |> toList
    subject |> should haveCount 4
   