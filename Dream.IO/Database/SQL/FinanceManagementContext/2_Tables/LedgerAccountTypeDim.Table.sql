SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'LedgerAccountTypeDim'))
BEGIN
    ALTER TABLE FinanceManagement.LedgerAccountTypeDim
		ALTER COLUMN LedgerAccountType varchar(50)
END