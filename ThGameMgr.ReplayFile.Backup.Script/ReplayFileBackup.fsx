open System
open System.IO
open System.IO.Compression
open System.Xml.Serialization

[<CLIMutable>]
[<XmlRoot("ReplayFileBackupInfo")>]
type ReplayFileBackupInfo = {
    [<XmlElement("GameId")>] GameId: string
    [<XmlElement("GameName")>] GameName: string
    [<XmlElement("SourceReplayFilePath")>] SourceReplayFilePath: string
    [<XmlElement("BackupName")>] BackupName: string
    [<XmlElement("Timestamp")>] Timestamp: string
    [<XmlElement("Comment")>] Comment: string
    [<XmlElement("ApplicationName")>] ApplicationName: string
}

let makeReplayFileBackupInfoFile (replayBackupInfo: ReplayFileBackupInfo) (baseDirectory: string) =
    let serializer = XmlSerializer(typeof<ReplayFileBackupInfo>)
    let replayBackupFilePath = Path.Combine(baseDirectory, "ReplayFileBackupInfo.xml")
    let stream = new FileStream(replayBackupFilePath, FileMode.Create)
    serializer.Serialize(stream, replayBackupInfo)
    stream.Dispose()

let makeReplayBackupFile (replayBackupFileName: string) (replayBackupInfo: ReplayFileBackupInfo) (tempDirectory: string) (outputDirectory: string) =
    let backupTempDirectory = Path.Combine(tempDirectory, replayBackupFileName)
    Directory.CreateDirectory(backupTempDirectory) |> ignore
    Path.Combine(backupTempDirectory, "rpy") |> Directory.CreateDirectory |> ignore
    let replayFilepath = replayBackupInfo.SourceReplayFilePath
    let destReplayFilePath =  Path.Combine(backupTempDirectory, "rpy", Path.GetFileName(replayFilepath))
    File.Copy(replayFilepath, destReplayFilePath)
    makeReplayFileBackupInfoFile replayBackupInfo backupTempDirectory
    let outputFilepath = Path.Combine(outputDirectory, $"{replayBackupFileName}.trpb")
    ZipFile.CreateFromDirectory(backupTempDirectory, outputFilepath)

let getReplayBackupFileInfo (replayBackupFilePath: string) =
    let rootEntry = Path.GetFileNameWithoutExtension(replayBackupFilePath)
    let archive = ZipFile.OpenRead(replayBackupFilePath)
    let infoFileEntry = archive.GetEntry($"ReplayFileBackupInfo.xml")
    let stream = infoFileEntry.Open()
    let serializer = new XmlSerializer(typeof<ReplayFileBackupInfo>)
    serializer.Deserialize(stream) :?> ReplayFileBackupInfo

let extractBackupFile (replayBackupFilePath: string) (outputDirectory: string) =
    let replayBackupInfo = getReplayBackupFileInfo(replayBackupFilePath)
    let archive  = ZipFile.OpenRead(replayBackupFilePath)
    let replayFileEntry = $"rpy/{Path.GetFileName(replayBackupInfo.SourceReplayFilePath)}" |> archive.GetEntry
    let outputFilePath = Path.Combine(outputDirectory, Path.GetFileName(replayBackupFilePath))
    replayFileEntry.ExtractToFile(outputFilePath, true)
