// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FSharpNetes
open k8s
open k8s.Models

    // let meta =
    //     metadata {
    //         name "devnews-cli"
    //         nmspc "bots"
    //     }


    // let devNewsPod = 
    //     podSpec {
    //         container devnewsContainer
    //         imagePullSecret "dockerhub"
    //     }

    // let cron =
    //     cronJob {
    //         metadata meta
    //         schedule "0 10 * * 1"
    //         spec devNewsPod
    //     }

    // let yaml = cron |> Serialization.toYaml
    // printfn $"{yaml}"

let meta = metadata {
    name "devnews-cli"
    nmspc "bots"
    labels [Label("app", "devnews-cli");]
}

let devnewsContainer =
    container {
        name "devnews-cli"
        image (Image("dominikus1910/devnewscli", SemVer("v1.3.0")))
        image_pull_policy Always
        env [ NameValue("MicrosoftTeams__Enabled", "false")
              SecretRef("ConnectionStrings__Discord", ValueFrom("devnews", "discord"))
              SecretRef("ConnectionStrings__Articles", ValueFrom("devnews", "articles")) ]
        args (Arg("--article-quantity"))
        args (Arg("10"))
    }

let devNewsCli = cronjob { 
    metadata meta
    concurrencyPolicy Forbid
    failedJobsHistoryLimit 5
    successfulJobsHistoryLimit 5 
    schedule "0 10 * * 1"
}

let yaml = devNewsCli |> Serialization.toYaml
 
printf $"YAML: \n{yaml}"
