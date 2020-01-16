SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationAnalysis'))
BEGIN
    CREATE TABLE dream.SecuritizationAnalysis (
		SecuritizationAnalysisId int IDENTITY(1,1) NOT NULL,
		SecuritizationAnalysisDataSetId int NOT NULL,
		SecuritizationAnalysisVersionId int NOT NULL,
		SecuritizationAnalysisScenarioId int NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		SecuritizationInputTypeId int NOT NULL,
		SecuritizationInputTypeDataSetId int NOT NULL,
	 CONSTRAINT PK_SecuritizationAnalysis PRIMARY KEY CLUSTERED 
		(
			SecuritizationAnalysisId ASC
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