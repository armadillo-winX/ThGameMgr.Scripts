open System
open System.IO

(*
    The decrypt function is ported to F# by Mashiro Tamane, 2026

    Original Code:
    ThScoreFileConverter by IIHOSHI Yoshinori
    https://github.com/y-iihoshi/ThScoreFileConverter/

    BSD 2-clause License

    Copyright (c) 2013, IIHOSHI Yoshinori

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this
      list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and/or other materials provided with the distribution.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*)
let decrypt (input: Stream) (output: Stream) =
    let data: byte[] = Array.zeroCreate<byte> (int input.Length)
    let decryptedData : byte[] = Array.zeroCreate<byte> (int input.Length)
    input.Read(data, 0, (int input.Length)) |> ignore
    let mutable mask : byte = 0uy
    let mutable index = 0
    while index < int input.Length do
        let decryptedByte: byte = data[index] ^^^ mask
        decryptedData[index] <- decryptedByte
        let temp = mask  + decryptedByte
        mask <- byte (temp >>> 5) ||| (temp <<< 3)
        index <- index + 1
    output.Write(decryptedData, 0, int input.Length)
    output.Flush()

// Main Entry as follow
let scriptDirectory = __SOURCE_DIRECTORY__
let binaryDirectory = $"{scriptDirectory}\\bin"

printfn "Th07 score data decoder script"
printfn "Copyright (c) 2026 珠音茉白/東方管制塔開発部"
printfn ""
printfn "Script location is %s" scriptDirectory
printfn ""

if Directory.Exists(binaryDirectory) <> true then Directory.CreateDirectory(binaryDirectory) |> ignore
if File.Exists($"{scriptDirectory}\\scoreConfig.txt") <> true then 
  printfn "scoreConfig.txt does not found." 
  exit 1

let configStreamReader = new StreamReader($"{scriptDirectory}\\scoreConfig.txt")
let th07ScoreFilePath = configStreamReader.ReadLine()
configStreamReader.Dispose()

let input = new FileStream(th07ScoreFilePath, FileMode.Open)
let dectypted = new FileStream($"{binaryDirectory}\\th07scoredecrypted.dat", FileMode.Create)

decrypt input dectypted |> ignore
input.Dispose()
dectypted.Dispose()

printfn "Decoded !"
