module ChartSettings

open FSharp.Data.Sql

let [<Literal>] connStringName = "PaymentsData"
let [<Literal>] connString = "Data Source=(localdb)\ProjectsV13;Initial Catalog=PaymentsData;Integrated Security=SSPI;"
let [<Literal>] schemaFile = "FullDbMap.dbml"

type PaymentsDb = SqlDataProvider<ConnectionString=connString>
let dataContext = PaymentsDb.GetDataContext()
FSharp.Data.Sql.Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O")