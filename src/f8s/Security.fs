namespace FSharpNetes

[<AutoOpen>]
module ContainerSecurityContext =
    open k8s
    open k8s.Models

    type ContainerSecurityContextState = { Capabilities: V1Capabilities option }

    type Capabilities = 
    | Add of string list
    | Drop of string list 

    let private mapCapabilities (c: V1Capabilities) (cap: Capabilities): V1Capabilities =
        match cap with 
        | Add(add) -> 
            if c.Add |> isNull then c.Add <- ResizeArray() 
            for a in add do
                c.Add.Add(a)
        | Drop(drop) -> 
            if c.Drop |> isNull then c.Drop <- ResizeArray() 
            for d in drop do
                c.Drop.Add(d)
        c           
        
    type ContainerSecurityContextBuilder internal () =
        member this.Yield(_) =
            { Capabilities = None }
        
        member this.Run(state: ContainerSecurityContextState) = 
            let capabilites = defaultArg state.Capabilities null
            V1SecurityContext(capabilities = capabilites)

        [<CustomOperation("capabilities")>]
        member _.Name (state: ContainerSecurityContextState, c: Capabilities) =
            match state.Capabilities with
            | Some(cap) -> 
                let res = mapCapabilities cap c
                { state with Capabilities = Some(res)}
            | None -> 
                let res = mapCapabilities (V1Capabilities()) c
                { state with Capabilities = Some(res)}
    

    let securityContext = ContainerSecurityContextBuilder()