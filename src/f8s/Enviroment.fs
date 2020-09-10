namespace FSharpNetes

[<AutoOpen>]
module Env =
    type EnviromentVar =
        | KeyValue of name: string * value: string
        
    type EnvState = {  }
