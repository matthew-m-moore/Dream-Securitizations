SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertLedgerAccountAggregationGroupMapping')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertLedgerAccountAggregationGroupMapping 
END
GO

CREATE PROCEDURE FinanceManagement.InsertLedgerAccountAggregationGroupMapping 
(
	@LedgerAccountAggregationGroupId int,
	@LedgerAccountKey int,
	@LedgerAccountAggregationGroupIdentifier varchar(250)
)
AS
BEGIN
	INSERT INTO FinanceManagement.LedgerAccountAggregationGroupMapping
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , LedgerAccountAggregationGroupId
			   , LedgerAccountKey
			   , LedgerAccountAggregationGroupIdentifier
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @LedgerAccountAggregationGroupId
			   , @LedgerAccountKey
			   , @LedgerAccountAggregationGroupIdentifier
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS LedgerAccountAggregationGroupMappingId
END


