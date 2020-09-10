namespace FsharpNetes

type Api =
    V1
    
module Api =
    let value api =
        match api with
        | V1 -> "v1"

