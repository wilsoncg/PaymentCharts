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
//
let stacks =
 Test.amounts
 |> Seq.map (fun t ->
            let name = fst t
            let transactions = snd t
            let _, _, amounts = 
             transactions
             |> Seq.toList
             |> List.unzip3
            transactions
            //|> Seq.map (fun t -> Test.second t == name ? Test.third t : 0m)
            //|> Seq.map (fun t ->
               //match Test.second t with
               //| name -> Test.third t)
            //|> Seq.toList
            //|> List.unzip3 (fun t -> third t)
            ////|> Seq.where (fun t -> Test.second t = name)
            //|> Seq.toList
            //|> List.unzip3 // (fun t -> Test.third t)
            //|> List.iter (fun t -> sprintf t)
            |> Seq.map (fun t ->
               Chart.StackedBar(Test.days, amounts, Name= sprintf "%A" name)))
            |> Seq.concat

let otherStacks =
 Test.transactions
 |> Seq.map (fun t ->
            let _, name, amounts = t 
             |> Seq.toList
             |> List.unzip3
             t
             |> Seq.map (fun t ->
                Chart.StackedBar(Test.days, amounts, Name= )

let chart = 
  stacks
  |> Chart.Combine
  |> Chart.Show

//GenericChart.ofTraceObject sampleChart layout
//|> Chart.Show
// Define your library scripting code here

