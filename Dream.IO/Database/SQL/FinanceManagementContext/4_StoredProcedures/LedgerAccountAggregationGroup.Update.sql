SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.UpdateLedgerAccountAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.UpdateLedgerAccountAggregationGroup 
END
GO

CREATE PROCEDURE FinanceManagement.UpdateLedgerAccountAggregationGroup 
(
	@LedgerAccountAggregationGroupId int,
	@LedgerAccountAggregationGroupDescription varchar(250),
	@IsAggregationGroupActive bit
)
AS
BEGIN
	UPDATE FinanceManagement.LedgerAccountAggregationGroup
		 SET     UpdatedDate = GETDATE()
			   , UpdatedBy = SUSER_NAME()
			   , LedgerAccountAggregationGroupDescription = @LedgerAccountAggregationGroupDescription
			   , IsAggregationGroupActive = @IsAggregationGroupActive
		 WHERE LedgerAccountAggregationGroupId = @LedgerAccountAggregationGroupId
END

