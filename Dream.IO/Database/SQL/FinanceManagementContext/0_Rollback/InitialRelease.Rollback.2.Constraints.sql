USE RAHERODW
GO

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_CostCenterAggregationGroupMapping_CostCenterDim')))
BEGIN
	ALTER TABLE FinanceManagement.CostCenterAggregationGroupMapping
		DROP CONSTRAINT FK_CostCenterAggregationGroupMapping_CostCenterDim
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_LedgerAccountAggregationGroupMapping_LedgerAccountDim')))
BEGIN
	ALTER TABLE FinanceManagement.LedgerAccountAggregationGroupMapping
		DROP CONSTRAINT FK_LedgerAccountAggregationGroupMapping_LedgerAccountDim
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_ProductLineAggregationGroupMapping_ProductLineDim')))
BEGIN
	ALTER TABLE FinanceManagement.ProductLineAggregationGroupMapping
		DROP CONSTRAINT FK_ProductLineAggregationGroupMapping_ProductLineDim
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_RegionAggregationGroupMapping_RegionDim')))
BEGIN
	ALTER TABLE FinanceManagement.RegionAggregationGroupMapping
		DROP CONSTRAINT FK_RegionAggregationGroupMapping_RegionDim
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_CostCenterAggregationGroupMapping_CostCenterAggregationGroup')))
BEGIN
	ALTER TABLE FinanceManagement.CostCenterAggregationGroupMapping
		DROP CONSTRAINT FK_CostCenterAggregationGroupMapping_CostCenterAggregationGroup
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_LedgerAccountAggregationGroupMapping_LedgerAccountAggregationGroup')))
BEGIN
	ALTER TABLE FinanceManagement.LedgerAccountAggregationGroupMapping
		DROP CONSTRAINT FK_LedgerAccountAggregationGroupMapping_LedgerAccountAggregationGroup
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_ProductLineAggregationGroupMapping_ProductLineAggregationGroup')))
BEGIN
	ALTER TABLE FinanceManagement.ProductLineAggregationGroupMapping
		DROP CONSTRAINT FK_ProductLineAggregationGroupMapping_ProductLineAggregationGroup
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('FinanceManagement.FK_RegionAggregationGroupMapping_RegionAggregationGroup')))
BEGIN
	ALTER TABLE FinanceManagement.RegionAggregationGroupMapping
		DROP CONSTRAINT FK_RegionAggregationGroupMapping_RegionAggregationGroup
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'FinanceManagement'
				  AND CONSTRAINT_NAME = 'UC_CostCenterAggregationGroupMapping_CostCenterAggregationGroupId_CostCenterKey'))
BEGIN
	ALTER TABLE FinanceManagement.CostCenterAggregationGroupMapping
		DROP CONSTRAINT UC_CostCenterAggregationGroupMapping_CostCenterAggregationGroupId_CostCenterKey
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'FinanceManagement'
				  AND CONSTRAINT_NAME = 'UC_LedgerAccountAggregationGroupMapping_LedgerAccountAggregationGroupId_LedgerAccountKey'))
BEGIN
	ALTER TABLE FinanceManagement.LedgerAccountAggregationGroupMapping
		DROP CONSTRAINT UC_LedgerAccountAggregationGroupMapping_LedgerAccountAggregationGroupId_LedgerAccountKey
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'FinanceManagement'
				  AND CONSTRAINT_NAME = 'UC_ProductLineAggregationGroupMapping_ProductLineAggregationGroupId_ProductLineKey'))
BEGIN
	ALTER TABLE FinanceManagement.ProductLineAggregationGroupMapping
		DROP CONSTRAINT UC_ProductLineAggregationGroupMapping_ProductLineAggregationGroupId_ProductLineKey
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'FinanceManagement'
				  AND CONSTRAINT_NAME = 'UC_RegionAggregationGroupMapping_RegionAggregationGroupId_RegionKey'))
BEGIN
	ALTER TABLE FinanceManagement.RegionAggregationGroupMapping
		DROP CONSTRAINT UC_RegionAggregationGroupMapping_RegionAggregationGroupId_RegionKey
END