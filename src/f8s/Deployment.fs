namespace FSharpNetes
open k8s.Models
open k8s

[<AutoOpen>]
module Deployment =
    
    type Deploymenttate = { MetaData: V1ObjectMeta option; Containers: V1Container list option }
    
    type DeploymentBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; Containers = None }
        
        member this.Run(state: Deploymenttate) = 
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            let containers = defaultArg state.Containers [] |> Utils.toList
            
            let spec = V1DeploymentSpec(selector = V1LabelSelector(),template = V1PodTemplateSpec(spec = V1PodSpec(containers = containers)))
            V1Deployment(metadata = meta, spec = spec)

        [<CustomOperation("metadata")>]
        member this.Name (state: Deploymenttate, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }

        [<CustomOperation("container")>]
        member this.Container (state: Deploymenttate, containers: V1Container) =
            this.Containers(state, [containers])

        [<CustomOperation("containers")>]
        member this.Containers (state: Deploymenttate, containers: V1Container list) =
            match state.Containers with
            | Some(cont) ->
                { state with Containers = Some(cont @ containers)}
            | None ->
                { state with Containers = Some(containers)}

    
    let deployment = DeploymentBuilder()
            