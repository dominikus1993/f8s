﻿namespace FSharpNetes

open k8s.Models
open k8s

[<AutoOpen>]
module Namespace =
    type NamespaceState = { MetaData: V1ObjectMeta option }
    
    type NamespaceBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; }
        
        member this.Run(state: NamespaceState) = 
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            V1Namespace(metadata = meta, kind="Namespace", apiVersion = "v1")

        [<CustomOperation("metadata")>]
        member this.Name (state: NamespaceState, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }
            
    
    let nmspc = NamespaceBuilder()

    // craete a namespace
    let create (client: Kubernetes)(pod: V1Namespace): V1Namespace =
        client.CreateNamespace(pod)
        