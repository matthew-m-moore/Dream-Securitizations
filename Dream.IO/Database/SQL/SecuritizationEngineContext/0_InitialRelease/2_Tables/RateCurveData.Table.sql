SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RateCurveData'))
BEGIN
    CREATE TABLE dream.RateCurveData (
		RateCurveDataId int IDENTITY(1,1) NOT NULL,
		RateCurveDataSetId int NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		MarketDateTime datetime NOT NULL,
		ForwardDateTime datetime NOT NULL,
		RateCurveValue float NOT NULL,
		RateIndexId int NOT NULL,
		MarketDataTypeId int NOT NULL,
	 CONSTRAINT PK_RateCurveData PRIMARY KEY CLUSTERED 
		(
			RateCurveDataId ASC
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