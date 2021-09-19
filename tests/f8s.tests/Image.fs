module ImageTests

open System
open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes

[<Fact>]
let ``Test image with latest tag`` () =
    let image = Image("xD", Latest)
    let subject = image |> Image.imageName
    subject |> should equal "xD:latest"


[<Fact>]
let ``Test image with semVet tag`` () =
    let image = Image("xD", SemVer("v1.0.0"))
    let subject = image |> Image.imageName
    subject |> should equal "xD:v1.0.0"