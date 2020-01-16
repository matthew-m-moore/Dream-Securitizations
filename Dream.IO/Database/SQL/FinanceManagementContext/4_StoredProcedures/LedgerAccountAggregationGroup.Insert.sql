SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertLedgerAccountAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertLedgerAccountAggregationGroup 
END
GO

CREATE PROCEDURE FinanceManagement.InsertLedgerAccountAggregationGroup 
(
	@LedgerAccountAggregationGroupDescription varchar(250),
	@IsAggregationGroupActive bit
)
AS
BEGIN
	INSERT INTO FinanceManagement.LedgerAccountAggregationGroup
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , LedgerAccountAggregationGroupDescription
			   , IsAggregationGroupActive
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @LedgerAccountAggregationGroupDescription
			   , @IsAggregationGroupActive
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS LedgerAccountAggregationGroupId
END


