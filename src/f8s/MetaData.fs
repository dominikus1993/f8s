namespace FSharpNetes
open k8s.Models

[<AutoOpen>]
module MetaData =
    type Label = Label of name: string * value: string
    type Labels = Label list

    type MetaDataState = { Name: string option; Namespace: string option; Labels: Map<string, string> }
    
    type MetaDataBuilder internal () =
        member _.Yield(_) =
            { Name = None; Namespace = None; Labels = Map.empty }
        
        member _.Run(state: MetaDataState) = 
            let name = defaultArg state.Name null
            let nmspc = defaultArg state.Namespace "default"
            V1ObjectMeta(name = name, namespaceProperty = nmspc, labels = state.Labels)

        [<CustomOperation("name")>]
        member _.Name (state: MetaDataState, name: string) =
            { state with Name = Some(name) }

        [<CustomOperation("nmspc")>]
        member _.Namespace (state: MetaDataState, name: string) =
            { state with Namespace = Some(name) }
        
        [<CustomOperation("label")>]
        member _.Label (state: MetaDataState, label: Label) =
            let (Label(name, value)) = label
            { state with Labels = Map.add name value state.Labels }

        [<CustomOperation("labels")>]
        member this.Labels (state: MetaDataState, labels: Labels) =
            List.fold(fun acc label -> this.Label(acc, label)) state labels

    let metadata = MetaDataBuilder()