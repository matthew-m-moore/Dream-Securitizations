USE RAHERODW
GO

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_CostCenterAggregationGroupMapping_CostCenterDim')))
BEGIN
	ALTER TABLE FinanceManagement.CostCenterAggregationGroupMapping
		ADD CONSTRAINT FK_CostCenterAggregationGroupMapping_CostCenterDim
			FOREIGN KEY (CostCenterKey) REFERENCES FinanceManagement.CostCenterDim(CostCenterKey)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_LedgerAccountAggregationGroupMapping_LedgerAccountDim')))
BEGIN
	ALTER TABLE FinanceManagement.LedgerAccountAggregationGroupMapping
		ADD CONSTRAINT FK_LedgerAccountAggregationGroupMapping_LedgerAccountDim
			FOREIGN KEY (LedgerAccountKey) REFERENCES FinanceManagement.LedgerAccountDim(LedgerAccountKey)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_ProductLineAggregationGroupMapping_ProductLineDim')))
BEGIN
	ALTER TABLE FinanceManagement.ProductLineAggregationGroupMapping
		ADD CONSTRAINT FK_ProductLineAggregationGroupMapping_ProductLineDim
			FOREIGN KEY (ProductLineKey) REFERENCES FinanceManagement.ProductLineDim(ProductLineKey)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_RegionAggregationGroupMapping_RegionDim')))
BEGIN
	ALTER TABLE FinanceManagement.RegionAggregationGroupMapping
		ADD CONSTRAINT FK_RegionAggregationGroupMapping_RegionDim
			FOREIGN KEY (RegionKey) REFERENCES FinanceManagement.RegionDim(RegionKey)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_CostCenterAggregationGroupMapping_CostCenterAggregationGroup')))
BEGIN
	ALTER TABLE FinanceManagement.CostCenterAggregationGroupMapping
		ADD CONSTRAINT FK_CostCenterAggregationGroupMapping_CostCenterAggregationGroup
			FOREIGN KEY (CostCenterAggregationGroupId) REFERENCES FinanceManagement.CostCenterAggregationGroup(CostCenterAggregationGroupId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_LedgerAccountAggregationGroupMapping_LedgerAccountAggregationGroup')))
BEGIN
	ALTER TABLE FinanceManagement.LedgerAccountAggregationGroupMapping
		ADD CONSTRAINT FK_LedgerAccountAggregationGroupMapping_LedgerAccountAggregationGroup
			FOREIGN KEY (LedgerAccountAggregationGroupId) REFERENCES FinanceManagement.LedgerAccountAggregationGroup(LedgerAccountAggregationGroupId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_ProductLineAggregationGroupMapping_ProductLineAggregationGroup')))
BEGIN
	ALTER TABLE FinanceManagement.ProductLineAggregationGroupMapping
		ADD CONSTRAINT FK_ProductLineAggregationGroupMapping_ProductLineAggregationGroup
			FOREIGN KEY (ProductLineAggregationGroupId) REFERENCES FinanceManagement.ProductLineAggregationGroup(ProductLineAggregationGroupId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_RegionAggregationGroupMapping_RegionAggregationGroup')))
BEGIN
	ALTER TABLE FinanceManagement.RegionAggregationGroupMapping
		ADD CONSTRAINT FK_RegionAggregationGroupMapping_RegionAggregationGroup
			FOREIGN KEY (RegionAggregationGroupId) REFERENCES FinanceManagement.RegionAggregationGroup(RegionAggregationGroupId)
END