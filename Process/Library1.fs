module Test

open System
open FSharp.Data.TypeProviders

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

let search map = Option.bind (fun k -> Map.tryFind k map)
type private Currency = { BaseCurrencyId : int; TermsCurrencyId : int; Rate : decimal; }
let private convertCurrency fromCurrency toCurrency amount =
 let rates =
    query {
        for r in dc.FXRates do
        select r
        }
    |> Seq.map (fun r -> { BaseCurrencyId = r.BaseCurrencyId; TermsCurrencyId = r.TermsCurrencyId; Rate = r.EndOfDayRate })
    |> Seq.toList
 let direct fromId toId c =
    (c.BaseCurrencyId = fromId && c.TermsCurrencyId = toId)
 let invert fromId toId c =
    (c.TermsCurrencyId = fromId & c.BaseCurrencyId = toId)
 let cross c1 c2 fromId toId =
    (c1.BaseCurrencyId = fromId & c1.TermsCurrencyId = c2.BaseCurrencyId & c2.TermsCurrencyId = toId)
 let invertCross c1 c2 fromId toId =
    (c1.TermsCurrencyId = fromId & c1.BaseCurrencyId = c2.TermsCurrencyId & c2.BaseCurrencyId = toId)

 let converted =
  match List.tryFind (direct fromCurrency toCurrency) rates with
  | Some r -> amount * r.Rate 
  | None -> match List.tryFind (invert fromCurrency toCurrency) rates with
    | Some s -> amount / s.Rate
    | None -> 0m
 converted
 

let convert amount fromId toId = 
    convertCurrency fromId toId amount 

let transactionTypeMap ttype =
 match ttype with
 | 82 -> "Deposit"
 | 115 -> "Withdrawal"
 | _ -> "Other"

let transactions =
  gtransactions
  |> Seq.map (fun groupedTransactions -> 
                            let day = 
                             match fst groupedTransactions with
                             | (tday, _) -> tday
                            let stype =
                             match fst groupedTransactions with
                             | (_, ttype) -> transactionTypeMap ttype
                            let sumAmount = 
                             snd groupedTransactions 
                             |> Seq.fold (fun acc tran -> 
                                match tran.CurrencyId with
                                | 6 -> acc + tran.Amount
                                | _ -> acc + convert tran.Amount tran.CurrencyId 6
                                ) 0m
                            day, stype, sumAmount)

let first (a, _, _) = a
let second (_, b, _) = b
let third (_, _, c) = c

//let days, amounts =
//  transactions
//  |> Seq.map (fun t -> 
//    first t, third t)
//  |> Seq.toList
//  |> List.unzip

let getFrom list selector =
  list
  |> Seq.map selector

type StackInfo = { Name : string; Days : seq<int>; Amounts : seq<decimal> }
let stacks =
    transactions
    |> Seq.groupBy (fun t -> second t)
    |> Seq.map (fun t -> 
        let trans = snd t
        { 
            Name = fst t; 
            Days = getFrom trans first;
            Amounts = getFrom trans third;
        })
  
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