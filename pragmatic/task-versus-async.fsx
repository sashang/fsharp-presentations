open System
open System.Threading
open System.Threading.Tasks

let sleepAsync (x: int) = async {
    printfn "sleeping for %d seconds" x
    do! Async.Sleep (x*1000)
}

async {
    let t = sleepAsync 3
    let x = sleepAsync 2
    let! t = t
    let! x = x
    return ()
} |> Async.RunSynchronously

[sleepAsync 3; sleepAsync 4] |> Async.Parallel |> Async.RunSynchronously

let sleepTask (x: int) = task {
    printfn "sleeping for %d seconds in task" x
    do! Async.Sleep (x*1000)
}

task {
    let t = sleepTask 3
    let x = sleepTask 2
    let! t = t
    let! x = x
    return ()
} |> Async.AwaitTask |> Async.RunSynchronously