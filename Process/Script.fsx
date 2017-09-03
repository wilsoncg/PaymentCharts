// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#r "../packages/FSharp.Plotly.1.0.2/lib/net40/FSharp.Plotly.dll"
#r "../packages/FSharp.Data.TypeProviders.5.0.0.2/lib/net40/FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq"

#load "Library1.fs"

open FSharp.Plotly

let layout =
    Layout()

// [(deposit, [(2, deposit, 220); (1, deposit, 2655); ... ], withdrawal [(1, withdrawal, 110); ..] )]

let stacks =
 Test.stacks
 |> Seq.map (fun t ->
        Chart.StackedBar(t.Days, t.Amounts, Name= sprintf "%A" t.Name))

let chart = 
  stacks
  |> Chart.Combine
  |> Chart.Show

//GenericChart.ofTraceObject sampleChart layout
//|> Chart.Show
// Define your library scripting code here

// fsi --exec Script.fsx