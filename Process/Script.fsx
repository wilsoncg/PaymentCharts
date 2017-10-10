// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#r "../packages/FSharp.Plotly.1.0.3/lib/net45/FSharp.Plotly.dll"
#r "../packages/SQLProvider.1.1.11/lib/FSharp.Data.SqlProvider.dll"
#r "../packages/FSharp.Configuration.1.3.0/lib/net45/FSharp.Configuration.dll"
#r "System.Data.Linq"

#load "ChartSettings.fs"
#load "Transactions.fs"

open FSharp.Plotly
open System.Web.UI.WebControls

let numDays = ChartSettings.numDays
let dc = ChartSettings.PaymentsDb.GetDataContext(ChartSettings.Settings.ConnectionStrings.PaymentsData, 300)

let daysChart = 
  Transactions.getDaysStacks numDays dc
  |> Seq.map (fun t -> Chart.StackedBar(t.Days, t.Amounts, Name= sprintf "%A" t.Name))
  |> Chart.Combine  
  |> Chart.withTitle (sprintf "Last %i days transactions" numDays)
  |> Chart.withSize (1200,900)
  |> Chart.SaveHtmlAs "last7days" 

let last24hChart = 
  Transactions.getHoursStacks dc
  |> Seq.map (fun t -> Chart.StackedColumn(t.Hours, t.Amounts, Name= sprintf "%A" t.Name))
  |> Chart.Combine  
  |> Chart.withTitle "Last 24 hours transactions"
  |> Chart.withSize (1200,900)
  |> Chart.SaveHtmlAs "last24hours"

//GenericChart.ofTraceObject sampleChart layout
//|> Chart.Show
// Define your library scripting code here

// fsi --exec Script.fsx