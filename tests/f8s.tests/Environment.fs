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

[<Fact>]
let ``Test add name value envs`` () =
    let environment = env {
        add_vars ([NameValue("xD", "xDDD"); NameValue("xDD", "xDDD")])
    }
    match environment with
    | [Choice1Of2(subject1); Choice1Of2(subject2)] -> 
        subject1.Name |> should equal "xD"
        subject1.Value |> should equal "xDDD"
        subject2.Name |> should equal "xDD"
        subject2.Value |> should equal "xDDD"        
    | _ ->
        Assert.True(false)

[<Fact>]
let ``Test one secret ref env`` () =
    let environment = env {
        add_var (SecretRef("xD", Secret("xDDD", "xDD")))
    }
    match environment with
    | [Choice1Of2(subject)] -> 
        subject.Name |> should equal "xD"
        subject.ValueFrom.SecretKeyRef.Key |> should equal "xDD"
        subject.ValueFrom.SecretKeyRef.Name |> should equal "xDDD"
    | _ ->
        Assert.True(false)

[<Fact>]
let ``Test one configMap env`` () =
    let environment = env {
        add_var (ConfigMap("xD"))
    }
    match environment with
    | [Choice2Of2(subject)] -> 
        subject.ConfigMapRef.Name |> should equal "xD"
    | _ ->
        Assert.True(false)
