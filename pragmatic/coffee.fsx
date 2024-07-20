open System

type Roast =
    | Light
    | Medium
    | Dark

type Style =
    | LongBlack
    | FlatWhite
    | Espresso
    | Latte
    | Cappuccino
    | Macchiato
    | Mocha

type Size =
    | Small
    | Medium
    | Large

type Coffee =
    { Roast: Roast
      Style: Style
      Size: Size }

type Order =
    { Coffee: Coffee
      Name: string
      Payment: decimal }

type ValidOrder =
    { Coffee: Coffee
      Name: string
      Payment: decimal }

type InvalidOrder =
    { Coffee: Coffee
      Name: string
      Payment: decimal }

type ValidateOrderFunc = Order -> Result<ValidOrder, InvalidOrder>

type CalcPriceFunc = Coffee -> decimal

let roastPriceTable = [(Light, 1.0m); (Roast.Medium, 1.1m); (Dark, 1.2m)] |> Map.ofList
let sizePriceTable = [(Small, 1.0m);(Medium, 1.1m); (Large, 1.2m)] |> Map.ofList
let stylePriceTable =
    [(Espresso, 1.0m); (LongBlack, 1.0m);(FlatWhite, 2.0m);
     (Latte, 2m); (Cappuccino, 2m); (Macchiato, 2m); (Mocha, 2.5m)] |> Map.ofList

let calcPrice : CalcPriceFunc =
    fun coffee ->
        roastPriceTable[coffee.Roast] + sizePriceTable[coffee.Size] + stylePriceTable[coffee.Style]

let validateOrder: ValidateOrderFunc =
    fun order ->
        if order.Payment < calcPrice order.Coffee then
            Error(
                { InvalidOrder.Coffee = order.Coffee
                  Name = order.Name
                  Payment = order.Payment }
            )
        else
            Ok(
                { ValidOrder.Coffee = order.Coffee
                  Name = order.Name
                  Payment = order.Payment }
            )

let order1 =
    { Order.Coffee =
        { Coffee.Roast = Dark
          Style = LongBlack
          Size = Small }
      Payment = 5.0m
      Name = "Sashan" }

let order2 =
    { Order.Coffee =
        { Coffee.Roast = Dark
          Style = LongBlack
          Size = Small }
      Payment = 1.0m
      Name = "Peter" }


let result1 = validateOrder order1
let result2 = validateOrder order2

match result2 with
| Ok order -> printfn "Making coffee"
| Error order -> printfn "Give more money"