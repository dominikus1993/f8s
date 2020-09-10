namespace FSharpNetes 
open System

[<AutoOpen>]
module Container = 
    open k8s
    open k8s.Models

    type ContainerState = { Name: string option }

    type ContainerBuilder internal () =
        member this.Yield(_) =
            { Name = None }
        
        member this.Run(state: ContainerState) = 
            let name = defaultArg state.Name (Guid.NewGuid().ToString())
            V1Container(name = name)
            
        [<CustomOperation("name")>]
        member this.Name (state: ContainerState, name: string) =
            { state with Name = Some(name) }

    let container = ContainerBuilder()

