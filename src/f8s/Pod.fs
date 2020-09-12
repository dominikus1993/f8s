namespace FSharpNetes 
open System

[<AutoOpen>]
module Pod = 
    open k8s
    open k8s.Models

    type PodState = { Name: string option; ImageName: string option; ImagePullPolicy: ImagePullPolicy }

    type PodBuilder internal () =
        member this.Yield(_) =
            { Name = None; ImageName = None; ImagePullPolicy = Always }
        
        member this.Run(state: PodState) = 
            let name = defaultArg state.Name (Guid.NewGuid().ToString())
            let pp = state.ImagePullPolicy.ToKubeValue()
            let imageName = defaultArg state.ImageName null
            V1Container(name = name, imagePullPolicy = pp, image = imageName)
            

    let pod = PodBuilder()

