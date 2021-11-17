module EnvTests

open System
open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes
open k8s.Models

[<Fact>]
let ``Test NameValue env`` () =
    let subject = env {
        add_var (NameValue("xD", "test"))
    }
    
    subject |> should haveLength 1
    let envVar = subject[0] 
    match envVar with 
    | Choice1Of2(var) -> 
        var.Name |> should equal "xD"
        var.Value |> should equal "test"
    | _ -> failwith "Should be V1EnvVar"

[<Fact>]
let ``Test ConfigMap env`` () =
    let subject = env {
        add_var (ConfigMap("xD"))
    }
    
    subject |> should haveLength 1
    let envVar = subject[0] 
    match envVar with 
    | Choice2Of2(var) -> 
        var.ConfigMapRef.Name |> should equal "xD"
    | _ -> failwith "Should be ConfigMapRef"

[<Fact>]
let ``Test SecretRef env`` () =
    let subject = env {
        add_var (SecretRef("xD", ValueFrom("xDD", "xDDD")))
    }
    
    subject |> should haveLength 1
    let envVar = subject[0] 
    match envVar with 
    | Choice1Of2(var) -> 
        var.Name |> should equal "xD"
        var.ValueFrom.SecretKeyRef.Name |> should equal "xDD"
        var.ValueFrom.SecretKeyRef.Key |> should equal "xDDD"
    | _ -> failwith "Should be SecretKeyRef"

[<Fact>]
let ``Test FieldRef env`` () =
    let subject = env {
        add_var (FieldRef("xD", FieldPath("xDD")))
    }
    
    subject |> should haveLength 1
    let envVar = subject[0] 
    match envVar with 
    | Choice1Of2(var) -> 
        var.Name |> should equal "xD"
        var.ValueFrom.FieldRef.FieldPath |> should equal "xDD"
    | _ -> failwith "Should be SecretKeyRef"


[<Fact>]
let ``Test Multifield env`` () =
    let subject = env {
        add_var (FieldRef("xD", FieldPath("xDD")))
        add_var (NameValue("xD", "test"))
        add_vars [(NameValue("xD2", "test2")); ConfigMap("xD")]
    }
    
    subject |> should haveLength 4