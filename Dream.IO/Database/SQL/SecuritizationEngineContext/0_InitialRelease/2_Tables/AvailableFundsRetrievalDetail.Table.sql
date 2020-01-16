SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'AvailableFundsRetrievalDetail'))
BEGIN
    CREATE TABLE dream.AvailableFundsRetrievalDetail (
		AvailableFundsRetrievalDetailId int IDENTITY(1,1) NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		AvailableFundsRetrievalTypeId int NOT NULL,
		AvailableFundsRetrievalValue float NULL,
		AvailableFundsRetrievalInteger int NULL,
		AvailableFundsRetrievalDate date NULL,
	 CONSTRAINT PK_AvailableFundsRetrievalDetail PRIMARY KEY CLUSTERED 
		(
			AvailableFundsRetrievalDetailId ASC
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