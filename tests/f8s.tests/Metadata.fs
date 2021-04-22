module MetaDataTests

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