[<AutoOpen>]
module Domain

open FSharp.Compiler.SyntaxTree
open Newtonsoft.Json.Linq
open Newtonsoft.Json
open YamlDotNet.Serialization.NamingConventions

[<RequireQualifiedAccess>]
type EmptyDefinitionResolution =
    | Ignore
    | GenerateFreeForm

/// Describes the compilation target
[<RequireQualifiedAccess>]
type Target =
    | FSharp
    | FSharpThoth
    | Fable

/// Describes the async return type of the functions of the generated clients
[<RequireQualifiedAccess>]
type AsyncReturnType =
    | Async
    | Task

[<RequireQualifiedAccess>]
type FactoryFunction =
    | Create
    | None

type NormalizedCodegenConfig =
    { schema: string
      output: string
      target: Target
      project : string
      asyncReturnType: AsyncReturnType
      synchronous: bool
      resolveReferences: bool
      emptyDefinitions: EmptyDefinitionResolution
      overrideSchema: JToken option
      filterTags: string list
      odataSchema: bool }

type CodegenConfig =
    { schema: string
      output: string
      target: Target
      project : string
      asyncReturnType: AsyncReturnType
      synchronous: bool
      resolveReferences: bool option
      emptyDefinitions: EmptyDefinitionResolution option
      overrideSchema: JToken option
      filterTags: string list option
      odataSchema: bool option }

    static member deserializeJson (json: string) = JsonConvert.DeserializeObject<CodegenConfig>(json)
    static member deserializeYaml (yaml: string) =
        let deserializer =
            YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build()
        deserializer.Deserialize(yaml)
        |> fun x -> printfn "%A" x; x
        |> unbox

    static member normalize config : NormalizedCodegenConfig =
        { schema = config.schema
          output = config.output
          target = config.target
          project = config.project
          asyncReturnType = config.asyncReturnType
          synchronous = config.synchronous
          resolveReferences = config.resolveReferences |> Option.defaultValue false
          emptyDefinitions = config.emptyDefinitions |> Option.defaultValue EmptyDefinitionResolution.Ignore
          overrideSchema = config.overrideSchema
          filterTags = config.filterTags |> Option.defaultValue []
          odataSchema = config.odataSchema |> Option.defaultValue false }


type OperationParameter = {
    parameterName: string
    parameterIdent: string
    required: bool
    parameterType: SynType
    docs : string
    location: string
    style: string
    properties: string list
}