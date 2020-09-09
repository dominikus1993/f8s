namespace FsharpNetes
open k8s.Models
module Pod =
    let createSimple(name: string) = 
        V1Pod()

