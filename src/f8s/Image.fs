namespace FSharpNetes
open Semver

type Version =
    | Latest
    | SemVer of string
    | Hex of string
with
    member internal this.GetVersion() =
        match this with
        | Latest -> "latest"
        | SemVer(v) -> v
        | Hex(v) -> v
    
type Image = Image of name: string * version : Version
    
module Image =
    let create(name: string, version: Version) = Image(name, version)
    
    let imageName (image: Image) =
        let (Image(name, ver)) = image
        $"%s{name}:%s{ver.GetVersion()}"