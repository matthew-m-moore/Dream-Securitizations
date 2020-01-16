SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'AggregationGroupAssignment'))
BEGIN
    CREATE TABLE dream.AggregationGroupAssignment (
		AggregationGroupAssignmentId int IDENTITY(1,1) NOT NULL,
		AggregationGroupDataSetId int NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		InstrumentIdentifier varchar(250) NOT NULL,
		AggregationGroupingIdentifier varchar(250) NOT NULL,
		AggregationGroupName varchar(250) NOT NULL,	
	 CONSTRAINT PK_AggregationGroupAssignment PRIMARY KEY CLUSTERED 
		(
			AggregationGroupAssignmentId ASC
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