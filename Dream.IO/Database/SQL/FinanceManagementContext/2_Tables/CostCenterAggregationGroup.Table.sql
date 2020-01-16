SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'FinanceManagement' 
                 AND  TABLE_NAME = 'CostCenterAggregationGroup'))
BEGIN
    CREATE TABLE FinanceManagement.CostCenterAggregationGroup (
		CostCenterAggregationGroupId int IDENTITY(1,1) NOT NULL,
		CostCenterAggregationGroupDescription varchar(250) NOT NULL,
		IsAggregationGroupActive bit NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
	 CONSTRAINT PK_CostCenterAggregationGroup PRIMARY KEY CLUSTERED 
		(
			CostCenterAggregationGroupId ASC
		)
			WITH 
			(
				PAD_INDEX = OFF, 
				STATISTICS_NORECOMPUTE = OFF, 
				IGNORE_DUP_KEY = OFF, 
				ALLOW_ROW_LOCKS = ON, 
				ALLOW_PAGE_LOCKS = ON
			)
		) ON [PRIMARY]
END