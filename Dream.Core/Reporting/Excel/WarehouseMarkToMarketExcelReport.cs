using ClosedXML.Excel;
using Dream.Common;
using Dream.Common.Enums;
using Dream.Common.ExtensionMethods;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Stratifications;
using Dream.Core.Reporting.Results;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Dream.Core.Reporting.Excel
{
    public class WarehouseMarkToMarketExcelReport
    {
        private const string _reportTabName = "Warehouse MTM Summary";
        private const string _fontName = "Open Sans";

        private const string _accountingNumberFormat = @"_(* #,##0.00_);_(* (#,##0.00);_(* "" - ""??_);_(@_)";
        private const string _accountingNumberFormatNoDecimals = @"_(* #,##0_);_(* (#,##0);_(* "" - ""??_);_(@_)";
        private const string _accountingNumberFormatOneDecmial = @"_(* #,##0.0_);_(* (#,##0.0);_(* "" - ""??_);_(@_)";
        private const string _accountingNumberFormatThreeDecimals = @"_(* #,##0.000_);_(* (#,##0.000);_(* "" - ""??_);_(@_)";

        private const int _baseFontSize = 8;
        private const int _blankColumnsOffset = 2;
        private const int _blankRowsOffset = 2;

        private static List<string> _headersRowWithPricing = new List<string>
        {
            "Aggregation Group",
            "Bond Count",
            "Balance ($)",
            "Coupon (%)",
            "WAL (years)",
            "Life CPR (%)",
            "Benchmark Yield (%)",
            "Nominal Spread (bps)",
            "Yield (%)",
            "Market Value ($)",
            "Price (%)",
            "Wgt. Av. Buy-Down (%)",
            "Cash Outlay (%)",
            "Advance Rate (%)",
            "MTM Cushion (%)",
            "Minimum Cushion (%)",
            "Advance Rate Adj. (%)",
            "Agg. Advance Rate (%)",
            "Is Cushion Below Min?",
            "Day-Counting",
            "Compounding",
            "Nominal Spread Data",
        };

        public static void AddReportTab(
            XLWorkbook excelWorkbook, 
            Securitization securitization, 
            SecuritizationResult securitizationResult, 
            WarehouseMarkToMarketInput markToMarketInputs)
        {
            var paceAssessments = securitization.Collateral.OfType<PaceAssessment>().ToList();
            var aggregationGroupings = securitization.AggregationGroupings;
            var selectedAggregationGrouping = securitization.Inputs.SelectedAggregationGrouping;
            var dictionaryOfResults = securitizationResult.CollateralCashFlowsResultsDictionary;

            var pricingStrategy = new NominalSpreadBasedPricingStrategy(
                markToMarketInputs.DayCountConvention,
                markToMarketInputs.CompoundingConvention,
                securitizationResult.MarketRateEnvironment,
                markToMarketInputs.MarketDataForNominalSpread,
                markToMarketInputs.NominalSpread);

            SetupReportTab(
                excelWorkbook,
                selectedAggregationGrouping,
                pricingStrategy,
                markToMarketInputs,
                aggregationGroupings,
                paceAssessments,
                dictionaryOfResults);
        }

        private static void SetupReportTab(
            XLWorkbook excelWorkbook,
            string selectedAggregationGrouping,
            NominalSpreadBasedPricingStrategy pricingStrategy,
            WarehouseMarkToMarketInput markToMarketInputs,
            AggregationGroupings aggregationGroupings,
            List<PaceAssessment> paceAssessments,
            Dictionary<string, ProjectedCashFlowsSummaryResult> dictionaryOfresults)
        { 
            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);
            var rateEnvironment = pricingStrategy.MarketRateEnvironment;
            var marketDataGrouping = pricingStrategy.MarketDataGrouping;
        
            // First, assemble the header rows.
            foreach (var headerIndex in Enumerable.Range(0, _headersRowWithPricing.Count))
            {
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Value = _headersRowWithPricing[headerIndex];
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // The very first header always has a left alignment, instead of center
                if (headerIndex > 0)
                {
                    excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                else
                {
                    excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
            }

            // Loop through all the aggregation groupings
            var dataRowOffsetCounter = 1;
            foreach (var keyValuePair in dictionaryOfresults)
            {
                if (keyValuePair.Key == AggregationGroupings.TotalAggregationGroupName) continue;

                var collateralCashFlows = keyValuePair.Value.ProjectedCashFlows;
                var aggregationGroupIdentifier = keyValuePair.Key;
                var selectedPaceAssessments = paceAssessments
                    .Where(p => aggregationGroupings[p.StringId, selectedAggregationGrouping] == aggregationGroupIdentifier).ToList();

                AddReportLineWithPricing(
                    excelWorksheet,
                    pricingStrategy,
                    rateEnvironment,
                    marketDataGrouping,
                    markToMarketInputs,
                    selectedPaceAssessments,
                    collateralCashFlows,
                    aggregationGroupIdentifier,
                    dataRowOffsetCounter);

                dataRowOffsetCounter++;
            }

            // Always pick out the "Total" grouping and display it last, if it is present
            if (dictionaryOfresults.ContainsKey(AggregationGroupings.TotalAggregationGroupName))
            {
                var collateralCashFlows = dictionaryOfresults[AggregationGroupings.TotalAggregationGroupName].ProjectedCashFlows;
                var aggregationGroupIdentifier = AggregationGroupings.TotalAggregationGroupName;

                AddReportLineWithPricing(
                    excelWorksheet,
                    pricingStrategy,
                    rateEnvironment,
                    marketDataGrouping,
                    markToMarketInputs,
                    paceAssessments,
                    collateralCashFlows,
                    aggregationGroupIdentifier,
                    dataRowOffsetCounter);

                excelWorksheet.Row(_blankRowsOffset + dataRowOffsetCounter).Style.Font.Bold = true;
            }

            // Setup font name and size
            excelWorksheet.Cells().Style.Font.FontName = _fontName;
            excelWorksheet.Cells().Style.Font.FontSize = _baseFontSize + 1;
            excelWorksheet.Row(_blankRowsOffset).Style.Font.FontSize = _baseFontSize;
            excelWorksheet.Row(_blankRowsOffset).Height = 15;
            excelWorksheet.SheetView.Freeze(_blankRowsOffset, _blankColumnsOffset);
            excelWorksheet.SetTabColor(XLColor.White);

            // Auto-fit and adjust column-widths
            excelWorksheet.Columns().AdjustToContents();
            foreach (var columnIndex in Enumerable.Range(1, _blankColumnsOffset - 1))
            {
                excelWorksheet.Column(columnIndex).Width = 2;
            }
        }

        private static void AddReportLineWithPricing(
            IXLWorksheet excelWorksheet,
            PricingStrategy pricingStrategy,
            MarketRateEnvironment rateEnvironment,
            MarketDataGrouping marketDataGrouping,
            WarehouseMarkToMarketInput markToMarketInputs,
            List<PaceAssessment> paceAssessments,
            List<ProjectedCashFlow> collateralCashFlows,
            string aggregationGroupIdentifier,
            int dataRowOffsetCounter)
        {
            var dataColumnOffsetCounter = 0;
            var totalCollateralBalance = collateralCashFlows.First().StartingBalance;

            // Note, these values are easier to get calculated up front
            var presentValue = pricingStrategy.CalculatePresentValue(collateralCashFlows);
            var price = pricingStrategy.CalculatePrice(collateralCashFlows);
            var internalRateOfReturn = pricingStrategy.CalculateInternalRateOfReturn(collateralCashFlows);
            var nominalSpread = pricingStrategy.CalculateNominalSpread(collateralCashFlows, rateEnvironment, marketDataGrouping);

            // Aggregation Group
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = aggregationGroupIdentifier;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            dataColumnOffsetCounter++;

            // Bond Count
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = collateralCashFlows.First().BondCount;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatNoDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Balance ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalCollateralBalance;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Coupon (%)
            var forwardWeightedAverageCoupon = CashFlowMetrics.CalculateForwardWeightedAverageCoupon(collateralCashFlows);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * forwardWeightedAverageCoupon;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // WAL (years)
            var weightedAverageLife = CashFlowMetrics.CalculateWeightedAverageLife(collateralCashFlows);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = weightedAverageLife;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Life CPR (%)
            var lifetimeConstantPrepaymentRate = CashFlowMetrics.CalculateLifetimeConstantPrepaymentRate(collateralCashFlows);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * lifetimeConstantPrepaymentRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatOneDecmial;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Benchmark Yield (%)         
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * pricingStrategy.InterpolatedRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatThreeDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Nominal Spread (bps)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.BpsPerOneHundredPercentagePoints * nominalSpread;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatNoDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Yield (%)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * internalRateOfReturn;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatThreeDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Market Value ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = presentValue;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Price (%)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * price;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Wgt. Av. Buy-Down (%)
            var buyDownRates = paceAssessments.Select(p => p.RatePlan.BuyDownRate).ToList();
            var assessmentBalances = paceAssessments.Select(p => p.Balance).ToList();
            var weightedAverageBuyDown = MathUtility.WeightedAverage(assessmentBalances, buyDownRates);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * weightedAverageBuyDown;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Cash Outlay (%)
            var cashOutlayRate = 1.0 - weightedAverageBuyDown;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * cashOutlayRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Advance Rate (%)
            var advanceRate = cashOutlayRate * markToMarketInputs.AdvanceRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * advanceRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // MTM Cushion (%)
            var markToMarketCushion = price - advanceRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * markToMarketCushion;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Minimum Cushion (%)
            var minimumCushion = markToMarketInputs.MinimumCushion;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * minimumCushion;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Advance Rate Adj (%)
            var advanceRateAdjustment = Math.Min(0.0, markToMarketCushion - minimumCushion);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * advanceRateAdjustment;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Agg. Advance Rate (%)
            var aggregateAdvanceRate = advanceRate + advanceRateAdjustment;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * aggregateAdvanceRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Is Cushion Below Min?
            var isCushionBelowMinimum = markToMarketCushion < minimumCushion;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = isCushionBelowMinimum.ToString().ToUpper();
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Day-Counting
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = pricingStrategy.DayCountConvention.GetFriendlyDescription();
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Compounding
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = pricingStrategy.CompoundingConvention.GetFriendlyDescription();
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Nominal Spread Data
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = pricingStrategy.MarketDataUsedForNominalSpread.GetFriendlyDescription();
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            pricingStrategy.ClearCachedValues();
        }
    }
}
