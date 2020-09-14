namespace FSharpNetes

open Newtonsoft.Json
open k8s

module Serialization =
    open System.Text.Json
    open System.Text.Json.Serialization
    
    let toJson obj =
        obj |> JsonConvert.SerializeObject
        
    let toYaml obj =
        obj |> Yaml.SaveToString
       
        
        

