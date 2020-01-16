SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'CollateralizedSecuritizationTranche'))
BEGIN
    CREATE TABLE dream.CollateralizedSecuritizationTranche (
		CollateralizedSecuritizationTrancheId int IDENTITY(1,1) NOT NULL,
		CollateralizedSecuritizationDataSetId int NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		SecuritizationAnalysisDataSetId int NOT NULL,
		SecuritizationAnalysisVersionId int NOT NULL,
		SecuritizatizedTrancheDetailId int NOT NULL,
		SecuritizatizedTranchePercentage float NOT NULL,
	 CONSTRAINT PK_CollateralizedSecuritizationTranche PRIMARY KEY CLUSTERED 
		(
			CollateralizedSecuritizationTrancheId ASC
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