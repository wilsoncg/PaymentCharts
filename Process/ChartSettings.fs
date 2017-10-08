module ChartSettings

open FSharp.Configuration

type Settings = AppSettings<"app.config">
let path = System.IO.Path.Combine [|__SOURCE_DIRECTORY__ ; "bin" ; "debug" ; "Process.dll" |]
Settings.SelectExecutableFile path
let numDays = Settings.NumDays