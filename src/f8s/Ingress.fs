namespace FSharpNetes
open k8s.Models
open k8s

[<AutoOpen>]
module IngressSpec =
    type IngressService = { Name: string; Port: int }

    type Path = { Path: string; PathType: string; Backend: IngressService}
    type IngressSpecBuilder internal () =
        member this.Yield(_) =
            V1IngressSpec()
        
        member this.Run(state: V1IngressSpec) = state

        [<CustomOperation("ingressClassName")>]
        member this.Metadata (state: V1IngressSpec, ingressClassName: string) =
            state.IngressClassName <- ingressClassName
    
    let ingressSpec = IngressSpecBuilder()

[<AutoOpen>]
module Ingress =
    type IngressBuilder internal () =
        member this.Yield(_) =
            V1Ingress(apiVersion = "networking.k8s.io/v1", kind = "Ingress")
        
        member this.Run(state: V1Ingress) = state

        [<CustomOperation("apiVersion")>]
        member this.ApiVersion (state: V1Ingress, apiVersion: string) =
            state.ApiVersion <- apiVersion
            state

        [<CustomOperation("metadata")>]
        member this.Metadata (state: V1Ingress, meta: V1ObjectMeta) =
            state.Metadata <- meta
            state
    
    let ingress = IngressBuilder()
            