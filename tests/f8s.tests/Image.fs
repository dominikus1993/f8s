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
let ``Test image with semVer tag`` () =
    let image = Image("xD", SemVer("1.0.0"))
    let subject = image |> Image.imageName
    subject |> should equal "xD:1.0.0"

[<Fact>]
let ``Test image with custom tag`` () =
    let image = Image("xD", Custom("xdddd"))
    let subject = image |> Image.imageName
    subject |> should equal "xD:xdddd"

[<Fact>]
let ``Test image with inorrect semVer tag`` () =
    let image = Image("xD", SemVer("1.0.ad"))
    Assert.ThrowsAny<ArgumentException>(fun () ->  image |> Image.imageName |> ignore)