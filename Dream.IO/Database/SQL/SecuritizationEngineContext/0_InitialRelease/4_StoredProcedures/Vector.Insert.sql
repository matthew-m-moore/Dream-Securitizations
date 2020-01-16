SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertVector')))
BEGIN
	DROP PROCEDURE dream.InsertVector 
END
GO

CREATE PROCEDURE dream.InsertVector 
(
	@VectorParentId int,
	@VectorPeriod int,
	@VectorValue float
)
AS
BEGIN
	INSERT INTO dream.Vector
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , VectorParentId
			   , VectorPeriod
			   , VectorValue
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @VectorParentId
			   , @VectorPeriod
			   , @VectorValue
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS VectorId
END