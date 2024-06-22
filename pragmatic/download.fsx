open System
open System.Net.Http

let httpClient = new HttpClient()

let donwloadAsync (url: string) =
    async {
        let! response = httpClient.GetAsync(url) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content
    }

let urls = [
    "https://www.google.com";
    "https://www.bing.com";
    "https://www.yahoo.com";
]

let result =
    urls
    |> List.map (fun url -> donwloadAsync url)
    |> Async.Parallel
    |> Async.RunSynchronously

result
|> Array.iter (fun x -> printfn "%s" x)