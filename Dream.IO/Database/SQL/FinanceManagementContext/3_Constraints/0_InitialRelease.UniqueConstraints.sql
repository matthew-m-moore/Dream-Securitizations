USE RAHERODW
GO

SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT *
				FROM SYS.INDEXES
				WHERE NAME = 'UIDX_CostCenterAggregationGroup'))
BEGIN
	CREATE UNIQUE INDEX UIDX_CostCenterAggregationGroup
	ON FinanceManagement.CostCenterAggregationGroup(CostCenterAggregationGroupDescription)
	WHERE IsAggregationGroupActive = 1
END

IF (NOT EXISTS (SELECT *
				FROM SYS.INDEXES
				WHERE NAME = 'UIDX_LedgerAccountAggregationGroup'))
BEGIN
	CREATE UNIQUE INDEX UIDX_LedgerAccountAggregationGroup
	ON FinanceManagement.LedgerAccountAggregationGroup(LedgerAccountAggregationGroupDescription)
	WHERE IsAggregationGroupActive = 1
END

IF (NOT EXISTS (SELECT *
				FROM SYS.INDEXES
				WHERE NAME = 'UIDX_ProductLineAggregationGroup'))
BEGIN
	CREATE UNIQUE INDEX UIDX_ProductLineAggregationGroup
	ON FinanceManagement.ProductLineAggregationGroup(ProductLineAggregationGroupDescription)
	WHERE IsAggregationGroupActive = 1
END

IF (NOT EXISTS (SELECT *
				FROM SYS.INDEXES
				WHERE NAME = 'UIDX_RegionAggregationGroup'))
BEGIN
	CREATE UNIQUE INDEX UIDX_RegionAggregationGroup
	ON FinanceManagement.RegionAggregationGroup(RegionAggregationGroupDescription)
	WHERE IsAggregationGroupActive = 1
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'FinanceManagement'
				  AND CONSTRAINT_NAME = 'UC_CostCenterAggregationGroupMapping_CostCenterAggregationGroupId_CostCenterKey'))
BEGIN
	ALTER TABLE FinanceManagement.CostCenterAggregationGroupMapping
		ADD CONSTRAINT UC_CostCenterAggregationGroupMapping_CostCenterAggregationGroupId_CostCenterKey
			UNIQUE (CostCenterAggregationGroupId, CostCenterKey)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'FinanceManagement'
				  AND CONSTRAINT_NAME = 'UC_LedgerAccountAggregationGroupMapping_LedgerAccountAggregationGroupId_LedgerAccountKey'))
BEGIN
	ALTER TABLE FinanceManagement.LedgerAccountAggregationGroupMapping
		ADD CONSTRAINT UC_LedgerAccountAggregationGroupMapping_LedgerAccountAggregationGroupId_LedgerAccountKey
			UNIQUE (LedgerAccountAggregationGroupId, LedgerAccountKey)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'FinanceManagement'
				  AND CONSTRAINT_NAME = 'UC_ProductLineAggregationGroupMapping_ProductLineAggregationGroupId_ProductLineKey'))
BEGIN
	ALTER TABLE FinanceManagement.ProductLineAggregationGroupMapping
		ADD CONSTRAINT UC_ProductLineAggregationGroupMapping_ProductLineAggregationGroupId_ProductLineKey
			UNIQUE (ProductLineAggregationGroupId, ProductLineKey)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'FinanceManagement'
				  AND CONSTRAINT_NAME = 'UC_RegionAggregationGroupMapping_RegionAggregationGroupId_RegionKey'))
BEGIN
	ALTER TABLE FinanceManagement.RegionAggregationGroupMapping
		ADD CONSTRAINT UC_RegionAggregationGroupMapping_RegionAggregationGroupId_RegionKey
			UNIQUE (RegionAggregationGroupId, RegionKey)
END