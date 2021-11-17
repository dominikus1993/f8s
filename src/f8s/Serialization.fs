namespace FSharpNetes

open Newtonsoft.Json
open k8s

module Serialization =
    
    let toJson obj =
        obj |> JsonConvert.SerializeObject
        
    let toYaml obj =
        obj |> Yaml.SaveToString
       
        
        

