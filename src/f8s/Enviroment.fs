namespace FSharpNetes
open k8s.Models

[<AutoOpen>]
module Environment =
    type EnvironmentVariable =
        | NameValue of name: string * value: string
        | ConfigMap of name: string
        | Secret of name: string
    type EnvironmentState = { Variables: EnvironmentVariable list }
    
    type EnvironmentBuilder internal () =
        member this.Yield(_) =
            { Variables = []; }
        
        member this.Run(state: EnvironmentState) = 
            state.Variables |> List.map(fun e -> match e with | NameValue(name, value) -> Choice1Of2(V1EnvVar(name, value)))
            

        [<CustomOperation("add_var")>]
        member this.AddVar (state: EnvironmentState, variable: EnvironmentVariable) =
            { state with Variables = variable :: state.Variables }    
    
    let env = EnvironmentBuilder()