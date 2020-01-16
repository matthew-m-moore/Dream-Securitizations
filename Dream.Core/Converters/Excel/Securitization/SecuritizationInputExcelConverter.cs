using Dream.Common;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System.Collections.Generic;
using System;

namespace Dream.Core.Converters.Excel.Securitization
{
    public class SecuritizationInputExcelConverter
    {
        /// <summary>
        /// Converts a list of securitization input records from Excel into a list of securitization input business objects.
        /// </summary>
        public static List<SecuritizationInput> ConvertListOfSecuritizationInputsRecords(List<SecuritizationInputsRecord> securitizationInputsRecords)
        {
            var listOfSecuritizationInputs = new List<SecuritizationInput>();
            foreach (var securitizationInputsRecord in securitizationInputsRecords)
            {
                if (securitizationInputsRecord.RunScenario.HasValue && !securitizationInputsRecord.RunScenario.Value) continue;
                var securitizationInput = ConvertSecuritizationInputsRecord(securitizationInputsRecord);
                listOfSecuritizationInputs.Add(securitizationInput);
            }

            return listOfSecuritizationInputs;
        }

        /// <summary>
        /// Converts a warehouse mark-to-market input record into securitization input and warehouse mark-to-market input business objects.
        /// </summary>
        public static SecuritizationInput ConvertWarehouseMarkToMarketInputsRecord(WarehouseMarkToMarketInputsRecord warehouseMarkToMarketInputsRecord,
            out WarehouseMarkToMarketInput warehouseMarkToMarketInput)
        {
            var marketDataGrouping = (MarketDataGrouping) Enum.Parse(typeof(MarketDataGrouping), warehouseMarkToMarketInputsRecord.MarketDataForSpread);
            var dayCountConvention = (DayCountConvention) Enum.Parse(typeof(DayCountConvention), warehouseMarkToMarketInputsRecord.DayCountConvention);
            var compoundingConvention = (CompoundingConvention) Enum.Parse(typeof(CompoundingConvention), warehouseMarkToMarketInputsRecord.CompoundingConvention);

            warehouseMarkToMarketInput = new WarehouseMarkToMarketInput
            {
                AdvanceRate = warehouseMarkToMarketInputsRecord.AdvanceRate,
                MinimumCushion = warehouseMarkToMarketInputsRecord.MinimumCushion,

                NominalSpread = (double) warehouseMarkToMarketInputsRecord.NominalSpreadInBps / Constants.BpsPerOneHundredPercentagePoints,
                MarketDataForNominalSpread = marketDataGrouping,
                DayCountConvention = dayCountConvention,
                CompoundingConvention = compoundingConvention
            };

            var securitizationInput = new SecuritizationInput
            {
                CollateralCutOffDate = warehouseMarkToMarketInputsRecord.RunDate,
                CashFlowStartDate = warehouseMarkToMarketInputsRecord.RunDate,
                InterestAccrualStartDate = warehouseMarkToMarketInputsRecord.RunDate,

                SelectedAggregationGrouping = warehouseMarkToMarketInputsRecord.AggregationGrouping,
                SelectedPerformanceAssumption = warehouseMarkToMarketInputsRecord.PerformanceAssumption,

                CashFlowPricingStrategy = new DoNothingPricingStrategy(),
            };

            return securitizationInput;
        }

        /// <summary>
        /// Converts a DBRS stress model input record into securitization input and DBRS stress model input business objects.
        /// </summary>
        public static List<SecuritizationInput> ConvertDbrsStressModelInputsRecord(List<DbrsStressModelInputsRecord> dbrsStressModelInputRecords, 
            out Dictionary<string, DbrsStressModelInput> dbrsStressModelInputsDictionary)
        {
            var listOfSecuritizationInputs = new List<SecuritizationInput>();
            var dictionaryOfDbrsStressModelInputs = new Dictionary<string, DbrsStressModelInput>();

            foreach (var dbrsStressModelInputsRecord in dbrsStressModelInputRecords)
            {
                if (dbrsStressModelInputsRecord.RunScenario.HasValue && !dbrsStressModelInputsRecord.RunScenario.Value) continue;

                var securitizationInput = ConvertSecuritizationInputsRecord(dbrsStressModelInputsRecord);
                listOfSecuritizationInputs.Add(securitizationInput);

                var dbrsStressModelInput = new DbrsStressModelInput
                {
                    AssumptionsStartDate = dbrsStressModelInputsRecord.InterestStartDate,
                    StateLevelDefaultRateDictionary = new Dictionary<PropertyState, double>(),
                    StateLevelLossGivenDefaultDictionary = new Dictionary<PropertyState, double?>(),
                    StateLevelForeclosureTermInMonthsDictionary = new Dictionary<PropertyState, int>(),
                    ReperformanceTermInMonths = new Dictionary<PropertyState, int>(),
                    TotalNumberOfDefaultSequences = dbrsStressModelInputsRecord.TotalNumberOfDefaultSequences,
                };

                var defaultPropertyState = (dbrsStressModelInputsRecord.DefaultPropertyState != null)
                    ? (PropertyState)Enum.Parse(typeof(PropertyState), dbrsStressModelInputsRecord.DefaultPropertyState)
                    : default(PropertyState);

                dbrsStressModelInput.StateLevelDefaultRateDictionary.Add(defaultPropertyState, dbrsStressModelInputsRecord.DefaultRate);
                dbrsStressModelInput.StateLevelLossGivenDefaultDictionary.Add(defaultPropertyState, dbrsStressModelInputsRecord.LossGivenDefault);
                dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary.Add(defaultPropertyState, dbrsStressModelInputsRecord.ForeclosureTermInMonths);
                dbrsStressModelInput.ReperformanceTermInMonths.Add(defaultPropertyState, dbrsStressModelInputsRecord.ReperformanceTermInMonths);

                if (dbrsStressModelInputsRecord.SecondPropertyState != null)
                {
                    var secondPropertyState = (PropertyState)Enum.Parse(typeof(PropertyState), dbrsStressModelInputsRecord.SecondPropertyState);
                    if (secondPropertyState == defaultPropertyState)
                    {
                        throw new Exception("ERROR: Cannot add DBRS stress data for the same property state twice. Please check inputs.");
                    }

                    dbrsStressModelInput.StateLevelDefaultRateDictionary.Add(secondPropertyState, dbrsStressModelInputsRecord.DefaultRateSecondState);
                    dbrsStressModelInput.StateLevelLossGivenDefaultDictionary.Add(secondPropertyState, dbrsStressModelInputsRecord.LossGivenDefaultSecondState);
                    dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary.Add(secondPropertyState, dbrsStressModelInputsRecord.ForeclosureTermInMonthsSecondState);
                    dbrsStressModelInput.ReperformanceTermInMonths.Add(secondPropertyState, dbrsStressModelInputsRecord.ReperformanceTermInMonthsSecondState);

                    if (dbrsStressModelInputsRecord.ThirdPropertyState != null)
                    {
                        var thirdPropertyState = (PropertyState)Enum.Parse(typeof(PropertyState), dbrsStressModelInputsRecord.ThirdPropertyState);
                        if (thirdPropertyState == secondPropertyState || thirdPropertyState == defaultPropertyState)
                        {
                            throw new Exception("ERROR: Cannot add DBRS stress data for the same property state twice. Please check inputs.");
                        }

                        dbrsStressModelInput.StateLevelDefaultRateDictionary.Add(thirdPropertyState, dbrsStressModelInputsRecord.DefaultRateThirdState);
                        dbrsStressModelInput.StateLevelLossGivenDefaultDictionary.Add(thirdPropertyState, dbrsStressModelInputsRecord.LossGivenDefaultThirdState);
                        dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary.Add(thirdPropertyState, dbrsStressModelInputsRecord.ForeclosureTermInMonthsThirdState);
                        dbrsStressModelInput.ReperformanceTermInMonths.Add(thirdPropertyState, dbrsStressModelInputsRecord.ReperformanceTermInMonthsThirdState);
                    }
                }

                dictionaryOfDbrsStressModelInputs.Add(securitizationInput.ScenarioDescription, dbrsStressModelInput);
            }

            dbrsStressModelInputsDictionary = dictionaryOfDbrsStressModelInputs;
            return listOfSecuritizationInputs;
        }

        private static SecuritizationInput ConvertSecuritizationInputsRecord(SecuritizationInputsRecord securitizationInputsRecord)
        {
            var securitizationInput = new SecuritizationInput
            {
                ScenarioDescription = securitizationInputsRecord.ScenarioDescription,
                AdditionalYieldScenarioDescription = securitizationInputsRecord.AdditionalYieldScenario,

                CollateralCutOffDate = securitizationInputsRecord.CollateralCutOffDate,
                CashFlowStartDate = securitizationInputsRecord.StartDate,
                InterestAccrualStartDate = securitizationInputsRecord.InterestStartDate,

                SelectedAggregationGrouping = securitizationInputsRecord.AggregationGrouping,
                SelectedPerformanceAssumption = securitizationInputsRecord.PerformanceAssumption,
                SelectedPerformanceAssumptionGrouping = securitizationInputsRecord.PerformanceAssumptionGrouping,

                CashFlowPricingStrategy = new DoNothingPricingStrategy(),

                PreFundingPercentageAmount = securitizationInputsRecord.PreFundingAmount,
                BondCountPerPreFunding = securitizationInputsRecord.PreFundingBondCount,
                UsePreFundingStartDate = securitizationInputsRecord.UsePreFundingStartDate,

                SecuritizationStartDate = securitizationInputsRecord.SecuritizationStartDate,
                SecuritizationFirstCashFlowDate = securitizationInputsRecord.SecuritizationFirstCashFlowDate,
                LastPreFundingDate = securitizationInputsRecord.LastPreFundingDate,
            };

            // Note, the default behavior is to turn on replines, so if nothing is provided, the default will be true
            if (securitizationInputsRecord.TurnOnReplining.HasValue)
            {
                securitizationInput.UseReplines = securitizationInputsRecord.TurnOnReplining.Value;
            }

            // Note, the default hehavior is to have no clean-up call
            if (securitizationInputsRecord.CleanUpCallPercentage.HasValue && securitizationInputsRecord.CleanUpCallPercentage.Value > 0.0)
            {
                securitizationInput.CleanUpCallPercentage = securitizationInputsRecord.CleanUpCallPercentage.Value;
            }

            // Note, the default behavior is not to separate out prepayment interest, so if nothing is provided, the default will be false
            if (securitizationInputsRecord.SeparatePrepaymentInterest.HasValue)
            {
                securitizationInput.SeparatePrepaymentInterest = securitizationInputsRecord.SeparatePrepaymentInterest.Value;
            }

            return securitizationInput;
        }
    }
}
