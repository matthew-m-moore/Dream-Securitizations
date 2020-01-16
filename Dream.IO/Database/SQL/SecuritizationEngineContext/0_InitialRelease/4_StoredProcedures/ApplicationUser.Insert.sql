SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertApplicationUser')))
BEGIN
	DROP PROCEDURE dream.InsertApplicationUser 
END
GO

CREATE PROCEDURE dream.InsertApplicationUser 
(
	@NetworkUserNameIdentifier varchar(250),
	@ApplicationDisplayableNickName varchar(250),	
	@IsReadOnlyUser int
)
AS
BEGIN
	INSERT INTO dream.ApplicationUser
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , NetworkUserNameIdentifier
			   , ApplicationDisplayableNickName
			   , IsReadOnlyUser
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @NetworkUserNameIdentifier
			   , @ApplicationDisplayableNickName
			   , @IsReadOnlyUser
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS ApplicationUserId
END