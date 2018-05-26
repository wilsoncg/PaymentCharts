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
	where [t0].LedgerTransactionDateTime >= '2000-01-01'
    ) AS [t7]
WHERE 
--([t7].[LedgerTransactionDateTime] >= '2000-01-01') AND 
--([t7].[LedgerTransactionTypeId] IN (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17, @p18, @p19, @p20, @p21, @p22, @p23, @p24, @p25, @p26)) AND ([t7].[ClientTypeId] <> @p27) AND 
(NOT (([t7].[IsDemo]) = 1)) AND (NOT (([t7].[IsTest]) = 1))
GROUP BY [t7].[value], [t7].[value2], [t7].[LedgerTransactionTypeId], [t7].[CurrencyId]