SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationAnalysisComment'))
BEGIN
    CREATE TABLE dream.SecuritizationAnalysisComment (
		SecuritizationAnalysisCommentId int IDENTITY(1, 1) NOT NULL,
		SecuritizationAnalysisDataSetId int NOT NULL,
		SecuritizationAnalysisVersionId int NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		IsVisible bit NOT NULL,
		CommentText varchar(max) NOT NULL,
	 CONSTRAINT PK_SecuritizationAnalysisComment PRIMARY KEY CLUSTERED 
		(
			SecuritizationAnalysisCommentId ASC
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