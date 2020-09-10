namespace FSharpNetes

open Newtonsoft.Json

module Serialization =
    open System.Text.Json
    open System.Text.Json.Serialization
    
    let toJson obj =
        obj |> JsonConvert.SerializeObject
       
        
        

