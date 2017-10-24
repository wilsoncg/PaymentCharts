module Transactions

open System
open System.Linq
open FSharp.Data.Sql
open FSharp.Data.Sql.Runtime
open ChartSettings
open FSharp.Plotly

type Currency = { BaseCurrencyId : int; TermsCurrencyId : int; Rate : decimal; }
let private curr b t r =
  { BaseCurrencyId = b; TermsCurrencyId = t; Rate = r }

let currencyList (dc:PaymentsDb.dataContext) = 
  let rates = 
    query {
        for r in dc.Dbo.FxRate do
        select (r)
        }
    |> Seq.toList
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

let private convertCurrency list fromCurrency toCurrency amount =
  list
  |> List.tryFind (fun c -> c.BaseCurrencyId = fromCurrency && c.TermsCurrencyId = toCurrency)
  |> Option.map (fun s-> amount * s.Rate)

let convert list amount fromId toId = 
   match convertCurrency list fromId toId amount with
   | Some c -> Math.Round(c, 2) 
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

let private groupedToTuple groupedTransactions currencies =
    let day = 
        match fst groupedTransactions with
        | (tday, _) -> tday
    let stype =
        match fst groupedTransactions with
        | (_, ttype) -> transactionTypeMap ttype
    let sumAmount = 
        snd groupedTransactions 
        |> Seq.fold (fun acc (tran:PaymentsDb.dataContext.``dbo.LedgerTransactionEntity``) -> 
        match tran.CurrencyId with
        | 6 -> acc + tran.Amount
        | _ -> acc + convert currencies tran.Amount tran.CurrencyId 6
        ) 0m
    day, stype, sumAmount

let private lastNDaysQuery numDays (dc:PaymentsDb.dataContext) =
   let typeIds = [|63;26;28;269;82;83;84;115;230;231;25;27;29;102;62;103;39;234;236;273;275;11;270|]
   query {
    for transaction in dc.Dbo.LedgerTransaction do
    join ao in dc.Dbo.AccountOperator on (transaction.AccountOperatorId = ao.LegalPartyId)
    join lccp in dc.Dbo.LegalContractCounterParty on (ao.LegalContractCounterPartyId = lccp.LegalContractCounterPartyId)
    join gl in dc.Dbo.GeneralLedger on (transaction.LedgerTransactionId = gl.LedgerTransactionId)
    join ta in dc.Dbo.TradingAccount on (gl.LedgerId = ta.LedgerId)
    join ca in dc.Dbo.ClientAccount on (ta.ClientAccountId = ca.ClientAccountId)
    join ctype in dc.Dbo.ClientType on (ca.ClientTypeId = ctype.ClientTypeId)  
    where (transaction.LedgerTransactionDateTime >= DateTime.UtcNow.Subtract(TimeSpan.FromDays(float numDays)) && 
            typeIds.Contains(transaction.LedgerTransactionTypeId) &&
            (ctype.ClientTypeId <> 2 && (lccp.IsDemo <> true) && (lccp.IsTest <> true)))
    sortByDescending transaction.LedgerTransactionDateTime
    select transaction
    } |> Seq.toList

let private lastNDaysTransactions numDays (dc:PaymentsDb.dataContext) = 
 let currencies = currencyList dc
 lastNDaysQuery numDays dc 
  |> Seq.groupBy (fun tran -> tran.LedgerTransactionDateTime.Day, tran.LedgerTransactionTypeId)
  |> Seq.map (fun groupedTransactions -> groupedToTuple groupedTransactions currencies)

let private last24hoursTransactions (dc:PaymentsDb.dataContext) =
 let currencies = currencyList dc
 lastNDaysQuery 1 dc
 |> Seq.groupBy (fun tran -> tran.LedgerTransactionDateTime.Hour, tran.LedgerTransactionTypeId)
 |> Seq.map (fun groupedTrans -> groupedToTuple groupedTrans currencies)

let first (a, _, _) = a
let second (_, b, _) = b
let third (_, _, c) = c

let getFrom list selector =
  list
  |> Seq.map selector

type DaysStackInfo = { Name : string; Days : seq<int>; Amounts : seq<decimal> }
type HoursStackInfo = { Name : string; Hours : seq<int>; Amounts : seq<decimal> }
let getDaysStacks numDays dataContext =
    lastNDaysTransactions numDays dataContext
    |> Seq.groupBy (fun t -> second t)
    |> Seq.map (fun t -> 
        let trans = snd t
        { 
            Name = fst t; 
            Days = getFrom trans first;
            Amounts = getFrom trans third;
        })

let getHoursStacks dataContext =
    last24hoursTransactions dataContext
    |> Seq.groupBy (fun t -> second t)
    |> Seq.map (fun t -> 
        let trans = snd t
        { 
            Name = fst t; 
            Hours = getFrom trans first;
            Amounts = getFrom trans third;
        }) 