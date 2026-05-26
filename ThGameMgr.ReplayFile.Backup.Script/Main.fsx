#load Version.fsx
#load Configurator.fsx
#load ReplayFileBackup.fsx

open System
open System.Collections.Generic
open System.IO

let makeReplayBackup (gameNameDictionary: Dictionary<string, string>) (tempDirectoryPath: string) (binaryDirectoryPath: string) =
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
        let replayBackupFileName  = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")

        let replayBackupInfo: ReplayFileBackup.ReplayFileBackupInfo = {
            GameId = gameId
            GameName = gameName
            SourceReplayFilePath = replayFilePath
            BackupName = backupName
            Timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            Comment = comment
            ApplicationName = Version.name
        }
        ReplayFileBackup.makeReplayBackupFile replayBackupFileName replayBackupInfo tempDirectoryPath binaryDirectoryPath
    else
        printfn "入力が不正です．"
        exit -1

let getReplayBackupInfo (binaryDirectoryPath: string) =
    let backupFiles = Directory.GetFiles(binaryDirectoryPath, "*.trpb", SearchOption.TopDirectoryOnly)
    if backupFiles.Length > 0 then
        for backupFile in backupFiles do
            let replayBackupInfo = ReplayFileBackup.getReplayBackupFileInfo backupFile
            printfn "Game ID: %s" replayBackupInfo.GameId
            printfn "ゲーム名: %s" replayBackupInfo.GameName
            printfn "バックアップ作成元: %s" replayBackupInfo.SourceReplayFilePath
            printfn "バックアップの名前: %s" replayBackupInfo.BackupName
            printfn "タイムスタンプ: %s" replayBackupInfo.Timestamp
            printfn "コメント: %s" replayBackupInfo.Comment
            printfn "アプリケーション名: %s" replayBackupInfo.ApplicationName
    else
        printfn "バックアップファイルがありません"

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

while true do
    printfn "操作を選択:"
    printfn "[0] リプレイバックアップファイルをつくる"
    printfn "[1] リプレイバックアップファイルの情報を取得"
    printfn "[x] スクリプトを終了する"

    let operationInput = Console.ReadLine()
    match operationInput with
    | "0" -> makeReplayBackup gameNameDictionary tempDirectoryPath binaryDirectoryPath
    | "1" -> getReplayBackupInfo binaryDirectoryPath
    | "x" -> exit 0
    |_-> printfn "入力が不正です．"
