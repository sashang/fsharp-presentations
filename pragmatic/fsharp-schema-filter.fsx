#r "nuget: Swashbuckle.AspNetCore"
open Swashbuckle.AspNetCore.SwaggerGen
open System
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