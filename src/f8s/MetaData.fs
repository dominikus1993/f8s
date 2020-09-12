namespace FSharpNetes

open k8s.Models

[<AutoOpen>]
module MetaData =
    type MetaDataState = { Name: string option }
    
    type MetaDataBuilder internal () =
        member this.Yield(_) =
            { Name = None; }
        
        member this.Run(state: MetaDataState) = 
            let name = defaultArg state.Name null
            V1ObjectMeta(name = name)

        [<CustomOperation("name")>]
        member this.Name (state: MetaDataState, name: string) =
            { state with Name = Some(name) }
            
    let metadata = MetaDataBuilder()