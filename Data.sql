SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE [master];
GO

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


USE PaymentsData

-- Insert data into the Table1 table.
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId)
  VALUES(1, 82, '2017-07-07 13:20:50.857', 200.000000, 6);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId)
   VALUES(2, 82, '2017-07-07 13:23:33.760', 20.000000, 6);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId)
VALUES(3, 82, '2017-07-07 13:24:40.660', 100.000000, 6);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId)
VALUES(4, 115, '2017-07-07 13:25:10.760', 1122.000000, 6);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId)
VALUES(5, 115, '2017-07-07 13:25:38.953', 0.150000, 6);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId)
VALUES(6, 82, '2017-07-07 13:26:29.303', 2400.000000, 6);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId)
VALUES(7, 82, '2017-07-07 13:28:01.110', 50.000000, 24);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId)
VALUES(8, 82, '2017-07-07 13:29:55.207', 55.000000, 6);
INSERT INTO [LedgerTransaction] (LedgerTransactionId, LedgerTransactionTypeId, LedgerTransactionDateTime, Amount, CurrencyId)
VALUES(9, 82, '2017-07-07 13:30:06.757', 69.390000, 6);


--LedgerTransactionId	LedgerTransactionTypeId	LedgerTransactionDateTime	Amount	CurrencyId	AccountOperatorID	LedgerTransactionDesc
--1	82	2017-07-07 13:20:50.857	200.000000	6	2020	Card Deposit between Client and Card Control
--2	82	2017-07-07 13:23:33.760	20.000000	6	2020	Card Deposit between Client and Card Control
--3	82	2017-07-07 13:24:40.660	100.000000	6	400769674	Card Deposit between Client and Card Control
--4	115	2017-07-07 13:25:10.760	1122.000000	6	400769674	Card Refund from Client to Card Control
--5	115	2017-07-07 13:25:38.953	0.150000	6	2020	Card Refund from Client to Card Control
--6	82	2017-07-07 13:26:29.303	2400.000000	6	2020	Card Deposit between Client and Card Control
--7	82	2017-07-07 13:28:01.110	50.000000	24	2101	Card Deposit between Client and Card Control
--8	82	2017-07-07 13:29:55.207	55.000000	6	2020	Card Deposit between Client and Card Control
--9	82	2017-07-07 13:30:06.757	69.390000	6	2438	Card Deposit between Client and Card Control