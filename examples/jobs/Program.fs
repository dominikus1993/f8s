// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FSharpNetes
open k8s
open k8s.Models

[<EntryPoint>]
let main argv =
    let meta =
        metadata {
            name "devnews-cli"
            nmspc "bots"
        }

    let devnewsContainer =
        container {
            name "devnews-cli"
            image (Image("dominikus1910/devnewscli", SemVer("v1.3.0")))
            image_pull_policy Always
            env [ NameValue("MicrosoftTeams__Enabled", "false")
                  SecretRef("ConnectionStrings__Discord", Secret("devnews", "discord"))
                  SecretRef("ConnectionStrings__Articles", Secret("devnews", "articles")) ]
        }

    let cron =
        cronJob {
            metadata meta
            schedule "40 * * * *"
            container devnewsContainer
        }

    let yaml = cron |> Serialization.toYaml
    printfn $"{yaml}"
    0 // return an integer exit code
