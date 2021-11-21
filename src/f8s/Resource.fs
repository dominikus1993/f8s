namespace FSharpNetes

open k8s.Models

type Cpu = M of int

type Memory =
    | K of int
    | M of int
    | G of int
    | Mi of int
    | Gi of int

type Resource = Resource of memory: Memory * cpu: Cpu

module Cpu =
    let toResourceQuantity (m: Cpu) : ResourceQuantity =
        match m with
        | Cpu.M (v) -> ResourceQuantity($"{v}m")


module Memory =
    let toResourceQuantity (k: Memory) : ResourceQuantity =
        let res =
            match k with
            | K (v) -> $"{v}k"
            | M (v) -> $"{v}M"
            | G (v) -> $"{v}G"
            | Mi (v) -> $"{v}Mi"
            | Gi (v) -> $"{v}Gi"

        ResourceQuantity(res)

module Resource =
    open System.Collections.Generic

    let convertToK8s (res: Resource) : IDictionary<string, ResourceQuantity> =
        let (Resource (memory, cpu)) = res

        let res =
            [ ("cpu", cpu |> Cpu.toResourceQuantity)
              ("memory", memory |> Memory.toResourceQuantity) ]

        Map.ofList res
