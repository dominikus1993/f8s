namespace FSharpNetes 
open System

[<AutoOpen>]
module Pod = 
    open k8s
    open k8s.Models

    type PodState = { Name: string option; ImageName: string option; ImagePullPolicy: ImagePullPolicy }

    type PodBuilder internal () =
        member this.Yield(_) =
            { Name = None; ImageName = None; ImagePullPolicy = ImagePullPolicy.Always }
        
        member this.Run(state: ContainerState) = 
            let name = defaultArg state.Name (Guid.NewGuid().ToString())
            let pp = state.ImagePullPolicy.ToKubeValue()
            let imageName = defaultArg state.ImageName null
            V1Container(name = name, imagePullPolicy = pp, image = imageName)
            
        [<CustomOperation("name")>]
        member this.Name (state: ContainerState, name: string) =
            { state with Name = Some(name) }
            
        [<CustomOperation("image_name")>]
        member this.ImageName (state: ContainerState, name: string) =
            { state with ImageName = Some(name) }

        [<CustomOperation("image_pull_policy")>]
        member this.ImagePullPolicy (state: ContainerState, policy: ImagePullPolicy) =
            { state with ImagePullPolicy = policy }

    let pod = PodBuilder()

