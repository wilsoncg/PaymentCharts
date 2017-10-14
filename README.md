## About

Some F# code combined with Plotly for graphing some payment transactions.

- [FSharp.Plotly](https://github.com/muehlhaus/FSharp.Plotly)
- [FSharp.Data.SqlProvider](https://fsprojects.github.io/SQLProvider/) *Not be confused with [FSharp.Data.TypeProviders](http://fsprojects.github.io/FSharp.Data.TypeProviders/sqldata.html) which ships with FSharp*
- [FSharp.Configuration](https://github.com/fsprojects/FSharp.Configuration)

```fsharp
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
  |> Chart.withLayout (
    Layout.init (
        Barmode=StyleParam.Barmode.Stack, 
        Title= sprintf "Last %i days transactions" numDays))
  |> Chart.withSize (1200,900)
  |> Chart.SaveHtmlAs "last7days" 
```

Creates something like this:
![Last 7 days](last7days.png)