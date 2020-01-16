SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'FeeDetail'))
BEGIN
    CREATE TABLE dream.FeeDetail (
		FeeDetailId int IDENTITY(1,1) NOT NULL,
		FeeGroupDetailId int NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		FeeName varchar(150) NOT NULL,
		FeeAssociatedTrancheDetailId int NULL,
		FeeAmount float NULL,
		FeeEffectiveDate date NULL,
		IsIncreasingFee bit NULL
	 CONSTRAINT PK_FeeDetail PRIMARY KEY CLUSTERED 
		(
			FeeDetailId ASC
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