#load "./Microsoft.AspNetCore.App-8.0.7.fsx"
#r "nuget: FSharp.SystemTextJson"
#r "nuget: Swashbuckle.AspNetCore"

open System
module Domain =
    type Manager =
        { Name: string
          Title: string
          Subordinates: int }

    type Accountant =
        { Name: string
          Title: string
          Companies: string list}

    type Employee =
        | Manager of Manager
        | Accountant of Accountant


module Contract =
    type EmployeeType =
        | ETUndefined = 0
        | ETManager = 1
        | ETAccountant = 2

    type ManagerRec =
        { Name: string
          Title: string
          Subordinates: int }

    type AccountantRec =
        { Name: string
          Title: string
          Companies: string list }

    type Employee  =
        { Type: EmployeeType
          Accountant: AccountantRec option
          Manager: ManagerRec option}

module DomainToContract =
    let mapEmployeeType (employee: Domain.Employee) =
        match employee with
        | Domain.Manager _ -> Contract.EmployeeType.ETManager
        | Domain.Accountant _ -> Contract.EmployeeType.ETAccountant

    let mapAccountant (employee: Domain.Employee) =
        match employee with
        | Domain.Accountant accountant ->
            Some { Contract.AccountantRec.Name = accountant.Name
                   Contract.AccountantRec.Title = accountant.Title
                   Contract.AccountantRec.Companies = accountant.Companies }
        | _ ->
            None

    let mapManager (employee: Domain.Employee) =
        match employee with
        | Domain.Manager m ->
           Some { Contract.ManagerRec.Name = m.Name
                  Contract.ManagerRec.Title = m.Title
                  Contract.ManagerRec.Subordinates = m.Subordinates }
        | _ -> None


    let mapEmployee employee =
        { Contract.Type = mapEmployeeType employee
          Contract.Accountant = mapAccountant employee
          Contract.Manager = mapManager employee }

let manager =
    { Domain.Name = "Elon"
      Domain.Title = "10xCEO"
      Domain.Subordinates = 100000 }

let accountant =
    { Domain.Accountant.Companies = ["Tesla"; "SpaceX"]
      Domain.Accountant.Name = "Jane"
      Domain.Accountant.Title = "Chief Accountant" }

let employee1 = Domain.Manager manager
let employee2 = Domain.Accountant accountant

let dtoEmployee1 = DomainToContract.mapEmployee employee1
let dtoEmployee2 = DomainToContract.mapEmployee employee2

open System.Text.Json
open System.Text.Json.Serialization

JsonSerializer.Serialize(dtoEmployee1)

let customOptions =
    JsonFSharpOptions.Default()
        .WithSkippableOptionFields()

JsonSerializer.Serialize(dtoEmployee1, customOptions.ToJsonSerializerOptions())

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection

let builder = WebApplication.CreateBuilder(Array.empty)
builder.Services.AddControllers().AddJsonOptions(fun o -> customOptions.AddToJsonSerializerOptions(o.JsonSerializerOptions))
builder.Services.AddEndpointsApiExplorer()
builder.Services.AddSwaggerGen()
let app = builder.Build()
app.MapSwagger()
app.MapGet("/{name}", Func<string,_>(fun name ->
    if name = "elon" then
        dtoEmployee1
    elif name = "jane" then
        dtoEmployee2
    else
        { Contract.Employee.Type = Contract.EmployeeType.ETUndefined
          Accountant = None
          Manager = None })
    ) |> ignore
app.UseSwagger()
app.UseSwaggerUI()
app.Run()

open Swashbuckle.AspNetCore.SwaggerGen
open Microsoft.OpenApi.Models

type FSharpOptionSchemaFilter() =
    interface ISchemaFilter with
        member this.Apply(schema: OpenApiSchema, context: SchemaFilterContext) =
            let isOption = fun (propType: Type) ->
                propType.IsGenericType && propType.GetGenericTypeDefinition() = typedefof<option<_>>

            for prop in context.Type.GetProperties() do
                if isOption(prop.PropertyType) then
                    let propName = prop.Name
                    match schema.Properties.TryGetValue(propName) with
                    | true, propSchema ->
                        propSchema.Nullable <- true
                    | _ -> ()

builder.Services.AddSwaggerGen(
    fun c ->
        c.SchemaFilter<FSharpOptionSchemaFilter>()
)
let appWithSchemaFilter = builder.Build()
appWithSchemaFilter.MapSwagger()
appWithSchemaFilter.MapGet("/{name}", Func<string,_>(fun name ->
    if name = "elon" then
        dtoEmployee1
    elif name = "jane" then
        dtoEmployee2
    else
        {Contract.Employee.Type =  Contract.EmployeeType.ETUndefined; Accountant = None; Manager = None })
    ) |> ignore

appWithSchemaFilter.UseSwagger()
appWithSchemaFilter.UseSwaggerUI()
appWithSchemaFilter.Run()