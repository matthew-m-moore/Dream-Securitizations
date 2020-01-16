USE CapitalMarkets
GO

IF (EXISTS (SELECT * 
				FROM SYS.DEFAULT_CONSTRAINTS
				WHERE OBJECT_ID = OBJECT_ID('dream.DF_ReserveAccountsDetail_TrancheCashFlowTypeId')))
BEGIN
	ALTER TABLE dream.ReserveAccountsDetail
		DROP CONSTRAINT DF_ReserveAccountsDetail_TrancheCashFlowTypeId
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_ReserveAccountsDetail_TrancheCashFlowType')))
BEGIN
	ALTER TABLE dream.ReserveAccountsDetail
		DROP CONSTRAINT FK_ReserveAccountsDetail_TrancheCashFlowType
END