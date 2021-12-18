namespace FSharpNetes

open k8s.Models


[<AutoOpen>]
module Secret =
    open System.Text
    open System.Collections.Generic

    type KubernetesSecretType = | Opaque | TLS | DockerConfigJson | DockerConfig | BasicAuth | SshAuth | Token | ServiceAccount with 
        static member ToKubernetesString(t: KubernetesSecretType) =
            match t with 
            | Opaque -> "Opaque"
            | TLS -> "kubernetes.io/tls"
            | DockerConfigJson -> "kubernetes.io/dockerconfigjson"
            | DockerConfig -> "kubernetes.io/dockercfg"
            | SshAuth -> "kubernetes.io/ssh-auth"
            | Token -> "bootstrap.kubernetes.io/token"
            | ServiceAccount -> "kubernetes.io/service-account-token"
            | BasicAuth -> "kubernetes.io/basic-auth"

    type SecretState = V1Secret

    type SecretBuilder internal () =
        member _.Yield(_) =
            V1Secret(apiVersion = "v1", kind = "Secret")

        member _.Run(state: SecretState) = state

        [<CustomOperation("metadata")>]
        member _.Name(state: SecretState, meta: V1ObjectMeta) =
            state.Metadata <- meta
            state

        [<CustomOperation("type")>]
        member _.Type(state: SecretState, secret: KubernetesSecretType) =
            state.Type <- secret |> KubernetesSecretType.ToKubernetesString
            state

        [<CustomOperation("data")>]
        member _.Data(state: SecretState, data: Map<string, string>) =
            if isNull state.Data then state.Data <- Dictionary<string, byte array>()
            for d in data do
                state.Data.Add(d.Key, Encoding.UTF8.GetBytes(d.Value))
            state

        [<CustomOperation("stringData")>]
        member _.StringData(state: SecretState, data: Map<string, string>) =
            if isNull state.StringData then state.StringData <- Dictionary<string, string>()
            for d in data do
                state.StringData.Add(d.Key, d.Value)
            state

    let secret = SecretBuilder()
