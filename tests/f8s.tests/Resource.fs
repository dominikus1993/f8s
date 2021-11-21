module ResourceTests
open FSharpNetes
open Xunit
open FsUnit.Xunit


[<Fact>]
let ``Test cpu resources`` () =
    let value = Cpu.M(100)
    let subject = Cpu.toString value

    subject |> should equal "100m"

[<Fact>]
let ``Test memory resources M`` () =
    let value = Memory.M(100)
    let subject = Memory.toString value

    subject |> should equal "100M"

[<Fact>]
let ``Test memory resources K`` () =
    let value = K(100)
    let subject = Memory.toString value

    subject |> should equal "100k"

[<Fact>]
let ``Test memory resources G`` () =
    let value = G(100)
    let subject = Memory.toString value

    subject |> should equal "100G"

[<Fact>]
let ``Test memory resources Gi`` () =
    let value = Gi(100)
    let subject = Memory.toString value

    subject |> should equal "100Gi"

[<Fact>]
let ``Test memory resources Mi`` () =
    let value = Mi(100)
    let subject = Memory.toString value

    subject |> should equal "100Mi"