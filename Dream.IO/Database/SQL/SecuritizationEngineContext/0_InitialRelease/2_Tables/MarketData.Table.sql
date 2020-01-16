SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'MarketData'))
BEGIN
    CREATE TABLE dream.MarketData (
		MarketDataId int IDENTITY(1,1) NOT NULL,
		MarketDataSetId int NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		MarketDateTime datetime NOT NULL,
		DataValue float NOT NULL,
		RateIndexId int NOT NULL,
		MarketDataTypeId int NOT NULL,
	 CONSTRAINT PK_MarketData PRIMARY KEY CLUSTERED 
		(
			MarketDataId ASC
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