module ChartSettings

open FSharp.Configuration
open FSharp.Data.Sql

type Settings = AppSettings<"app.config">
let path = System.IO.Path.Combine [|__SOURCE_DIRECTORY__ ; "bin" ; "debug" ; "Process.dll" |]
Settings.SelectExecutableFile path
let numDays = Settings.NumDays
let filePath = Settings.FilePath

let [<Literal>] connStringName = "PaymentsData"
let [<Literal>] dbVendor = Common.DatabaseProviderTypes.MSSQLSERVER
let [<Literal>] schemaFile = "FullDbMap.dbml"

type PaymentsDb = 
 SqlDataProvider<
    DatabaseVendor=dbVendor,
    ConnectionStringName=connStringName,
    TableNames="FxRate,LedgerTransaction">

FSharp.Data.Sql.Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O")