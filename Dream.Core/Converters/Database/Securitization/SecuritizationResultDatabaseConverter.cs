using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.Reporting.Results;
using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;

namespace Dream.Core.Converters.Database.Securitization
{
    public class SecuritizationResultDatabaseConverter
    {
        private const string _weightedAverageLife = "Weighted Average Life (WAL)";
        private const string _macaulayDuration = "Macaulay Duration";
        private const string _modifiedDurationAnalytical = "Modified Duration (analytical)";
        private const string _modifiedDurationNumerical = "Modified Duration (numerical)";
        private const string _paymentWindowStart = "Payment Window (start)";
        private const string _paymentWindowEnd = "Payment Window (end)";
        private const string _nominalSpread = "Nominal Spread";
        private const string _nominalBenchmarkRate = "Nominal Benchmark Rate";
        private const string _presentValue = "Present Value";
        private const string _internalRateOfReturn = "Internal Rate of Return (IRR)";
        private const string _dollarPrice = "Dollar Price";
        private const string _dollarDuration = "Dollar Duration";

        private readonly Dictionary<int, (string Description, double? Divisor)> _securitizationResultTypesDictionary;

        private readonly List<string> _selectedResultsToSave = new List<string>
        {
            _weightedAverageLife,
            _macaulayDuration,
            _modifiedDurationAnalytical,
            _modifiedDurationNumerical,
            _paymentWindowStart,
            _paymentWindowEnd,
            _nominalSpread,
            _nominalBenchmarkRate,
            _presentValue,
            _internalRateOfReturn,
            _dollarPrice,
            _dollarDuration,
        };

        public SecuritizationResultDatabaseConverter(Dictionary<int, (string Description, double? Divisor)> securitizationResultTypesDictionary)
        {
            _securitizationResultTypesDictionary = securitizationResultTypesDictionary;
        }

        public List<SecuritizationAnalysisResultEntity> ConvertToSecuritizationAnalysisResultEntities(
            SecuritizationCashFlowsSummaryResult securitizationCashFlowsSummaryResult,
            Dictionary<string, int> securitizationResultTypeDescriptionsDictionary,
            string securitizationNodeName,
            int? securitizationTrancheDetailId,    
            int securitizationAnalysisDataSetId,
            int securitizationAnalysisVersionId,
            int securitizationAnalysisScenarioId)
        {
            var securitizationAnalysisResultEntities = new List<SecuritizationAnalysisResultEntity>();
            foreach(var selectedResultToSave in _selectedResultsToSave)
            {
                var securitizationResultTypeId = securitizationResultTypeDescriptionsDictionary[selectedResultToSave];
                var securitizationAnalysisResultEntity = ConvertToSecuritizationAnalysisResultEntity(
                    securitizationCashFlowsSummaryResult,
                    securitizationNodeName,
                    selectedResultToSave,
                    securitizationTrancheDetailId,
                    securitizationResultTypeId,
                    securitizationAnalysisDataSetId,
                    securitizationAnalysisVersionId,
                    securitizationAnalysisScenarioId);

                // For values that are NaN or infinity, they cannot be saved to the database as 'float'
                if (double.IsNaN(securitizationAnalysisResultEntity.SecuritizationResultValue) ||
                    double.IsInfinity(securitizationAnalysisResultEntity.SecuritizationResultValue))
                    continue;

                securitizationAnalysisResultEntities.Add(securitizationAnalysisResultEntity);
            }

            return securitizationAnalysisResultEntities;
        }

        public SecuritizationResult ConvertToSecuritizationResult(
            List<SecuritizationAnalysisResultEntity> securitizationAnalysisResultEntities,
            Dictionary<int, TrancheDetailEntity> trancheDetailsDictionary)
        {
            var securitizationResult = new SecuritizationResult();
            foreach (var securitizationAnalysisResultEntity in securitizationAnalysisResultEntities)
            {
                // Results can be stored at both the tranche level and node level
                string dictionaryKey = null;
                if (securitizationAnalysisResultEntity.SecuritizationTrancheDetailId.HasValue)
                {
                    var trancheDetailId = securitizationAnalysisResultEntity.SecuritizationTrancheDetailId.Value;
                    if (trancheDetailsDictionary.ContainsKey(trancheDetailId))
                    {
                        dictionaryKey = trancheDetailsDictionary[trancheDetailId].TrancheName;               
                    }
                }
                else if (!string.IsNullOrEmpty(securitizationAnalysisResultEntity.SecuritizationNodeName))
                {
                    dictionaryKey = securitizationAnalysisResultEntity.SecuritizationNodeName;
                }

                // If the key is empty, skip it
                if (string.IsNullOrEmpty(dictionaryKey)) continue;

                // If the dictionary is missing this key add it
                if (!securitizationResult.SecuritizationResultsDictionary.ContainsKey(dictionaryKey))
                {
                    var emptyListOfCashFlows = new List<SecuritizationCashFlow>();
                    var securitizationSummaryResult = new SecuritizationCashFlowsSummaryResult(emptyListOfCashFlows);

                    // Setting up the tranche name is important for identifying displayable results
                    if (securitizationAnalysisResultEntity.SecuritizationTrancheDetailId.HasValue)
                    {
                        securitizationSummaryResult.TrancheName = dictionaryKey;
                    }
                    else
                    {
                        securitizationSummaryResult.SecuritizationNodeName = dictionaryKey;
                    }

                    securitizationResult.SecuritizationResultsDictionary.Add(dictionaryKey, securitizationSummaryResult);
                }

                var resultValue = securitizationAnalysisResultEntity.SecuritizationResultValue;
                var resultTypeId = securitizationAnalysisResultEntity.SecuritizationResultTypeId;

                AddResultToDictionary(securitizationResult, dictionaryKey, resultValue, resultTypeId);
            }

            return securitizationResult;
        }

        private SecuritizationAnalysisResultEntity ConvertToSecuritizationAnalysisResultEntity(
            SecuritizationCashFlowsSummaryResult securitizationCashFlowsSummaryResult, 
            string securitizationNodeName,
            string selectedResultToSave,
            int? securitizationTrancheDetailId,
            int securitizationResultTypeId,
            int securitizationAnalysisDataSetId,
            int securitizationAnalysisVersionId,
            int securitizationAnalysisScenarioId)
        {
            var resultDescriptionAndDivisor = _securitizationResultTypesDictionary[securitizationResultTypeId];
            var resultDescription = resultDescriptionAndDivisor.Description;
            var resultMultiplier = resultDescriptionAndDivisor.Divisor.GetValueOrDefault(1.0);

            var securitizationAnalysisResultEntity = new SecuritizationAnalysisResultEntity
            {
                SecuritizationAnalysisDataSetId = securitizationAnalysisDataSetId,
                SecuritizationAnalysisVersionId = securitizationAnalysisVersionId,
                SecuritizationAnalysisScenarioId = securitizationAnalysisScenarioId,
                SecuritizationTrancheDetailId = securitizationTrancheDetailId,
                SecuritizationNodeName = securitizationNodeName,
                SecuritizationResultTypeId = securitizationResultTypeId,       
            };

            switch (resultDescription)
            {
                case _weightedAverageLife:
                    securitizationAnalysisResultEntity.SecuritizationResultValue = securitizationCashFlowsSummaryResult.WeightedAverageLife;
                    break;

                case _macaulayDuration:
                    securitizationAnalysisResultEntity.SecuritizationResultValue = securitizationCashFlowsSummaryResult.MacaulayDuration;
                    break;

                case _modifiedDurationAnalytical:
                    securitizationAnalysisResultEntity.SecuritizationResultValue = securitizationCashFlowsSummaryResult.ModifiedDurationAnalytical;
                    break;

                case _modifiedDurationNumerical:
                    securitizationAnalysisResultEntity.SecuritizationResultValue = securitizationCashFlowsSummaryResult.ModifiedDurationNumerical;
                    break;

                case _nominalSpread:
                    securitizationAnalysisResultEntity.SecuritizationResultValue = securitizationCashFlowsSummaryResult.NominalSpread.GetValueOrDefault();
                    break;

                case _nominalBenchmarkRate:
                    securitizationAnalysisResultEntity.SecuritizationResultValue = securitizationCashFlowsSummaryResult.NominalBenchmarkRate.GetValueOrDefault();
                    break;

                case _presentValue:
                    securitizationAnalysisResultEntity.SecuritizationResultValue = securitizationCashFlowsSummaryResult.PresentValue;
                    break;

                case _internalRateOfReturn:
                    securitizationAnalysisResultEntity.SecuritizationResultValue = securitizationCashFlowsSummaryResult.InternalRateOfReturn;
                    break;

                case _dollarPrice:
                    securitizationAnalysisResultEntity.SecuritizationResultValue = securitizationCashFlowsSummaryResult.DollarPrice.GetValueOrDefault();
                    break;

                case _dollarDuration:
                    securitizationAnalysisResultEntity.SecuritizationResultValue = securitizationCashFlowsSummaryResult.DollarDuration;
                    break;

                case _paymentWindowStart:
                case _paymentWindowEnd:
                    HandlePaymentCorridor(securitizationAnalysisResultEntity, securitizationCashFlowsSummaryResult.PaymentCorridor, resultDescription);
                    break;
            }

            securitizationAnalysisResultEntity.SecuritizationResultValue *= resultMultiplier;
            return securitizationAnalysisResultEntity;
        }

        private void AddResultToDictionary(SecuritizationResult securitizationResult, string dictionaryKey, double resultValue, int resultTypeId)
        {
            // There is foreign key in the database that should ensure this key always exists
            var resultDescriptionAndDivisor = _securitizationResultTypesDictionary[resultTypeId];
            var resultDescription = resultDescriptionAndDivisor.Description;
            var resultDivisor = resultDescriptionAndDivisor.Divisor.GetValueOrDefault(1.0);

            resultValue /= resultDivisor;

            switch (resultDescription)
            {
                case _weightedAverageLife:
                    securitizationResult.SecuritizationResultsDictionary[dictionaryKey].WeightedAverageLife = resultValue;
                    return;

                case _macaulayDuration:
                    securitizationResult.SecuritizationResultsDictionary[dictionaryKey].MacaulayDuration = resultValue;
                    return;

                case _modifiedDurationAnalytical:
                    securitizationResult.SecuritizationResultsDictionary[dictionaryKey].ModifiedDurationAnalytical = resultValue;
                    return;

                case _modifiedDurationNumerical:
                    securitizationResult.SecuritizationResultsDictionary[dictionaryKey].ModifiedDurationNumerical = resultValue;
                    return;

                case _nominalSpread:
                    securitizationResult.SecuritizationResultsDictionary[dictionaryKey].NominalSpread = resultValue;
                    return;

                case _nominalBenchmarkRate:
                    securitizationResult.SecuritizationResultsDictionary[dictionaryKey].NominalBenchmarkRate = resultValue;
                    return;

                case _presentValue:
                    securitizationResult.SecuritizationResultsDictionary[dictionaryKey].PresentValue = resultValue;
                    return;

                case _internalRateOfReturn:
                    securitizationResult.SecuritizationResultsDictionary[dictionaryKey].InternalRateOfReturn = resultValue;
                    return;

                case _dollarPrice:
                    securitizationResult.SecuritizationResultsDictionary[dictionaryKey].DollarPrice = resultValue;
                    return;

                case _dollarDuration:
                    securitizationResult.SecuritizationResultsDictionary[dictionaryKey].DollarDuration = resultValue;
                    return;

                case _paymentWindowStart:
                case _paymentWindowEnd:
                    HandlePaymentCorridor(securitizationResult, resultDescription, dictionaryKey, resultValue);
                    return;
            }
        }

        private void HandlePaymentCorridor(SecuritizationAnalysisResultEntity securitizationAnalysisResultEntity, PaymentCorridor paymentCorridor, string resultDescription)
        {
            if (paymentCorridor == null) return;

            if (resultDescription == _paymentWindowStart)
            {
                securitizationAnalysisResultEntity.SecuritizationResultValue = Convert.ToDouble(paymentCorridor.FirstPaymentPeriod);
            }

            if (resultDescription == _paymentWindowEnd)
            {
                securitizationAnalysisResultEntity.SecuritizationResultValue = Convert.ToDouble(paymentCorridor.LastPaymentPeriod);
            }
        }

        private void HandlePaymentCorridor(SecuritizationResult securitizationResult, string resultDescription, string dictionaryKey, double resultValue)
        {
            if (securitizationResult.SecuritizationResultsDictionary[dictionaryKey].PaymentCorridor == null)
            {
                securitizationResult.SecuritizationResultsDictionary[dictionaryKey].PaymentCorridor = new PaymentCorridor();
            }

            if (resultDescription == _paymentWindowStart)
            {
                securitizationResult.SecuritizationResultsDictionary[dictionaryKey].PaymentCorridor.FirstPaymentPeriod = Convert.ToInt32(resultValue);
            }

            if (resultDescription == _paymentWindowEnd)
            {
                securitizationResult.SecuritizationResultsDictionary[dictionaryKey].PaymentCorridor.LastPaymentPeriod = Convert.ToInt32(resultValue);
            }
        }
    }
}
