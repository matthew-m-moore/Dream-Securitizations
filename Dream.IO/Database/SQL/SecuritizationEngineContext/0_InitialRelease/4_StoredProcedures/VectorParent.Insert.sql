SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertVectorParent')))
BEGIN
	DROP PROCEDURE dream.InsertVectorParent 
END
GO

CREATE PROCEDURE dream.InsertVectorParent 
(
	@CutOffDate date,
	@IsFlatVector bit,
	@VectorParentDescription varchar(250)	
)
AS
BEGIN
	INSERT INTO dream.VectorParent
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , IsFlatVector
			   , VectorParentDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @IsFlatVector
			   , @VectorParentDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS VectorParentId
END