// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#r "../packages/FSharp.Plotly.1.0.2/lib/net40/FSharp.Plotly.dll"
#r "../packages/SQLProvider.1.1.11/lib/FSharp.Data.SqlProvider.dll"
#r "../packages/FSharp.Configuration.1.3.0/lib/net45/FSharp.Configuration.dll"
#r "System.Data.Linq"


#load "ChartSettings.fs"
#load "Transactions.fs"

open FSharp.Plotly
open System.Web.UI.WebControls

let numDays = ChartSettings.numDays
let dc = ChartSettings.PaymentsDb.GetDataContext(ChartSettings.Settings.ConnectionStrings.PaymentsData, 300)

let layout =
    Layout()
    
let stacks =
 Transactions.getStacks numDays dc
 |> Seq.map (fun t ->
        Chart.StackedBar(t.Days, t.Amounts, Name= sprintf "%A" t.Name))

let chart = 
  stacks
  |> Chart.Combine  
  |> Chart.withLayout(Options.Layout(Title="Last 7 days transactions"))
  |> Chart.Show

//GenericChart.ofTraceObject sampleChart layout
//|> Chart.Show
// Define your library scripting code here

// fsi --exec Script.fsx