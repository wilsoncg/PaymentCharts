// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#r "../packages/XPlot.Plotly.1.4.5/lib/net45/XPlot.Plotly.dll"
#r "../packages/SQLProvider.1.1.42/lib/net451/FSharp.Data.SqlProvider.dll"
#r "../packages/FSharp.Configuration.1.3.0/lib/net45/FSharp.Configuration.dll"
#r "../packages/HtmlAgilityPack.1.6.0/lib/Net45/HtmlAgilityPack.dll"
#r "../packages/Newtonsoft.Json.10.0.3/lib/net45/Newtonsoft.Json.dll"
#r "../packages/FSharp.Data.TypeProviders.5.0.0.2/lib/net40/FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq"

#load "ChartSettings.fs"
#load "Transactions.fs"
#load "CustomChartExtensions.fs"

open XPlot.Plotly
open CustomChartExtensions
open System.Web.UI.WebControls

let numDays = ChartSettings.numDays
let dc = ChartSettings.PaymentsDb.GetDataContext()
let date = System.DateTime.UtcNow.ToString("dd/MM/yy")

let daysChart = 
  let stacks = Transactions.getDaysStacks numDays dc
  if Seq.isEmpty stacks then stacks |> ignore 
  else
  stacks
  |> Seq.map (fun t -> 
    Bar(
     x = t.Days, 
     y = t.Amounts, 
     name= sprintf "%A" t.Name, 
     marker = Marker(color = t.Colour)))
  |> Chart.Plot  
  |> Chart.WithLayout (
    Layout(
        barmode="relative", 
        title= sprintf "Last %i days transactions %s" numDays date))
  |> Chart.WithSize (1200,900)
  |> CustomSaveHtmlAs "last31days"

let last24hChart = 
  let stacks = Transactions.getHoursStacks dc
  if Seq.isEmpty stacks then stacks |> ignore 
  else
    stacks
    |> Seq.map (fun t -> 
    Bar(
     x = t.Hours, 
     y = t.Amounts, 
     name= sprintf "%A" t.Name, 
     marker = Marker(color = t.Colour)))
    |> Chart.Plot  
    |> Chart.WithLayout (
     Layout(
        barmode="relative", 
        title= sprintf "Last 24 hours transactions %s" date))
    |> Chart.WithSize (1200,900)
    |> CustomSaveHtmlAs "last24hours"

let last6monthsChart = 
  let stacks = Transactions.getDaysStacks 180 dc
  if Seq.isEmpty stacks then stacks |> ignore 
  else
    stacks
    |> Seq.map (fun t -> 
    Bar(
     x = t.Days, 
     y = t.Amounts, 
     name= sprintf "%A" t.Name, 
     marker = Marker(color = t.Colour)))
    |> Chart.Plot  
    |> Chart.WithLayout (
     Layout(
        barmode="relative", 
        title= sprintf "Last 6 months transactions %s" date))
    |> Chart.WithSize (1200,900)
    |> CustomSaveHtmlAs "last6months"

//GenericChart.ofTraceObject sampleChart layout
//|> Chart.Show
// Define your library scripting code here

// fsi --exec Script.fsx