module Transactions

open System
open FSharp.Data.TypeProviders

[<Literal>]
let connStringName = "PaymentsData"
[<Literal>]
let schemaFile = "FullDbMap.dbml"

type internal DataConnection = 
 SqlDataConnection<
    ConnectionStringName=connStringName,
    LocalSchemaFile=schemaFile,
    ForceUpdate=false,
    Functions=false, 
    StoredProcedures=false, 
    Pluralize=true>
let private dc = DataConnection.GetDataContext()

type Currency = { BaseCurrencyId : int; TermsCurrencyId : int; Rate : decimal; }
let private curr b t r =
  { BaseCurrencyId = b; TermsCurrencyId = t; Rate = r }

let private ratesLazy = lazy (
    query {
        for r in dc.FXRates do
        select r
        }
    |> Seq.toList)

let precomputed = 
  let rates = ratesLazy.Force()
  let crosses =
      [for x in rates do
       for y in rates do
       if y.BaseCurrencyId = x.TermsCurrencyId 
       then 
        yield curr x.BaseCurrencyId y.TermsCurrencyId (1m * x.EndOfDayRate * y.EndOfDayRate)
        yield curr y.TermsCurrencyId x.BaseCurrencyId (1m / (1m * x.EndOfDayRate * y.EndOfDayRate))
      ]
  let directs =
      [for r in rates do 
       yield curr r.BaseCurrencyId r.TermsCurrencyId r.EndOfDayRate
       yield curr r.TermsCurrencyId r.BaseCurrencyId (1m / r.EndOfDayRate)
      ]
  List.append crosses directs

let private matchCurrency baseC termsC c = c.BaseCurrencyId = baseC && c.TermsCurrencyId = termsC
let private convertCurrency fromCurrency toCurrency amount =
  precomputed
  |>
  List.tryFind (fun c -> c.BaseCurrencyId = fromCurrency && c.TermsCurrencyId = toCurrency)
  |> Option.map (fun s-> amount * s.Rate)

let convert amount fromId toId = 
   match convertCurrency fromId toId amount with
   | Some c -> c
   | None -> 0m

let transactionTypeMap ttype =
 match ttype with
 | 63 | 26 | 28 | 269 -> "Bank Reversal"
 | 82 | 83|  84 -> "Card Deposit"
 | 115 -> "Card Withdrawal"
 | 230 | 231 -> "Card Reversal"
 | 25 | 27| 29 -> "Bank Deposit"
 | 102 | 62 | 103 -> "Bank Withdrawal"
 | 39 -> "Cheque/Verisign/Paypal"
 | 239 -> "Billing Japan"
 | 234 | 236 -> "Netbanx Deposit"
 | 273 -> "NETS Deposit"
 | 275 -> "Billpay Deposit"
 | 11 -> "BPay deposit"
 | 270 -> "PA Payout"
 | _ -> "Other"

let private transactions numDays = 
  query {
   for transaction in dc.LedgerTransactions do
   where (transaction.LedgerTransactionDateTime >= DateTime.UtcNow.Subtract(TimeSpan.FromDays(float numDays)))
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
let getStacks numDays =
    transactions numDays
    |> Seq.groupBy (fun t -> second t)
    |> Seq.map (fun t -> 
        let trans = snd t
        { 
            Name = fst t; 
            Days = getFrom trans first;
            Amounts = getFrom trans third;
        })
  