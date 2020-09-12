open FSharpNetes

[<EntryPoint>]
let main argv =
    let meta = metadata {
        name "test"
    }
    
    let nspc = nmspc {
        metadata meta
    }
    
    let enviroment = env {
        add_var (NameValue("Test", "22312"))
        add_var (NameValue("Test2", "22312"))
        add_var (Secret("Test3"))
        add_var (ConfigMap("Test4"))
    }
    
    let container = container {
        name "nginx"
        image (Image("nginx", Latest))
        image_pull_policy Always
        env enviroment
    }
    
  
    printfn "Hello World from F#! %A" (container |> Serialization.toJson)
    0 // return an integer exit code
