open System.Net.Http

let httpClient = new HttpClient()

let downloadAsync (url: string) =
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

let downloadAll =
    async {
        let result = [
            for url in urls do
                let content = downloadAsync url
                content
        ]
        let! r = result |> Async.Parallel
        return r
   }


let result =
    urls
    |> List.map (fun url -> downloadAsync url)
    |> Async.Parallel
    |> Async.RunSynchronously

result
|> Array.iter (fun x -> printfn "%s" x)
