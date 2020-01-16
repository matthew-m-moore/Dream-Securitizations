SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'TrancheType'))
BEGIN
    CREATE TABLE dream.TrancheType (
		TrancheTypeId int IDENTITY(1,1) NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		TrancheTypeDescription varchar(250) NOT NULL,
		IsVisible bit NOT NULL,
		IsFeeTranche bit NOT NULL,
		IsReserveTranche bit NOT NULL,
		IsInterestPayingTranche bit NULL,
	 CONSTRAINT PK_TrancheType PRIMARY KEY CLUSTERED 
		(
			TrancheTypeId ASC
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