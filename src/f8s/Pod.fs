namespace FSharpNetes
open k8s.Models
open k8s

[<AutoOpen>]
module PodSpec = 
    type PodSpecState = { Containers: V1Container list option; SecurityContext: V1PodSecurityContext option; ImagePullSecrets: V1LocalObjectReference list option}

    type PodSpecBuilder internal () =
        member this.Yield(_) =
            { Containers = None; SecurityContext = None; ImagePullSecrets = None }
        
        member this.Run(state: PodSpecState) = 
            let containers = defaultArg state.Containers [] |> Utils.toList
            let security = defaultArg state.SecurityContext null
            let imagePullSecrets = defaultArg (state.ImagePullSecrets |> Option.map(fun x -> x |> Utils.toList)) null
            V1PodSpec(containers = containers, securityContext = security, imagePullSecrets = imagePullSecrets)

        [<CustomOperation("imagePullSecret")>]
        member this.ImagePullSecret (state: PodSpecState, secret: string) =
            this.ImagePullSecrets(state, [secret])

        [<CustomOperation("imagePullSecrets")>]
        member this.ImagePullSecrets (state: PodSpecState, secrets: string list) =
            let s = secrets |> List.map(fun secret -> V1LocalObjectReference(secret))
            match state.ImagePullSecrets with
            | Some(cont) ->
                { state with ImagePullSecrets = Some(cont @ s)}
            | None ->
                { state with ImagePullSecrets = Some(s)}

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
    type PodState = { MetaData: V1ObjectMeta option; Spec: V1PodSpec option; }
    
    type PodBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; Spec = None;}
        
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

        [<CustomOperation("imagePullSecrets")>]
        member this.ImagePullSecret (state: PodState, secret: string) =
            this.ImagePullSecrets(state, [secret])

        [<CustomOperation("containers")>]
        member this.ImagePullSecrets (state: PodState, secrets: string list) =
            let s = secrets |> List.map(fun secret -> V1LocalObjectReference(secret)) |> Utils.toList
            match state.Spec with
            | Some(spec) ->
                spec.ImagePullSecrets <- s 
                { state with Spec = Some(spec)}
            | None ->
                let spec = 
                    podSpec {
                        imagePullSecrets secrets
                    }
                { state with Spec = Some(spec)}
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
            