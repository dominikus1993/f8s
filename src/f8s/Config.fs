namespace FSharpNetes

open System
open System.Linq
open FSharpNetes.Utils
open k8s
open k8s.Models

[<AutoOpen>]
module ConfigMap = 

    type ConfigMapBuilder internal() =
        member this.Yield(_) =
            V1ConfigMap(kind="ConfigMap", apiVersion = "v1")
        
        member this.Run(state: V1ConfigMap) = 
            state

        [<CustomOperation("metadata")>]
        member _.Metadata (state: V1ConfigMap, meta: V1ObjectMeta) =
            state.Metadata <- meta
            state

        [<CustomOperation("data")>]
        member _.Data (state: V1ConfigMap, meta: Map<string, string>) =
            state.Data <- meta
            state

        [<CustomOperation("immutable")>]
        member _.Immutable (state: V1ConfigMap, immutable: bool) =
            state.Immutable <- immutable
            state
    
    let configMap = ConfigMapBuilder()