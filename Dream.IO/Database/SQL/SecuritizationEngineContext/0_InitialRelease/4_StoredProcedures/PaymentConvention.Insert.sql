SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPaymentConvention')))
BEGIN
	DROP PROCEDURE dream.InsertPaymentConvention 
END
GO

CREATE PROCEDURE dream.InsertPaymentConvention 
(
	@PaymentConventionDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.PaymentConvention
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , PaymentConventionDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @PaymentConventionDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PaymentConventionId
END