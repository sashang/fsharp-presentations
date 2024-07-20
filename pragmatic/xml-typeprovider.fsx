#r "nuget: FSharp.Data"

open FSharp.Data

type Books = XmlProvider<"books.xml">

let data = Books.Load("books.xml")

for book in data.Books do
    printfn "%s, %s" book.Author book.Title