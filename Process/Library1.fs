module Test

open System
open System.Data
open System.Data.Linq
open FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq

type internal DB = SqlDataConnection<"Data Source=(localdb)\MSSqlLocalDB;Initial Catalog=PaymentsData;Integrated Security=SSPI;", Pluralize=true>
 let private dc = DB.GetDataContext()

 let private sum = 
  query {
   for transaction in dc.LedgerTransactions do
   where (transaction.LedgerTransactionDateTime > DateTime.UtcNow.Subtract(TimeSpan.FromDays(7.00)))
   select transaction
  } 
  |> Seq.groupBy (fun tran -> tran.LedgerTransactionDateTime.Day)
  |> Seq.map (fun tranDay -> 
                            let day = fst tranDay
                            let sumAmount = snd tranDay |> Seq.fold (fun acc tran -> acc + tran.Amount) 0m
                            day, sumAmount)
  
  //|> Seq.fold (fun acc tran -> acc + tran.Amount) 0m
  //|> Seq.iter (fun t -> t.Amount)
  //|> Seq.fold (fun acc elem -> acc + elem) 0 transaction.A

// line plot
// x-axis, date (days)
// y-axis, amount (sum), payment method (transaction type)

// let x  = [1.; 2.; 3.; 4.; 5.; 6.; 7.; 8.; 9.; 10.; ]
// let y = amount summed per day
// yCardDeposit, yCardWithdrawal, yBank, etc