namespace FSharpNetes
open k8s.Models
open k8s

[<AutoOpen>]
module PodSpec = 
    type PodSpecState = { Containers: V1Container list option; SecurityContext: V1PodSecurityContext option}

    type PodSpecBuilder internal () =
        member this.Yield(_) =
            { Containers = None; SecurityContext = None }
        
        member this.Run(state: PodSpecState) = 
            let containers = defaultArg state.Containers [] |> Utils.toList
            let security = defaultArg state.SecurityContext null
            V1PodSpec(containers = containers, securityContext = security)

        [<CustomOperation("container")>]
        member this.Container (state: PodSpecState, containers: V1Container) =
            this.Containers(state, [containers])

        [<CustomOperation("containers")>]
        member this.Containers (state: PodSpecState, containers: V1Container list) =
            match state.Containers with
            | Some(cont) ->
                { state with Containers = Some(cont @ containers)}
            | None ->
                { state with Containers = Some(containers)}

    
    let podSpec = PodSpecBuilder()
[<AutoOpen>]
module Pod =
    type PodState = { MetaData: V1ObjectMeta option; Spec: V1PodSpec option}
    
    type PodBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; Spec = None }
        
        member this.Run(state: PodState) = 
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            let spec = defaultArg state.Spec null
            V1Pod(metadata = meta, spec = spec, apiVersion = "v1", kind = "Pod")

        [<CustomOperation("metadata")>]
        member this.Name (state: PodState, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }

        [<CustomOperation("container")>]
        member this.Container (state: PodState, containers: V1Container) =
            this.Containers(state, [containers])

        [<CustomOperation("spec")>]
        member this.Spec (state: PodState, spec: V1PodSpec) =
            { state with Spec = (spec |> Option.ofObj)}

        [<CustomOperation("containers")>]
        member this.Containers (state: PodState, containers: V1Container list) =
            match state.Spec with
            | Some(spec) ->
                for cont in containers do
                    spec.Containers.Add(cont)
                { state with Spec = Some(spec)}
            | None ->
                let conts = containers |> Utils.toList
                { state with Spec = Some(V1PodSpec(containers = conts))}

    
    let pod = PodBuilder()
            