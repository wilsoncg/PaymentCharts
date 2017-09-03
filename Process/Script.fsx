// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#r "../packages/FSharp.Plotly.1.0.2/lib/net40/FSharp.Plotly.dll"
#r "../packages/FSharp.Data.TypeProviders.5.0.0.2/lib/net40/FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq"

#load "Library1.fs"

open FSharp.Plotly

let layout =
    Layout()

// [(deposit, [(17, deposit, 100); (16, deposit, 99); ... ], withdrawal [(17, withdrawal, 100); ..] )]

let stacks =
 Test.amounts
 |> Seq.map (fun t ->
            let transactions = snd t
            let _, name, amounts = 
             transactions
             |> Seq.toList
             |> List.unzip3
            transactions
            |> Seq.map (fun t ->
               Chart.StackedBar(Test.days, amounts, Name= sprintf "%A" name)))
            |> Seq.concat
            
let chart = 
  stacks
  |> Chart.Combine
  |> Chart.Show

//GenericChart.ofTraceObject sampleChart layout
//|> Chart.Show
// Define your library scripting code here

