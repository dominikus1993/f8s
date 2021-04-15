module ApiTests

open System
open Xunit
open k8s
open FsUnit.Xunit
open FSharpNetes

[<Fact>]
let ``Test api V1`` () =
    let v1 = V1
    let subject = v1 |> Api.value
    subject |> should equal "v1"
    Assert.True(true)
