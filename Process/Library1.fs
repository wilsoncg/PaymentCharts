module Test

open System
open FSharp.Data.TypeProviders
open FSharp.Plotly

type internal DB = SqlDataConnection<"Data Source=(localdb)\MSSqlLocalDB;Initial Catalog=PaymentsData;Integrated Security=SSPI;", Pluralize=true>
let private dc = DB.GetDataContext()

let printSeq seq1 = Seq.iter (printf "%A ") seq1; printfn ""

let private gtransactions = 
  query {
   for transaction in dc.LedgerTransactions do
   where (transaction.LedgerTransactionDateTime >= DateTime.UtcNow.Subtract(TimeSpan.FromDays(28.00)))
   sortByDescending transaction.LedgerTransactionDateTime
   select transaction
  } 
  |> Seq.groupBy (fun tran -> tran.LedgerTransactionDateTime.Day, tran.LedgerTransactionTypeId)

let transactions =
  gtransactions
  |> Seq.map (fun groupedTransactions -> 
                            let day = 
                             match fst groupedTransactions with
                             | (tday, ttype) -> tday
                            let stype =
                             match fst groupedTransactions with
                             | (tday, ttype) -> 
                              match ttype with
                              | 82 -> "Deposit"
                              | 115 -> "Withdrawal"
                              | _ -> "Other"
                            let sumAmount = 
                             snd groupedTransactions 
                             |> Seq.fold (fun acc tran -> acc + tran.Amount) 0m
                            day, stype, sumAmount)

let first (a, _, _) = a
let second (_, b, _) = b
let third (_, _, c) = c

let days = 
  transactions
  |> Seq.map (fun t -> first t)  
  |> Seq.distinct
  |> List.ofSeq

let amounts =
  transactions
  |> Seq.groupBy (fun t -> first t)
  |> List.ofSeq
  
 // Order by date descending?

 // keys = days
 // values = list of amounts, for each day, for only one type
 // [ (100, 17), (99, 16), (101, 15) ] £100-17th, £99-16th, £101-15th...

// [17, 16, 15, 14, 13]
// [1122.15, xxxx ]
  
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