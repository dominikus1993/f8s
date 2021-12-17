module SecurityTests
open FSharpNetes
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Test security context when add capabilities`` () =
    let subject = securityContext {
        capabilities (Add(["NET_ADMIN"; "SYS_TIME"]))
    }
    
    subject.Capabilities.Add |> should haveCount 2
    subject.Capabilities.Add |> should contain "NET_ADMIN" 
    subject.Capabilities.Add |> should contain "SYS_TIME" 
    subject.Capabilities.Drop |> should be Null

[<Fact>]
let ``Test security context when drop capabilities`` () =
    let subject = securityContext {
        capabilities (Drop(["NET_ADMIN"; "SYS_TIME"]))
    }
    
    subject.Capabilities.Drop |> should haveCount 2
    subject.Capabilities.Drop |> should contain "NET_ADMIN" 
    subject.Capabilities.Drop |> should contain "SYS_TIME" 
    subject.Capabilities.Add |> should be Null

[<Fact>]
let ``Test security context when drop and add capabilities`` () =
    let subject = securityContext {
        capabilities (Drop(["NET_ADMIN"; "SYS_TIME"]))
        capabilities (Add(["NET_ADMIN"; "SYS_TIME"]))
    }
    
    subject.Capabilities.Drop |> should haveCount 2
    subject.Capabilities.Drop |> should contain "NET_ADMIN" 
    subject.Capabilities.Drop |> should contain "SYS_TIME" 
    subject.Capabilities.Add |> should haveCount 2
    subject.Capabilities.Add |> should contain "NET_ADMIN" 
    subject.Capabilities.Add |> should contain "SYS_TIME" 