module ChartSettings

open FSharp.Configuration
open FSharp.Data.TypeProviders

type Settings = AppSettings<"app.config">
let path = System.IO.Path.Combine [|__SOURCE_DIRECTORY__ ; "bin" ; "debug" ; "Process.dll" |]
Settings.SelectExecutableFile path
let numDays = Settings.NumDays

let [<Literal>] connStringName = "PaymentsData"
let [<Literal>] schemaFile = "FullDbMap.dbml"

type PaymentsDb = 
 SqlDataConnection<
    ConnectionStringName=connStringName,
    LocalSchemaFile=schemaFile,
    ForceUpdate=false,
    Views=false,
    StoredProcedures=false,
    Timeout=300
    >

let dataContext = PaymentsDb.GetDataContext()
dataContext.DataContext.ObjectTrackingEnabled <- false;
dataContext.DataContext.CommandTimeout <- 300
type FxRate = PaymentsDb.ServiceTypes.FxRate

FSharp.Data.Sql.Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O")