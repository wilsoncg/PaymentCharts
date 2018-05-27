-- created
SELECT SUM([t7].[Amount]) AS [Item5], [t7].[value] AS [Item1], [t7].[value2] AS [Item2], [t7].[LedgerTransactionTypeId] AS [Item3], [t7].[CurrencyId] AS [Item4]
FROM (
    SELECT [t0].[LedgerTransactionTypeId], [t0].[LedgerTransactionDateTime], [t0].[Amount], [t0].[CurrencyId], DATEPART(DayOfYear, [t0].[LedgerTransactionDateTime]) AS [value], DATEPART(Hour, [t0].[LedgerTransactionDateTime]) AS [value2], [t6].[ClientTypeId], [t2].[IsDemo], [t2].[IsTest]
    FROM [dbo].[LedgerTransaction] AS [t0]
    INNER JOIN [dbo].[AccountOperator] AS [t1] ON ([t0].[AccountOperatorId]) = [t1].[LegalPartyId]
    INNER JOIN [dbo].[LegalContractCounterParty] AS [t2] ON [t1].[LegalContractCounterPartyId] = [t2].[LegalContractCounterPartyId]
    INNER JOIN [dbo].[GeneralLedger] AS [t3] ON [t0].[LedgerTransactionId] = [t3].[LedgerTransactionId]
    INNER JOIN [dbo].[TradingAccount] AS [t4] ON [t3].[LedgerId] = [t4].[LedgerId]
    INNER JOIN [dbo].[ClientAccount] AS [t5] ON [t4].[ClientAccountId] = [t5].[ClientAccountId]
    INNER JOIN [dbo].[ClientType] AS [t6] ON [t5].[ClientTypeId] = [t6].[ClientTypeId]
    ) AS [t7]
WHERE ([t7].[LedgerTransactionDateTime] >= '2000-01-01') AND 
--([t7].[LedgerTransactionTypeId] IN (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17, @p18, @p19, @p20, @p21, @p22, @p23, @p24, @p25, @p26)) AND ([t7].[ClientTypeId] <> @p27) AND 
(NOT (([t7].[IsDemo]) = 1)) AND (NOT (([t7].[IsTest]) = 1))
GROUP BY [t7].[value], [t7].[value2], [t7].[LedgerTransactionTypeId], [t7].[CurrencyId]

SELECT TOP (1) [t1].[LedgerTransactionId]
FROM (
    SELECT TOP (1) [t0].[LedgerTransactionId]
    FROM [dbo].[LedgerTransaction] AS [t0]
    WHERE [t0].[LedgerTransactionDateTime] >= '2018-04-01'
    ORDER BY [t0].[LedgerTransactionId]
    ) AS [t1]
ORDER BY [t1].[LedgerTransactionId]

--adjusted
SELECT SUM([t7].[Amount]) AS [Item5], [t7].[value] AS [Item1], [t7].[value2] AS [Item2], [t7].[LedgerTransactionTypeId] AS [Item3], [t7].[CurrencyId] AS [Item4]
FROM (
    SELECT [t0].[LedgerTransactionTypeId], [t0].[LedgerTransactionDateTime], [t0].[Amount], [t0].[CurrencyId], DATEPART(DayOfYear, [t0].[LedgerTransactionDateTime]) AS [value], DATEPART(Hour, [t0].[LedgerTransactionDateTime]) AS [value2], [t6].[ClientTypeId], [t2].[IsDemo], [t2].[IsTest]
    FROM [dbo].[LedgerTransaction] AS [t0]
    INNER JOIN [dbo].[AccountOperator] AS [t1] ON ([t0].[AccountOperatorId]) = [t1].[LegalPartyId]
    INNER JOIN [dbo].[LegalContractCounterParty] AS [t2] ON [t1].[LegalContractCounterPartyId] = [t2].[LegalContractCounterPartyId]
    INNER JOIN [dbo].[GeneralLedger] AS [t3] ON [t0].[LedgerTransactionId] = [t3].[LedgerTransactionId]
    INNER JOIN [dbo].[TradingAccount] AS [t4] ON [t3].[LedgerId] = [t4].[LedgerId]
    INNER JOIN [dbo].[ClientAccount] AS [t5] ON [t4].[ClientAccountId] = [t5].[ClientAccountId]
    INNER JOIN [dbo].[ClientType] AS [t6] ON [t5].[ClientTypeId] = [t6].[ClientTypeId]
	where [t0].LedgerTransactionId > 753192567 and
	([t0].[LedgerTransactionTypeId] IN (63,26,28,269,82,83,84,115,230,231,25,27,29,102,62,103,39,234,236,273,275,11,270,241,337,339))
    ) AS [t7]
WHERE 
--([t7].[LedgerTransactionDateTime] >= '2000-01-01') AND 
--([t7].[LedgerTransactionTypeId] IN (63,26,28,269,82,83,84,115,230,231,25,27,29,102,62,103,39,234,236,273,275,11,270,241,337,339)) AND
--AND ([t7].[ClientTypeId] <> @p27) AND 
(NOT (([t7].[IsDemo]) = 1)) AND (NOT (([t7].[IsTest]) = 1))
GROUP BY [t7].[value], [t7].[value2], [t7].[LedgerTransactionTypeId], [t7].[CurrencyId]

-- notes
-- quick because of index DateTime desc on LedgerTransaction
SELECT --[t0].[LedgerTransactionId],[t0].[LedgerTransactionDateTime]  --
TOP (1) [t0].[LedgerTransactionId]
    FROM [dbo].[LedgerTransaction] AS [t0]
    WHERE [t0].[LedgerTransactionDateTime] > '2018-04-01 00:00:00' and [t0].[LedgerTransactionDateTime] < '2018-04-02 00:00:00'
    --ORDER BY [t0].[LedgerTransactionId]

select * from LedgerTransaction where LedgerTransactionid = 753192567

select * from LedgerTransaction where LedgerTransactionid > 753192567
and ledgertransactiontypeid IN (63,26,28,269,82,83,84,115,230,231,25,27,29,102,62,103,39,234,236,273,275,11,270,241,337,339)

SELECT [t0].[LedgerTransactionTypeId], [t0].[LedgerTransactionDateTime], [t0].[Amount], [t0].[CurrencyId]
, DATEPART(DayOfYear, [t0].[LedgerTransactionDateTime]) AS [value], DATEPART(Hour, [t0].[LedgerTransactionDateTime]) AS [value2]
--,gl.LedgerId as [LedgerId]
--, [t6].[ClientTypeId], [t2].[IsDemo], [t2].[IsTest]
    FROM [dbo].[LedgerTransaction] AS [t0]	
	--inner join GeneralLedger gl on t0.LedgerTransactionId = gl.LedgerTransactionId
	where [t0].LedgerTransactionId > 753192567 and
	([t0].[LedgerTransactionTypeId] IN (63,26,28,269,82,83,84,115,230,231,25,27,29,102,62,103,39,234,236,273,275,11,270,241,337,339))
	and t0.AccountOperatorID in (
		select ao.LegalPartyId
		from [dbo].[AccountOperator] ao
		INNER JOIN [dbo].[LegalContractCounterParty] AS [lccp] ON ao.[LegalContractCounterPartyId] = [lccp].[LegalContractCounterPartyId]
		where ([lccp].[IsDemo] <> 1) AND ([lccp].[IsTest] <> 1)
	)	
	AND t0.LedgerTransactionId in (
		select LedgerTransaction.LedgerTransactionId 
		from GeneralLedger gl
		inner join LedgerTransaction on gl.LedgerTransactionId = LedgerTransaction.LedgerTransactionId
		INNER JOIN [dbo].[TradingAccount] AS [t4] ON gl.[LedgerId] = [t4].[LedgerId]
		INNER JOIN [dbo].[ClientAccount] AS [t5] ON [t4].[ClientAccountId] = [t5].[ClientAccountId]
		INNER JOIN [dbo].[ClientType] AS [t6] ON [t5].[ClientTypeId] = [t6].[ClientTypeId]
		where [t6].[ClientTypeId] <> 2
	)
