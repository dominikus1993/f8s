namespace FSharpNetes

type Version =
    | Latest
    | SemVer of string
with
    member this.GetVersion() =
        match this with
        | Latest -> "latest"
        | SemVer(v) -> v
    
type Image = { Name: string; Version: Version }
    
module Image =
    let create(name: string, version: Version) = { Name = name; Version = version }
    
    let imageName (image: Image) = sprintf "%s:%s" image.Name (image.Version.GetVersion())