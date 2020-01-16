SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'FeeGroupDetail'))
BEGIN
    CREATE TABLE dream.FeeGroupDetail (
		FeeGroupDetailId int IDENTITY(1,1) NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		FeeGroupName varchar(150) NOT NULL,
		FeeRate float NULL,
		FeePerUnit float NULL,
		FeeMinimum float NULL,
		FeeMaximum float NULL,
		FeeIncreaseRate float NULL,
		FeeRateUpdateFrequencyInMonths int NULL,
		FeeRollingAverageInMonths int NULL,
		UseStartingBalanceToDetermineFee bit NULL,
	 CONSTRAINT PK_FeeGroupDetail PRIMARY KEY CLUSTERED 
		(
			FeeGroupDetailId ASC
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