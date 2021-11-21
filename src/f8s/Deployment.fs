namespace FSharpNetes
open k8s.Models
open k8s

[<AutoOpen>]
module Deployment =

    type private SelectorState = { MatchLabels: Map<string, string> }
    
    type Selector = 
        | MatchLabels of key: string * value: string

    type Deploymenttate = { MetaData: V1ObjectMeta option; Pod: V1PodSpec option; Selectors: Selector list option; Replicas: int option; }
    
    let private mapSelectors (selectors: Selector list) =
        let mapSelector s state =
            match s with
             MatchLabels(k, v) ->
                { state with MatchLabels = state.MatchLabels |> Map.add k v}
        let res = selectors |> List.fold (fun acc x -> mapSelector x acc) ({ MatchLabels = Map.empty})
        V1LabelSelector(matchLabels = res.MatchLabels)

    type DeploymentBuilder internal () =
        member _.Yield(_) =
            { MetaData = None; Pod = None; Selectors = None; Replicas = None;}
        
        member _.Run(state: Deploymenttate) = 
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            let pod = defaultArg state.Pod null
            let selector = defaultArg (state.Selectors |> Option.map(mapSelectors)) null
            let replicas = defaultArg state.Replicas 1
            let spec = V1DeploymentSpec(selector = selector, replicas = replicas, template = V1PodTemplateSpec(spec = pod))
            V1Deployment(metadata = meta, spec = spec, kind = "Deployment", apiVersion = "apps/v1")

        [<CustomOperation("metadata")>]
        member _.Name (state: Deploymenttate, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }

        [<CustomOperation("pod")>]
        member _.Container (state: Deploymenttate, pod: V1Pod) =
            { state with Pod = Some(pod.Spec) }

        [<CustomOperation("replicas")>]
        member _.Replicas (state: Deploymenttate, replicas: int) =
            { state with Replicas = Some(replicas) }

        [<CustomOperation("selector")>]
        member _.AddSelector (state: Deploymenttate, selector: Selector) =
            match state.Selectors with
            | Some(selectors) ->
                { state with Selectors = Some(selectors @ [selector]) }
            | None -> 
                { state with Selectors = Some([selector])}

    
    let deployment = DeploymentBuilder()
            