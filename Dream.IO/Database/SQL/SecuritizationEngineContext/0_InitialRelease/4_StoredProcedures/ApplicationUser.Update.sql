SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.UpdateApplicationUser')))
BEGIN
	DROP PROCEDURE dream.UpdateApplicationUser 
END
GO

CREATE PROCEDURE dream.UpdateApplicationUser 
(
	@ApplicationUserId int,
	@ApplicationDisplayableNickName varchar(250),
	@IsReadOnlyUser bit
)
AS
BEGIN
	UPDATE dream.ApplicationUser
		 SET   UpdatedDate = GETDATE()
		 	 , UpdatedBy = SUSER_NAME()
			 , ApplicationDisplayableNickName = @ApplicationDisplayableNickName
			 , IsReadOnlyUser = @IsReadOnlyUser
		 WHERE ApplicationUserId = @ApplicationUserId
END