namespace FSharpNetes
open Semver

type ImagePullPolicy =
    | Always
    | IfNotPresent
    | Never
module ImagePullPolicy = 
    let toKubeValue policy =
        match policy with
        | Always -> "Always"
        | IfNotPresent -> "IfNotPresent"
        | Never -> "Never"

type Version =
    | Latest
    | SemVer of string
    | Custom of string
with
    member internal this.GetVersion() =
        match this with
        | Latest -> "latest"
        | SemVer(v) -> SemVersion.Parse(v).ToString()
        | Custom(v) -> v
    
type Image = Image of name: string * version : Version
    
module Image =
    let create(name: string, version: Version) = Image(name, version)
    
    let imageName (image: Image) =
        let (Image(name, ver)) = image
        $"%s{name}:%s{ver.GetVersion()}"