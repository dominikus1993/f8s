namespace FSharpNetes

open k8s.Models

type Cpu =
    | M of int

type Memory =
    | K of int
    | M of int
    | G of int
    | Mi of int
    | Gi of int

module Cpu = 
    let toString (m: Cpu) = 
        match m with
        | Cpu.M(v) -> $"{v}m"


module Memory =
    let toString (k: Memory) =
        match k with
        | K(v) ->  $"{v}k"
        | M(v) ->  $"{v}M"
        | G(v) ->  $"{v}G"
        | Mi(v) ->  $"{v}Mi"
        | Gi(v) ->  $"{v}Gi"        
