USE RAHERODW
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertCostCenterAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertCostCenterAggregationGroup 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.UpdateCostCenterAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.UpdateCostCenterAggregationGroup 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertCostCenterAggregationGroupMapping')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertCostCenterAggregationGroupMapping 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertLedgerAccountAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertLedgerAccountAggregationGroup 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.UpdateLedgerAccountAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.UpdateLedgerAccountAggregationGroup 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertLedgerAccountAggregationGroupMapping')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertLedgerAccountAggregationGroupMapping 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertProductLineAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertProductLineAggregationGroup 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.UpdateProductLineAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.UpdateProductLineAggregationGroup 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertProductLineAggregationGroupMapping')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertProductLineAggregationGroupMapping 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertRegionAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertRegionAggregationGroup 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.UpdateRegionAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.UpdateRegionAggregationGroup 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertRegionAggregationGroupMapping')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertRegionAggregationGroupMapping 
END