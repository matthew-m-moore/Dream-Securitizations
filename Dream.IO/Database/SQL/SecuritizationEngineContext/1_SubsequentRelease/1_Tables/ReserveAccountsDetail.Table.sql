SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF COL_LENGTH('dream.ReserveAccountsDetail', 'TrancheCashFlowTypeId') IS NULL
BEGIN
    ALTER TABLE dream.ReserveAccountsDetail
		ADD TrancheCashFlowTypeId int NOT NULL
		CONSTRAINT DF_ReserveAccountsDetail_TrancheCashFlowTypeId DEFAULT 1
END