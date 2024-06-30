module Config

open System.IO

let readConfig file =
    try
        match File.Exists file with
        | false -> Error $"Hawaii configuration file {file} was not found"
        | true ->
            let content = File.ReadAllText(file)
            let rawConfig =
                match file |> Path.GetExtension with
                | ".json" -> CodegenConfig.deserializeJson(content)
                | ".yaml"
                | ".yml" ->
                    printfn "%A" content
                    CodegenConfig.deserializeYaml(content)
                | _ -> failwith "Unsupported configuration file format"

            let configParent = Path.GetDirectoryName file

            { rawConfig with
                schema = Path.getAbsolutePath configParent rawConfig.schema
                output = Path.getAbsolutePath configParent rawConfig.output }
            |> CodegenConfig.normalize
            |> Ok
    with
    | error ->
        Error $"Error ocurred while reading the configuration file: {error.Message}"
