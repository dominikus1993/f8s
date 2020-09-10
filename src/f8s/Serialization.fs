namespace FSharpNetes

module Serialization =
    open System.Text.Json
    open System.Text.Json.Serialization
    
    let private jsonOptions =
        let options = JsonSerializerOptions()
        options.IgnoreNullValues <- true
        options.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
        options.Converters.Add(JsonFSharpConverter())
        options
    
    let toJson obj =
        JsonSerializer.Serialize(obj, jsonOptions)    
        

