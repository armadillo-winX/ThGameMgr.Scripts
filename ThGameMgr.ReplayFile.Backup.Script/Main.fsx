#load Version.fsx
#load Configurator.fsx
#load ReplayFileBackup.fsx

open System
open System.IO

// The Main Entry as follow
let scriptDirectory = __SOURCE_DIRECTORY__
let thGameNameConfigFilePath = Path.Combine(scriptDirectory, "ThGameNameConfig.json")
let binaryDirectoryPath = Path.Combine(scriptDirectory, "bin")
let tempDirectoryPath = Path.Combine(scriptDirectory, "temp")

printfn "==============================================================================="
printfn "%s" Version.name
printfn "version.%s" Version.verison
printfn "%s" Version.copyright
printfn "==============================================================================="

if File.Exists(thGameNameConfigFilePath) <> true then 
    printfn "'ThGameNameConfig.json' が見つかりません．スクリプトを終了します．"
    printfn "何かキーを押して続行..."
    Console.ReadKey() |> ignore
    exit 1

if Directory.Exists(binaryDirectoryPath) <> true then
    Directory.CreateDirectory(binaryDirectoryPath) |> ignore

if Directory.Exists(tempDirectoryPath) <> true then
    Directory.CreateDirectory(tempDirectoryPath) |> ignore

let gameNameDictionary = Configurator.loadThGameNameConfig thGameNameConfigFilePath

printfn "Game ID を入力:"
let input = Console.ReadLine()
let result, gameName = gameNameDictionary.TryGetValue(input)
if result = true then
    let gameId = input
    printfn "リプレイファイルのパスを入力:"
    let replayFilePath = Console.ReadLine()
    printfn "バックアップの名前を入力:"
    let backupName = Console.ReadLine()
    printfn "コメントを入力:"
    let comment = Console.ReadLine()
    let timestamp  = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")
    
    let replayBackupInfo: ReplayFileBackup.ReplayFileBackupInfo = {
        GameId = gameId
        GameName = gameName
        SourceReplayFilePath = replayFilePath
        BackupName = backupName
        Timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        Comment = comment
    }
    ReplayFileBackup.makeReplayBackupFile timestamp replayBackupInfo tempDirectoryPath binaryDirectoryPath
else
    printfn "入力が不正です．"
    exit -1
