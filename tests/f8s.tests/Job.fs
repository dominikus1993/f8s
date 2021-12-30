module JobTests

open Xunit
open k8s
open k8s.Models
open FsUnit.Xunit
open FSharpNetes


[<Fact>]
let ``Test jobSpec`` () =
    let nginxCont =
        container {
            name "nginx"
            image (Image("nginx", Latest))
            image_pull_policy (IfNotPresent)
            command [ "nginx"; "-g"; "daemon off;" ]
            env [ NameValue("PORT", "8080") ]
            ports [ TCP(8080) ]
            request ({ Memory = Mi(512); Cpu = Cpu.M(512) })
            limit ({ Memory = Gi(1); Cpu = Cpu.M(1024) })
        }

    let nginxPod = podSpec {
        container nginxCont
    }
    
    let subject = jobSpec {
        activeDeadlineSeconds 1
        parallelism 21
        suspend true
        pod nginxPod
    }

    subject.ActiveDeadlineSeconds |> should equal 1L
    subject.Parallelism |> should equal 21
    subject.Suspend |> should be True
    subject.Template.Spec.Containers |> should haveCount 1
    let cont = subject.Template.Spec.Containers.[0]
    cont.Name |> should equal "nginx"
    cont.Image |> should equal "nginx:latest"
    cont.ImagePullPolicy |> should equal "IfNotPresent"
    cont.Env |> should haveCount 1
    let env = cont.Env.[0]
    env.Name |> should equal "PORT"
    env.Value |> should equal "8080"
    cont.Ports |> should haveCount 1
    let port = cont.Ports.[0]
    port.Protocol |> should equal "TCP"
    port.ContainerPort |> should equal 8080
    cont.Command |> Seq.toList |> should matchList (["nginx"; "-g"; "daemon off;"])


[<Fact>]
let ``Test cronjobTemplate`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }
    let jobSpec = jobSpec {
        activeDeadlineSeconds 1
        parallelism 21
    }

    let subject = cronjobTemplate {
        metadata meta
        spec jobSpec
    }

    subject.Metadata.Name |> should equal meta.Name
    subject.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    subject.Spec.ActiveDeadlineSeconds |> should equal 1L
    subject.Spec.Parallelism |> should equal 21


[<Fact>]
let ``Test cron job`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }
    let nginxSpec = cronJobSpec {
        metadata meta
        concurrencyPolicy Allow
        suspend true 
        failedJobsHistoryLimit 1
        successfulJobsHistoryLimit 2
        startingDeadlineSeconds 22
        schedule "* * * * *"
    }
    let nginxJob = cronjob { 
        metadata meta
        spec nginxSpec
    }

    nginxJob.Kind |> should equal "CronJob"
    nginxJob.ApiVersion |> should equal "batch/v1"
    nginxJob.Metadata.Name |> should equal meta.Name
    nginxJob.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    nginxJob.Spec.Schedule |> should equal "* * * * *"
    nginxJob.Spec.ConcurrencyPolicy |> should equal "Allow"
    nginxJob.Spec.Suspend |> should be True
    nginxJob.Spec.FailedJobsHistoryLimit |> should equal 1
    nginxJob.Spec.SuccessfulJobsHistoryLimit |> should equal 2
    nginxJob.Spec.StartingDeadlineSeconds |> should equal 22L



[<Fact>]
let ``Test cron job wit spec`` () =
    let meta = metadata {
        name "test"
        nmspc "test"
        labels [Label("app", "test"); Label("server", "nginx")]
    }
    let nginxJob = cronjob { 
        metadata meta
        concurrencyPolicy Allow
        suspend true 
        failedJobsHistoryLimit 1
        successfulJobsHistoryLimit 2
        startingDeadlineSeconds 22
        schedule "* * * * *"
    }

    nginxJob.Kind |> should equal "CronJob"
    nginxJob.ApiVersion |> should equal "batch/v1"
    nginxJob.Metadata.Name |> should equal meta.Name
    nginxJob.Metadata.NamespaceProperty |> should equal meta.NamespaceProperty
    nginxJob.Spec.Schedule |> should equal "* * * * *"
    nginxJob.Spec.ConcurrencyPolicy |> should equal "Allow"
    nginxJob.Spec.Suspend |> should be True
    nginxJob.Spec.FailedJobsHistoryLimit |> should equal 1
    nginxJob.Spec.SuccessfulJobsHistoryLimit |> should equal 2
    nginxJob.Spec.StartingDeadlineSeconds |> should equal 22L

