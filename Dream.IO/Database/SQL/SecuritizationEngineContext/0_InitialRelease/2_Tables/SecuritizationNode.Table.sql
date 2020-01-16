SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationNode'))
BEGIN
    CREATE TABLE dream.SecuritizationNode (
		SecuritizationNodeId int IDENTITY(1,1) NOT NULL,
		SecuritizationNodeDataSetId int NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		SecuritizationNodeName varchar(150) NOT NULL,
		SecuritizationChildNodeName varchar(150) NULL,
		FundsDistributionTypeId int NOT NULL,
		TrancheDetailId int NULL,
		TranchePricingTypeId int NULL,
		TranchePricingRateIndexId int NULL,
		TranchePricingValue float NULL,
		TranchePricingDayCountConventionId int NULL,
		TranchePricingCompoundingConventionId int NULL,
	 CONSTRAINT PK_SecuritizationNode PRIMARY KEY CLUSTERED 
		(
			SecuritizationNodeId ASC
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