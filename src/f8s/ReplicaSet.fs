namespace FSharpNetes

open k8s.Models
open System

[<AutoOpen>]
module ReplicaSet =
    type ReplicaSetState = { Replicas: int option; Api: Api option }
    
    type ReplicaSetBuilder internal () =
        member this.Yield(_) =
            { Replicas = None; Api = None }
        
        member this.Run(state: ReplicaSetState) =
            let replicas = defaultArg state.Replicas 1
            let api = defaultArg state.Api V1 |> Api.value
            let spec = V1ReplicaSetSpec(null, replicas = Nullable<int>(replicas))
            V1ReplicaSet(apiVersion = api, kind = "ReplicaSet", spec = spec)