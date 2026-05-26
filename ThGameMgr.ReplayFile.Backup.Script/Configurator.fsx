open System
open System.Collections.Generic
open System.IO
open System.Text.Encodings.Web
open System.Text.Json
open System.Text.Unicode

let loadThGameNameConfig (configFilePath: string) =
    let config = File.ReadAllText(configFilePath)
    let serializerOption = new JsonSerializerOptions()
    serializerOption.Encoder <- JavaScriptEncoder.Create(UnicodeRanges.All)
    JsonSerializer.Deserialize<Dictionary<string, string>>(config, serializerOption)
