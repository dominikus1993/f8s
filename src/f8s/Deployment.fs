namespace FSharpNetes
open k8s.Models
open k8s

[<AutoOpen>]
module Deployment =

    type private SelectorState = { MatchLabels: Map<string, string> }

    type Selector = 
        | MatchLabels of key: string * value: string

    type Deploymenttate = { MetaData: V1ObjectMeta option; Pod: V1PodSpec option; Selectors: Selector list option  }
    
    let private mapSelectors (selectors: Selector list) =
        let mapSelector selector 
            match s with
             MatchLabels(k, v) ->
                V1LabelSelector(matchLabels = )

    type DeploymentBuilder internal () =
        member this.Yield(_) =
            { MetaData = None; Pod = None; Selector = None }
        
        member this.Run(state: Deploymenttate) = 
            let meta = defaultArg state.MetaData (V1ObjectMeta())
            let pod = defaultArg state.Pod null
            let selector = state.Selectors |> Option.map(fun s -> )
            let spec = V1DeploymentSpec(selector = V1LabelSelector(), template = V1PodTemplateSpec(spec = pod))
            V1Deployment(metadata = meta, spec = spec)

        [<CustomOperation("metadata")>]
        member this.Name (state: Deploymenttate, meta: V1ObjectMeta) =
            { state with MetaData = Some(meta) }

        [<CustomOperation("pod")>]
        member this.Container (state: Deploymenttate, pod: V1PodSpec) =
            { state with Pod = Some(pod) }

        [<CustomOperation("selector")>]
        member this.AddSelector (state: Deploymenttate, selector: Selector) =
            match state.Selectors with
            | Some(selectors) ->
                { state with Selectors = Some(selectors @ [selector]) }
            | None -> 
                { state with Selectors = Some([selector])}

    
    let deployment = DeploymentBuilder()
            