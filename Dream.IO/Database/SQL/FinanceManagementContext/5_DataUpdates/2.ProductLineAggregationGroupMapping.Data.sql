DECLARE @Division int
DECLARE @WorkDay int

EXEC FinanceManagement.InsertProductLineAggregationGroup 'Division', 1
SET @Division = (SELECT MAX(ProductLineAggregationGroupId) FROM FinanceManagement.ProductLineAggregationGroup)

EXEC FinanceManagement.InsertProductLineAggregationGroup 'WorkDay', 1
SET @WorkDay = (SELECT MAX(ProductLineAggregationGroupId) FROM FinanceManagement.ProductLineAggregationGroup)

EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @Division, 1, 'HERO'
EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @Division, 2, 'Benji'
EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @Division, 3, 'Corporate'
EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @Division, 4, 'HERO'
EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @Division, 6, 'HERO'
EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @Division, 5, 'Benji'

EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @WorkDay, 1, 'PRODUCT_HERO'
EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @WorkDay, 2, 'PRODUCT_BENJI'
EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @WorkDay, 3, 'PRODUCT_CORPORATE'
EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @WorkDay, 4, 'PRODUCT_HERO'
EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @WorkDay, 6, 'PRODUCT_HERO'
EXEC FinanceManagement.InsertProductLineAggregationGroupMapping @WorkDay, 5, 'PRODUCT_BENJI'