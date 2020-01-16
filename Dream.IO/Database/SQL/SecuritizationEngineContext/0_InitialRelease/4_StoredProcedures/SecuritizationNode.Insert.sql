SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationNode')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationNode 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationNode 
(
	@SecuritizationNodeDataSetId int,
	@SecuritizationNodeName varchar(150),
	@SecuritizationChildNodeName varchar(150),
	@FundsDistributionTypeId int,
	@TrancheDetailId int,
	@TranchePricingTypeId int,
	@TranchePricingRateIndexId int,
	@TranchePricingValue float,
	@TranchePricingDayCountConventionId int,
	@TranchePricingCompoundingConventionId int
)
AS
BEGIN
	INSERT INTO dream.SecuritizationNode
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , SecuritizationNodeDataSetId
			   , SecuritizationNodeName
			   , SecuritizationChildNodeName
			   , FundsDistributionTypeId
			   , TrancheDetailId
			   , TranchePricingTypeId
			   , TranchePricingRateIndexId
			   , TranchePricingValue
			   , TranchePricingDayCountConventionId
			   , TranchePricingCompoundingConventionId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @SecuritizationNodeDataSetId
			   , @SecuritizationNodeName
			   , @SecuritizationChildNodeName
			   , @FundsDistributionTypeId
			   , @TrancheDetailId
			   , @TranchePricingTypeId
			   , @TranchePricingRateIndexId
			   , @TranchePricingValue
			   , @TranchePricingDayCountConventionId
			   , @TranchePricingCompoundingConventionId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationNodeId
END