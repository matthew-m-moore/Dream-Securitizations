USE RAHERODW
GO

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'AccountMapping'))
BEGIN
	DROP TABLE FinanceManagement.AccountMapping
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'CostCenterMapping'))
BEGIN
	DROP TABLE FinanceManagement.CostCenterMapping
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'ProductLineMapping'))
BEGIN
	DROP TABLE FinanceManagement.ProductLineMapping
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'CostCenterAggregationGroup'))
BEGIN
	DROP TABLE FinanceManagement.CostCenterAggregationGroup
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'CostCenterAggregationGroupMapping'))
BEGIN
	DROP TABLE FinanceManagement.CostCenterAggregationGroupMapping
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'LedgerAccountAggregationGroup'))
BEGIN
	DROP TABLE FinanceManagement.LedgerAccountAggregationGroup
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'LedgerAccountAggregationGroupMapping'))
BEGIN
	DROP TABLE FinanceManagement.LedgerAccountAggregationGroupMapping
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'ProductLineAggregationGroup'))
BEGIN
	DROP TABLE FinanceManagement.ProductLineAggregationGroup
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'ProductLineAggregationGroupMapping'))
BEGIN
	DROP TABLE FinanceManagement.ProductLineAggregationGroupMapping
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'RegionAggregationGroup'))
BEGIN
	DROP TABLE FinanceManagement.RegionAggregationGroup
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'RegionAggregationGroupMapping'))
BEGIN
	DROP TABLE FinanceManagement.RegionAggregationGroupMapping
END