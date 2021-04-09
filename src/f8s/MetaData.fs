namespace FSharpNetes
open k8s.Models

[<AutoOpen>]
module MetaData =
    type Label = Label of name: string * value: string

    type MetaDataState = { Name: string option; Namespace: string option; Labels: Map<string, string> }
    
    type MetaDataBuilder internal () =
        member this.Yield(_) =
            { Name = None; Namespace = None; Labels = Map.empty }
        
        member this.Run(state: MetaDataState) = 
            let name = defaultArg state.Name null
            let nmspc = defaultArg state.Namespace "default"
            V1ObjectMeta(name = name, namespaceProperty = nmspc, labels = state.Labels)

        [<CustomOperation("name")>]
        member this.Name (state: MetaDataState, name: string) =
            { state with Name = Some(name) }

        [<CustomOperation("nmspc")>]
        member this.Namespace (state: MetaDataState, name: string) =
            { state with Namespace = Some(name) }
        
        [<CustomOperation("label")>]
        member this.Label (state: MetaDataState, label: Label) =
            let (Label(name, value)) = label
            { state with Labels = Map.add name value state.Labels }

            
    let metadata = MetaDataBuilder()