module Process

open System
open System.Data
open System.Data.Linq
open FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq

type Class1() = 
    member this.X = "F#"

type internal DB = SqlDataConnection<"Data Source=(localdb)\MSSqlLocalDB;Initial Catalog=PaymentsData;Integrated Security=SSPI;">
 let private lt = DB.GetDataContext().LedgerTransaction