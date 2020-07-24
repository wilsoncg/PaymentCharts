// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
// SqlProvider (https://github.com/fsprojects/SQLProvider) FSharp.Data.SqlTypeProvider namespace
// https://github.com/fsprojects/FSharp.Data.SqlClient/issues/373
#r "nuget:XPlot.Plotly"
#r "nuget:SQLProvider"
#r "nuget:HtmlAgilityPack"
#r "nuget:FSharp.Data.TypeProviders"
//#r "System.Data.Linq"
//#r "System.Data.SqlClient"

#load "ChartSettings.fs"
#load "Transactions.fs"
#load "CustomChartExtensions.fs"

open FSharp.Data
open XPlot.Plotly
open CustomChartExtensions

let numDays = 31
let dc = ChartSettings.PaymentsDb.GetDataContext()
let date = System.DateTime.UtcNow.ToString("dd/MM/yy")

let daysChart = 
  let stacks = Transactions.getDaysStacks numDays dc
  if Seq.isEmpty stacks then stacks |> ignore 
  else
  stacks
  |> Seq.map (fun t -> 
    Bar(
     x = t.DaysText, 
     y = t.Amounts, 
     name= sprintf "%A" t.Name, 
     marker = Marker(color = t.Colour)))
  |> Chart.Plot  
  |> Chart.WithLayout(
    Layout(
        barmode="relative", 
        xaxis= Xaxis(tickangle= -45.),
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