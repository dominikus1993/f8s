namespace FSharpNetes
open k8s.Models

[<AutoOpen>]
module MetaData =
    type MetaDataState = { Name: string option; Namespace: string option }
    
    type MetaDataBuilder internal () =
        member this.Yield(_) =
            { Name = None; Namespace = None }
        
        member this.Run(state: MetaDataState) = 
            let name = defaultArg state.Name null
            let nmspc = defaultArg state.Namespace null
            V1ObjectMeta(name = name, namespaceProperty = nmspc)

        [<CustomOperation("name")>]
        member this.Name (state: MetaDataState, name: string) =
            { state with Name = Some(name) }

        [<CustomOperation("nmspc")>]
        member this.Namespace (state: MetaDataState, name: string) =
            { state with Namespace = Some(name) }
            
    let metadata = MetaDataBuilder()