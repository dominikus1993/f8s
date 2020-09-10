namespace FsharpNetes
open k8s.Models
open k8s
open System.Text.Json
open System.Text.Json.Serialization
module Yaml = 
    let private jsonOptions =
        let options = JsonSerializerOptions()
        options.IgnoreNullValues <- true
        options.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
        options.Converters.Add(JsonFSharpConverter())
        options
    
    let private serialize obj =
        JsonSerializer.Serialize(obj, jsonOptions)    
        
    let print(pod: V1Pod) = 
        pod |> serialize
        
module Pod =
    let createSimple(name: string) = 
        let metaData = V1ObjectMeta(Name = name)
        V1Pod(metadata = metaData)

