open System.Net.Http

let httpClient = new HttpClient()

let downloadAsync (url: string) =
    async {
        let! response = httpClient.GetAsync(url) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content
    }

let urls = [
    "https://jsonplaceholder.typicode.com/posts/1"
    "https://jsonplaceholder.typicode.com/posts/2"
    "https://jsonplaceholder.typicode.com/posts/3"
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
