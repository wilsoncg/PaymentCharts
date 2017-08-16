module Test

open System
open FSharp.Data.TypeProviders
open FSharp.Plotly

type internal DB = SqlDataConnection<"Data Source=(localdb)\MSSqlLocalDB;Initial Catalog=PaymentsData;Integrated Security=SSPI;", Pluralize=true>
 let private dc = DB.GetDataContext()

 let printSeq seq1 = Seq.iter (printf "%A ") seq1; printfn ""

 let d, t, s = 
  query {
   for transaction in dc.LedgerTransactions do
   where (transaction.LedgerTransactionDateTime > DateTime.UtcNow.Subtract(TimeSpan.FromDays(7.00)))
   select transaction
  } 
  |> Seq.groupBy (fun tran -> tran.LedgerTransactionDateTime.Day, tran.LedgerTransactionTypeId)
  |> Seq.map (fun groupedTransactions -> 
                            let day = 
                             match fst groupedTransactions with
                             | (tday, ttype) -> tday
                            let ttype =
                             match fst groupedTransactions with
                             | (tday, ttype) -> ttype
                            let sumAmount = snd groupedTransactions |> Seq.fold (fun acc tran -> acc + tran.Amount) 0m
                            let depOrWith = snd groupedTransactions |> Seq.groupBy (fun tran -> tran.LedgerTransactionTypeId)
                            day, ttype, sumAmount)
  |> List.ofSeq 
  |> List.unzip3 
  
 //let days = d |> Seq.ofList

 
// [1, 2, 3, 4, 5]
// [100, 50, 80, 110, 120]
  
//printSeq sum
  //|> Seq.fold (fun acc tran -> acc + tran.Amount) 0m
  //|> Seq.iter (fun t -> t.Amount)
  //|> Seq.fold (fun acc elem -> acc + elem) 0 transaction.A

// line plot
// x-axis, date (days)
// y-axis, amount (sum), payment method (transaction type)

// let x  = [1.; 2.; 3.; 4.; 5.; 6.; 7.; 8.; 9.; 10.; ]
// let y = amount summed per day
// yCardDeposit, yCardWithdrawal, yBank, etc