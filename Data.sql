SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE [master];
GO

ALTER DATABASE PaymentsData
SET SINGLE_USER WITH
ROLLBACK AFTER 5 --this will give your current connections 5 seconds to complete

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'PaymentsData')
  DROP DATABASE PaymentsData;
GO

-- Create the PaymentsData database.
CREATE DATABASE PaymentsData;
GO

-- Specify a simple recovery model 
-- to keep the log growth to a minimum.
ALTER DATABASE PaymentsData 
SET RECOVERY SIMPLE;
GO

USE PaymentsData;
GO

-- Create the Table1 table.
CREATE TABLE [dbo].[LedgerTransaction] (
  [LedgerTransactionId]			INT        NOT NULL,
  [LedgerTransactionTypeId]		INT    NOT NULL,
  [LedgerTransactionDateTime] DATETIME	NOT NULL,
  [Amount]      DECIMAL(18,6)      NOT NULL,
  [CurrencyId] INT not null,
  [AccountOperatorId] int null
  PRIMARY KEY CLUSTERED ([LedgerTransactionId] ASC)
);

CREATE TABLE [dbo].[FxRate] (
    [BaseCurrencyId]  INT             NOT NULL,
    [TermsCurrencyId] INT             NOT NULL,
    [EndOfDayRate]    DECIMAL (18, 7) NOT NULL
	PRIMARY KEY CLUSTERED ([BaseCurrencyId] ASC, [TermsCurrencyId] ASC)
);

create table [dbo].[AccountOperator] (
	[LegalPartyId]	int not null,
	[LegalContractCounterPartyId] int not null
);

create table [dbo].[LegalContractCounterParty] (
	[LegalContractCounterPartyId] int not null,
	[IsDemo] bit,
	[IsTest] bit
);

create table [dbo].[GeneralLedger] (
	[LedgerId] int not null,
	[LedgerTransactionId] int not null
);

create table [dbo].[TradingAccount] (
	[ClientAccountId] int not null,
	[LedgerId] int not null
);

create table [dbo].[ClientAccount] (
	[ClientAccountId] int not null,
	[ClientTypeId] int not null
);

create table [dbo].[ClientType] (
	[ClientTypeId] int not null
);

USE PaymentsData

declare @date DateTime
set @date = GETUTCDATE()

-- Insert data into the Table1 table.
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
  VALUES(1, 82, @date - (0.04167*10), 200.000000, 6, 1);
 INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
  VALUES(2, 82, @date - (0.04167*9), 200.000000, 24, 1);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
   VALUES(3, 83, @date - (0.04167*8), 20.000000, 25, 1);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
VALUES(4, 82, @date - (0.04167*7), 100.000000, 6, 1);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
VALUES(5, 115, @date - (0.04167*6), 50.000000, 6, 1);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
VALUES(6, 115, @date - (0.04167*5), 60.000000, 6, 1);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
VALUES(7, 82, @date - (0.04167*4), 2400.000000, 6, 1);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
VALUES(8, 82, @date - (0.04167*3), 50.000000, 24, 1);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
VALUES(9, 82, @date - (0.04167*2), 55.000000, 6, 1);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
VALUES(10, 82, @date - 0.04167, 50.000000, 6, 1);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId, AccountOperatorId)
VALUES(11, 1, @date, 50000.000000, 25, 2);

insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (1, 1)
insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (1, 2)
insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (1, 3)
insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (1, 4)
insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (1, 5)
insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (1, 6)
insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (1, 7)
insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (1, 8)
insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (1, 9)
insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (1, 10)
insert into [dbo].[GeneralLedger] ([LedgerId], [LedgerTransactionId]) values (2, 11)

insert into [dbo].[LegalContractCounterParty] ([LegalContractCounterPartyId],[IsDemo],[IsTest]) values (1, 0, 0)
insert into [dbo].[LegalContractCounterParty] ([LegalContractCounterPartyId],[IsDemo],[IsTest]) values (2, 1, 1)

insert into [dbo].[AccountOperator] ([LegalPartyId], [LegalContractCounterPartyId]) values (1, 1) 
insert into [dbo].[AccountOperator] ([LegalPartyId], [LegalContractCounterPartyId]) values (2, 2)

insert into [dbo].[ClientType] ([ClientTypeId]) values (1)
insert into [dbo].[ClientType] ([ClientTypeId]) values (2)

insert into [dbo].[ClientAccount] ([ClientAccountId], [ClientTypeId]) values (1,1)
insert into [dbo].[ClientAccount] ([ClientAccountId], [ClientTypeId]) values (2,2)

insert into [dbo].[TradingAccount] ([ClientAccountId], [LedgerId]) values(1,1)
insert into [dbo].[TradingAccount] ([ClientAccountId], [LedgerId]) values(2,2)

INSERT INTO [dbo].[FxRate] ([BaseCurrencyId], [TermsCurrencyId], [EndOfDayRate]) VALUES (1, 11, CAST(0.7899000 AS Decimal(18, 7)))
INSERT INTO [dbo].[FxRate] ([BaseCurrencyId], [TermsCurrencyId], [EndOfDayRate]) VALUES (4, 11, CAST(1.1886000 AS Decimal(18, 7)))
INSERT INTO [dbo].[FxRate] ([BaseCurrencyId], [TermsCurrencyId], [EndOfDayRate]) VALUES (6, 11, CAST(1.5000000 AS Decimal(18, 7)))
INSERT INTO [dbo].[FxRate] ([BaseCurrencyId], [TermsCurrencyId], [EndOfDayRate]) VALUES (11, 24, CAST(2.0000000 AS Decimal(18, 7)))
INSERT INTO [dbo].[FxRate] ([BaseCurrencyId], [TermsCurrencyId], [EndOfDayRate]) VALUES (25, 11, CAST(0.5 AS Decimal(18, 7)))


-- not demo AO transactions, not hedge
--SELECT [t0].[LedgerTransactionId], [t0].[LedgerTransactionTypeId]
----, ltt.LedgerTransactionDesc
--, [t0].[LedgerTransactionDateTime], [t0].[Amount], [t0].[CurrencyId], c.IsoCurrency
--, [t0].[AccountOperatorId]
----, ao.AccountOperatorDesc
--, ctype.ClientTypeDesc
--FROM [dbo].[LedgerTransaction] AS [t0]
--inner join currency c on t0.currencyId = c.CurrencyId
--inner join ledgertransactiontype ltt on t0.LedgerTransactionTypeId = ltt.LedgerTransactionTypeId
--inner join accountoperator ao on [t0].AccountOperatorID = ao.LegalPartyId
--inner join LegalContractCounterParty lccp on lccp.LegalContractCounterPartyId = ao.LegalContractCounterPartyId
--inner join generalledger gl on [t0].ledgertransactionid = gl.ledgertransactionid
--inner join tradingaccount ta on gl.LedgerId = ta.LedgerId
--inner join clientaccount ca on ta.ClientAccountId = ca.ClientAccountId
--inner join ClientType ctype on ca.ClientTypeId = ctype.ClientTypeId
--WHERE [t0].[LedgerTransactionDateTime] >= '2017-10-18 00:00:00.000'
----and ltt.ledgertransactiontypeid in (82,83,84)
--and ltt.ledgertransactiontypeid in (63,26,28,269,82,83,84,115,230,231,25,27,29,102,62,103,39,234,236,273,275,11,270)
--and lccp.IsDemo <> 1 and lccp.IsTest <> 1
--and ctype.ClientTypeId <> 2
--ORDER BY [t0].[LedgerTransactionDateTime] DESC


ALTER DATABASE PaymentsData SET MULTI_USER