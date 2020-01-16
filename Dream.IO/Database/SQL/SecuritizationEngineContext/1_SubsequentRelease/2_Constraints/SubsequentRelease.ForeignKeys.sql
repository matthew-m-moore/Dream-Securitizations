USE CapitalMarkets
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_ReserveAccountsDetail_TrancheCashFlowType')))
BEGIN
	ALTER TABLE dream.ReserveAccountsDetail
		ADD CONSTRAINT FK_ReserveAccountsDetail_TrancheCashFlowType
			FOREIGN KEY (TrancheCashFlowTypeId) REFERENCES dream.TrancheCashFlowType(TrancheCashFlowTypeId)
END
