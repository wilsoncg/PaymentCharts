module Transactions

open System
open System.Linq
open ChartSettings
open FSharp.Plotly
open Microsoft.FSharp.Linq.RuntimeHelpers

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
        select (r)
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
 | 241 -> "PA Deposit"
 | 337 -> "Echeck Deposit"
 | 339 -> "Echeck Withdrawal"
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
        |> Seq.fold (fun acc (tran:Transaction) -> 
        match tran.CurrencyId with
        | 6 -> acc + tran.SumAmount
        | _ -> acc + convert currencies tran.SumAmount tran.CurrencyId 6
        ) 0m
    day, stype, sumAmount

let firstFrom8 (f, _,_,_,_,_,_) = f

let lastNDaysQueryFaster numDays (dc:dataContext) =
   let typeIds = [|63;26;28;269;82;83;84;115;230;231;25;27;29;102;62;103;39;234;236;273;275;11;270;241;337;339|]
   let findLatestTransaction =
    query {
        for transaction in dc.LedgerTransaction do
        where (transaction.LedgerTransactionDateTime < DateTime.UtcNow.Subtract(TimeSpan.FromDays(float numDays)))
        select (transaction.LedgerTransactionId)
        take 1
    }
   let query1 =
    query {
        for transaction in dc.LedgerTransaction.AsQueryable() do 
        where (transaction.LedgerTransactionId >= findLatestTransaction.First() &&
                typeIds.Contains(transaction.LedgerTransactionTypeId))
        select transaction } 
   query {
        for transaction in query1 do
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
        let splitTuple (w, x, y, z, s) =
          createTransaction w x y z s
        splitTuple t)
    |> Seq.toList
   
let private lastNDaysQuery numDays (dc:dataContext) =
   let typeIds = [|63;26;28;269;82;83;84;115;230;231;25;27;29;102;62;103;39;234;236;273;275;11;270;241;337;339|]
   query {
    for transaction in dc.LedgerTransaction do
    join ao in dc.AccountOperator on (transaction.AccountOperatorId.Value = ao.LegalPartyId)
    join lccp in dc.LegalContractCounterParty on (ao.LegalContractCounterPartyId = lccp.LegalContractCounterPartyId)
    join gl in dc.GeneralLedger on (transaction.LedgerTransactionId = gl.LedgerTransactionId)
    join ta in dc.TradingAccount on (gl.LedgerId = ta.LedgerId)
    join ca in dc.ClientAccount on (ta.ClientAccountId = ca.ClientAccountId)
    join ctype in dc.ClientType on (ca.ClientTypeId = ctype.ClientTypeId)  
    where (transaction.LedgerTransactionDateTime >= DateTime.UtcNow.Subtract(TimeSpan.FromDays(float numDays)) && 
            typeIds.Contains(transaction.LedgerTransactionTypeId) &&
            (ctype.ClientTypeId <> 2 && (lccp.IsDemo.Value <> true) && (lccp.IsTest.Value <> true)))
    sortByDescending transaction.LedgerTransactionDateTime
    select transaction
    } 
    |> Seq.map (fun t -> 
         createTransaction t.LedgerTransactionDateTime.DayOfYear t.LedgerTransactionDateTime.Hour t.LedgerTransactionTypeId t.CurrencyId t.Amount
        )
    |> Seq.toList

let private lastNDaysTransactions numDays (dc:dataContext) = 
 let currencies = currencyList dc
 lastNDaysQuery numDays dc 
  |> Seq.groupBy (fun tran -> tran.Day, tran.TypeId)
  |> Seq.map (fun groupedTransactions -> groupedToTuple groupedTransactions currencies)

let private lastNDaysTransactionsFaster numDays (dc:dataContext) = 
 let currencies = currencyList dc
 lastNDaysQueryFaster numDays dc 
  |> Seq.groupBy (fun tran -> tran.Day, tran.TypeId)
  |> Seq.map (fun groupedTransactions -> groupedToTuple groupedTransactions currencies)

let private last24hoursTransactions (dc:dataContext) =
 let currencies = currencyList dc
 lastNDaysQuery 1 dc
 |> Seq.groupBy (fun tran -> tran.Hour, tran.TypeId)
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
    lastNDaysTransactionsFaster numDays dataContext
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