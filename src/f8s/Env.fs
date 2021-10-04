namespace FSharpNetes

open k8s.Models

[<AutoOpen>]
module Environment =
    type FieldPath = FieldPath of string
    type ValueFrom = ValueFrom of name: string * key: string 
    type EnvironmentVariable =
        | NameValue of name: string * value: string
        | ConfigMap of name: string
        | SecretRef of name: string * valueFrom: ValueFrom
        | FieldRef of name: string * fieldPath: FieldPath
        
    type EnvironmentVariables = EnvironmentVariable list
    type EnvironmentState = { Variables: EnvironmentVariable list }

    let internal mapConfig (env: EnvironmentVariable) =
        match env with
        | NameValue(name, value) -> Choice1Of2(V1EnvVar(name, value))
        | ConfigMap(name) -> Choice2Of2(V1EnvFromSource(configMapRef = V1ConfigMapEnvSource(name)))
        | SecretRef(envName, ValueFrom(name, key)) -> Choice1Of2(V1EnvVar(envName, valueFrom = V1EnvVarSource(secretKeyRef = V1SecretKeySelector(key, name))))
        | FieldRef(envName, FieldPath(fieldPath)) -> Choice1Of2(V1EnvVar(envName, valueFrom = V1EnvVarSource(fieldRef = V1ObjectFieldSelector(fieldPath))))
        
    type EnvironmentBuilder internal () =
        member this.Yield(_) =
            { Variables = []; }
        
        member this.Run(state: EnvironmentState) = 
            state.Variables |> List.map(mapConfig)
            
        [<CustomOperation("add_var")>]
        member this.AddVar (state: EnvironmentState, variable: EnvironmentVariable) =
            { state with Variables = variable :: state.Variables }    
        [<CustomOperation("add_vars")>]
        member this.AddVars (state: EnvironmentState, variables: EnvironmentVariables) =
            { state with Variables = variables @ state.Variables }  
    let env = EnvironmentBuilder()