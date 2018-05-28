﻿module Transactions

open System
open System.Linq
open ChartSettings
open Microsoft.FSharp.Linq.RuntimeHelpers
open System.Globalization

type dataContext = ChartSettings.PaymentsDb.ServiceTypes.SimpleDataContextTypes.PaymentsData

type Currency = { BaseCurrencyId : int; TermsCurrencyId : int; Rate : decimal; }
let private createCurrency b t r =
  { BaseCurrencyId = b; TermsCurrencyId = t; Rate = r }

type Transaction = {
    Day : int;
    Hour : int;
    TypeId : int;
    CurrencyId : int;
    SumAmount : decimal;
    }
let private createTransaction d h t c a =
    { Day = d; Hour = h; TypeId = t; CurrencyId = c; SumAmount = a }

let currencyList (dc:dataContext) = 
  let rates = 
    query {
        for r in dc.FxRate do
        select r
        }
    |> Seq.toList
  let crosses =
      [for x in rates do
       for y in rates do
       if y.BaseCurrencyId = x.TermsCurrencyId 
       then 
        yield createCurrency x.BaseCurrencyId y.TermsCurrencyId (1m * x.EndOfDayRate * y.EndOfDayRate)
        yield createCurrency y.TermsCurrencyId x.BaseCurrencyId (1m / (1m * x.EndOfDayRate * y.EndOfDayRate))
      ]
  let directs =
      [for r in rates do 
       yield createCurrency r.BaseCurrencyId r.TermsCurrencyId r.EndOfDayRate
       yield createCurrency r.TermsCurrencyId r.BaseCurrencyId (1m / r.EndOfDayRate)
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
 | 230 | 231 -> "Card Reversal" 
 | 25 | 27| 29 -> "Bank Deposit" 
 | 82 | 83|  84 -> "Card Deposit"
 | 39 -> "Cheque/Verisign/Paypal"
 | 239 -> "Billing Japan"
 | 234 | 236 -> "Netbanx Deposit"
 | 337 -> "Echeck Deposit"
 | 273 -> "NETS Deposit"
 | 275 -> "Billpay Deposit"
 | 11 -> "BPay deposit" 
 | 241 -> "PA Deposit"
 | 102 | 62 | 103 -> "Bank Withdrawal"
 | 339 -> "Echeck Withdrawal"
 | 115 -> "Card Withdrawal"
 | 270 -> "PA Payout"
 | _ -> "Other"

let (|Contains|_|) (c:string) (s:string) =
    if s.ToLower().Contains(c) then
        Some(s)
    else
        None

let colourMap ttype =
    match ttype with
    | Contains "bank deposit" s -> "#339933"
    | Contains "card deposit" s -> "#40bf40"
    | Contains "cheque" s -> "#267326"
    | Contains "japan" s -> "#5c0099"
    | Contains "netbanx" s -> "#6b00b3"
    | Contains "echeck deposit" s -> "#7a00cc"
    | Contains "nets" s -> "#8a00e6"
    | Contains "billpay" s -> "#9900ff"
    | Contains "bpay" s -> "#a31aff"
    | Contains "pa deposit" s-> "#ad33ff"
    | Contains "bank withdrawal" s -> "#e68a00"
    | Contains "card withdrawal" s-> "#ff9900"
    | Contains "pa payout" s -> "#ffa31a"
    | Contains "echeck withdrawal" s -> "#ffad33"
    | _ -> "#527a7a"

let private groupedToTuple groupedTransactions currencies =
    let day = 
        match fst groupedTransactions with
        | (tday, _) -> tday
    let stype =
        match fst groupedTransactions with
        | (_, ttype) -> transactionTypeMap ttype
    let sumAmount = 
        snd groupedTransactions 
        |> Seq.fold (fun acc (tran:Transaction) -> 
        match tran.CurrencyId with
        | 6 -> acc + tran.SumAmount
        | _ -> acc + convert currencies tran.SumAmount tran.CurrencyId 6
        ) 0m
    let makeWithdrawalNegative stype amount =
        match stype with
        | Contains "echeck" s -> amount
        | Contains "withdrawal" s -> -amount
        | Contains "payout" s -> -amount
        | _ -> amount
    day, stype, makeWithdrawalNegative stype sumAmount

let lastNDaysQueryFaster numDays (dc:dataContext) =
   let typeIds = [|63;26;28;269;82;83;84;115;230;231;25;27;29;102;62;103;39;239;234;236;273;275;11;270;241;337;339|]
   let findLatestTransaction =
    query {
        for transaction in dc.LedgerTransaction do
        where (transaction.LedgerTransactionDateTime < DateTime.UtcNow.Subtract(TimeSpan.FromDays(float numDays)))
        select (transaction.LedgerTransactionId)
        take 1 }
   let notTestOrDemo =
    query {
        for ao in dc.AccountOperator do
        join lccp in dc.LegalContractCounterParty on (ao.LegalContractCounterPartyId = lccp.LegalContractCounterPartyId)
        where (lccp.IsDemo.Value <> true && lccp.IsTest.Value <> true)
        select ao.LegalPartyId
    }
   let interestedTransactions =
    query {
        for transaction in dc.LedgerTransaction.AsQueryable() do 
        where (transaction.LedgerTransactionId >= findLatestTransaction.First() &&
                typeIds.Contains(transaction.LedgerTransactionTypeId) &&
                notTestOrDemo.Contains(transaction.AccountOperatorId.Value))
        select transaction } 
   query {
        for transaction in interestedTransactions do
        let key = 
           AnonymousObject<_,_,_,_>(
            transaction.LedgerTransactionDateTime.DayOfYear, 
            transaction.LedgerTransactionDateTime.Hour,
            transaction.LedgerTransactionTypeId, 
            transaction.CurrencyId)
        groupValBy transaction key into g
        let summed = 
         query {
            for gs in g do
            sumBy gs.Amount }
        select (g.Key.Item1, g.Key.Item2, g.Key.Item3, g.Key.Item4, summed)
    } 
    |> Seq.toList
    |> Seq.map (fun t -> 
        let splitTuple (day, hour, typeId, currencyId, sum) =
          createTransaction day hour typeId currencyId sum
        splitTuple t)
    |> Seq.toList
    
let private lastNDaysTransactionsFaster numDays (dc:dataContext) = 
 let currencies = currencyList dc
 lastNDaysQueryFaster numDays dc 
  |> Seq.groupBy (fun tran -> tran.Day, tran.TypeId)
  |> Seq.map (fun groupedTransactions -> groupedToTuple groupedTransactions currencies)

let private last24hoursTransactions (dc:dataContext) =
 let currencies = currencyList dc
 lastNDaysQueryFaster 1 dc
 |> Seq.groupBy (fun tran -> tran.Hour, tran.TypeId)
 |> Seq.map (fun groupedTrans -> groupedToTuple groupedTrans currencies)

let first (a, _, _) = a
let second (_, b, _) = b
let third (_, _, c) = c

let getFrom list selector =
  list
  |> Seq.map selector

let mapToDateDisplay days =
    days 
    |> Seq.map (fun d -> 
        (new DateTime(DateTime.Now.Year, 1, 1)).AddDays(float(d - 1)).ToString("MMM dd")) 

type DaysStackInfo = { Name : string; Days : seq<int>; DaysText: seq<string>; Amounts : seq<decimal>; Colour : string }
type HoursStackInfo = { Name : string; Hours : seq<int>; Amounts : seq<decimal>; Colour : string }

let getDaysStacks numDays dataContext =
    lastNDaysTransactionsFaster numDays dataContext
    |> Seq.sortByDescending (fun t -> first t)
    |> Seq.groupBy (fun t -> 
        let selectType (_, transactionType, _) = transactionType
        selectType t)
    |> Seq.map (fun t -> 
        let trans = snd t
        { 
            Name = fst t; 
            Days = getFrom trans first;
            Amounts = getFrom trans third;
            Colour = colourMap (fst t);
            DaysText = mapToDateDisplay (getFrom trans first)
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
            Colour = colourMap (fst t)
        }) 