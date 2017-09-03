module Test

open System
open FSharp.Data.TypeProviders

type internal DB = SqlDataConnection<"Data Source=(localdb)\MSSqlLocalDB;Initial Catalog=PaymentsData;Integrated Security=SSPI;", Pluralize=true>
let private dc = DB.GetDataContext()

let search map = Option.bind (fun k -> Map.tryFind k map)
type private Currency = { BaseCurrencyId : int; TermsCurrencyId : int; Rate : decimal; }
let private convertCurrency fromCurrency toCurrency amount =
 let rates =
    query {
        for r in dc.FXRates do
        select r
        }
    |> Seq.map (fun r -> 
        { 
            BaseCurrencyId = r.BaseCurrencyId; 
            TermsCurrencyId = r.TermsCurrencyId; 
            Rate = r.EndOfDayRate 
        })
    |> Seq.toList

 let direct fromId toId c =
    (c.BaseCurrencyId = fromId && c.TermsCurrencyId = toId)
 let invert fromId toId c =
    (c.TermsCurrencyId = fromId & c.BaseCurrencyId = toId)
 let cross fromId toId c =
    (c.BaseCurrencyId = fromId)// && c1.TermsCurrencyId = c2.BaseCurrencyId && c2.TermsCurrencyId = toId)
 let invertCross fromId toId c =
    (c.TermsCurrencyId = fromId)// && c1.BaseCurrencyId = c2.TermsCurrencyId && c2.BaseCurrencyId = toId)

 let converted =
  match List.tryFind (direct fromCurrency toCurrency) rates with
  | Some r -> amount * r.Rate 
  | None -> match List.tryFind (invert fromCurrency toCurrency) rates with
    | Some s -> amount / s.Rate
    | None -> match List.tryFind (invertCross fromCurrency toCurrency) rates with
        | Some t -> match List.tryFind (direct toCurrency t.BaseCurrencyId) rates with
                    | Some u -> amount / u.Rate / t.Rate
                    | None -> 0m
        | None -> match List.tryFind (cross fromCurrency toCurrency) rates with
            | Some v -> match List.tryFind (direct toCurrency v.TermsCurrencyId ) rates with
                | Some w -> amount * v.Rate / w.Rate
                | None -> 0m
            | None -> 0m
 converted

let convert amount fromId toId = 
    convertCurrency fromId toId amount 

let transactionTypeMap ttype =
 match ttype with
 | 82 -> "Deposit"
 | 115 -> "Withdrawal"
 | _ -> "Other"

let private transactions = 
  query {
   for transaction in dc.LedgerTransactions do
   where (transaction.LedgerTransactionDateTime >= DateTime.UtcNow.Subtract(TimeSpan.FromDays(28.00)))
   sortByDescending transaction.LedgerTransactionDateTime
   select transaction
  } 
  |> Seq.groupBy (fun tran -> tran.LedgerTransactionDateTime.Day, tran.LedgerTransactionTypeId)
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
  