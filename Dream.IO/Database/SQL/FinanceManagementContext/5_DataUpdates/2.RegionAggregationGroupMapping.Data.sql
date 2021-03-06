DECLARE @WorkDay int

EXEC FinanceManagement.InsertRegionAggregationGroup 'WorkDay', 1
SET @WorkDay = (SELECT MAX(RegionAggregationGroupId) FROM FinanceManagement.RegionAggregationGroup)

EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 14, 'AL'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 45, 'AK'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 1, 'AZ'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 15, 'AR'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 2, 'CA'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 16, 'CO'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 3, 'CT'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 4, 'CORPORATE'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 17, 'DE'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 5, 'DC'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 6, 'FL'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 43, 'GA'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 18, 'HI'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 46, 'ID'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 19, 'IL'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 20, 'IN'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 21, 'IA'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 7, 'KS'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 47, 'KY'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 22, 'LA'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 23, 'ME'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 24, 'MD'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 25, 'MA'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 26, 'MI'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 8, 'MN'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 48, 'MS'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 9, 'MO'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 27, 'MT'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 28, 'NE'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 29, 'NV'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 49, 'NH'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 50, 'NJ'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 51, 'NM'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 30, 'NY'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 10, 'NC'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 31, 'ND'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 32, 'OH'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 33, 'OK'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 34, 'OR'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 35, 'PA'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 36, 'RI'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 11, 'SC'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 37, 'SD'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 12, 'TN'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 13, 'TX'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 38, 'UT'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 39, 'VT'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 40, 'VA'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 41, 'WA'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 52, 'WV'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 42, 'WI'
EXEC FinanceManagement.InsertRegionAggregationGroupMapping @WorkDay, 53, 'WY'