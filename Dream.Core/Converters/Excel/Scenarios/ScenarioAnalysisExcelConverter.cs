using Dream.Common;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Scenarios;
using Dream.Core.BusinessLogic.Scenarios.PrincingStrategyShocks;
using Dream.Core.BusinessLogic.Scenarios.PerformanceAssumptionShocks;
using Dream.Core.Interfaces;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;

namespace Dream.Core.Converters.Excel.Scenarios
{
    public class ScenarioAnalysisExcelConverter
    {
        private const string _cpr = "CPR";
        private const string _priceToCall = "Call";
        private const string _pricingShock = "Pricing Shock";
        private const string _collateral = "Collateral";

        private CashFlowGenerationInput _baseInputs;
        private Dictionary<int, List<IScenarioLogic>> _scenarioLogicDictionary;

        public ScenarioAnalysisExcelConverter(CashFlowGenerationInput baseInputs)
        {
            _baseInputs = baseInputs;
            _scenarioLogicDictionary = new Dictionary<int, List<IScenarioLogic>>();
        }

        /// <summary>
        /// Gets a list of scenarios to analyze, with one for each prepayment shock provided. All internal shocks are provided as children
        /// or nest scenarios of the parent prepayment shocks.
        /// </summary>
        public List<ScenarioAnalysis> GetPrepaymentShockScenarioAnalyses(List<double> callPercentages, List<double> prepaymentShocks, string shockStrategyDescription)
        {
            var shockStrategy = ShockStrategyExcelConverter.ConvertString(shockStrategyDescription);
            var listOfSecuritizationScenarios = new List<ScenarioAnalysis>();

            foreach (var callPercentage in callPercentages)
            {
                foreach (var prepaymentShock in prepaymentShocks)
                {
                    // Note that each prepayment shock should be annualized and thus represented as a CPR
                    var prepaymentShockDescription = prepaymentShock.ToString("0%") + " " + _cpr;
                    if (callPercentage > 0.0) prepaymentShockDescription += ("-" + callPercentage.ToString("0%") + " " + _priceToCall);
                    var prepaymentShockScenarioAnalysis = new ScenarioAnalysis(prepaymentShockDescription);

                    var prepaymentShockScenario = new PrepaymentShockScenario(prepaymentShock, shockStrategy);
                    prepaymentShockScenarioAnalysis.AddScenarioLogic(prepaymentShockScenario);
                    if (callPercentage > 0.0) AddCleanUpCallScenario(prepaymentShockScenarioAnalysis, callPercentage);

                    // After adding the parent prepayment shock, now add all the child value shocks
                    foreach (var scenarioNumber in _scenarioLogicDictionary.Keys)
                    {
                        var pricingShockScenarioDescription = _pricingShock + " " + scenarioNumber;
                        var pricingShockScenario = new ScenarioAnalysis(pricingShockScenarioDescription);

                        pricingShockScenario.AddListOfScenarioLogic(_scenarioLogicDictionary[scenarioNumber]);
                        prepaymentShockScenarioAnalysis.NestedScenarios.Add(pricingShockScenario);
                    }

                    // Now, add the scenaro and it's nested scenarios to the list
                    listOfSecuritizationScenarios.Add(prepaymentShockScenarioAnalysis);
                }
            }

            return listOfSecuritizationScenarios;
        }

        /// <summary>
        /// Adds pricing shocks to the internal scenario logic dictionary based on the provided Excel record objects
        /// </summary>
        public void AddPricingScenarioRecords(List<PricingScenarioRecord> listOfSecuritizationPricingScenarioRecords)
        {
            foreach (var securitizationPricingScenarioRecord in listOfSecuritizationPricingScenarioRecords)
            {
                AddPricingScenarioRecord(securitizationPricingScenarioRecord);
            }
        }

        /// <summary>
        /// Adds pricing shocks to the internal scenario logic dictionary based on the provided Excel record object
        /// </summary>
        private void AddPricingScenarioRecord(PricingScenarioRecord securitizationPricingScenarioRecord)
        {
            var scenarioNumber = securitizationPricingScenarioRecord.ScenarioNumber;
            var groupingIdentifier = securitizationPricingScenarioRecord.GroupingIdentifier;
            var shockValue = securitizationPricingScenarioRecord.ScenarioValue;
            var shockStrategy = ShockStrategyExcelConverter.ConvertString(securitizationPricingScenarioRecord.ScenarioStrategy);          

            var pricingMethodology = securitizationPricingScenarioRecord.PricingMethodology;

            switch (pricingMethodology)
            {
                case Constants.NominalSpreadBasedPricing:
                    AddPricingShockScenario<NominalSpreadShockScenario>(scenarioNumber, groupingIdentifier, shockValue, shockStrategy);
                    break;

                case Constants.YieldBasedPricing:
                    AddPricingShockScenario<YieldShockScenario>(scenarioNumber, groupingIdentifier, shockValue, shockStrategy);
                    break;

                case Constants.MarketValueBasedPricing:
                    AddPricingShockScenario<MarketValueShockScenario>(scenarioNumber, groupingIdentifier, shockValue, shockStrategy);
                    break;

                case Constants.PercentOfBalanceBasedPricing:
                    AddPricingShockScenario<PriceShockScenario>(scenarioNumber, groupingIdentifier, shockValue, shockStrategy);
                    break;

                default:
                    throw new Exception(string.Format("ERROR: The pricing methodology provided in the Price/Yield Table inputs named '{0}' is not supported.",
                        pricingMethodology));
            }
        }

        private void AddPricingShockScenario<T>(int scenarioNumber, string groupingIdentifier, double shockValue, ShockStrategy shockStrategy) 
            where T : PricingStrategyShockScenario
        {
            if (groupingIdentifier == _collateral)
            {
                var newPriceShockScenario = Activator.CreateInstance<T>();
                newPriceShockScenario.SetGlobalScenarioShock(shockValue, shockStrategy);

                _scenarioLogicDictionary.Add(scenarioNumber, new List<IScenarioLogic>());
                _scenarioLogicDictionary[scenarioNumber].Add(newPriceShockScenario);
                return;
            }

            if (_scenarioLogicDictionary.ContainsKey(scenarioNumber))
            {
                // Find all the shock scenarios of a given type
                var priceShockScenarios = _scenarioLogicDictionary[scenarioNumber].OfType<T>().Where(s => s.GlobalScenarioShock == null).ToList();
                if (priceShockScenarios.Any())
                {                   
                    if (priceShockScenarios.Any(s => s.ScenarioShockDictionary.ContainsKey(groupingIdentifier)))
                    {
                        throw new Exception(string.Format("ERROR: Mulitple pricing shock scenarios are specified in the same scenario analysis for the tranche named '{0}'.",
                            groupingIdentifier));
                    }

                    // Find the one shock scenario of this type without the tranche name in its dictionary
                    var priceShockScenario = priceShockScenarios.SingleOrDefault();
                    if (priceShockScenario != null)
                    {
                        priceShockScenario.AddToScenarioShockDictionary(groupingIdentifier, shockValue, shockStrategy);
                    }
                    else
                    {
                        throw new Exception("INTERNAL ERROR: Mulitple pricing shock scenarios are being created where only one is necessary. Please report this error.");
                    }
                }
                else
                {
                    // If no scenarios of this type yet exist, add one
                    var newPriceShockScenario = Activator.CreateInstance<T>();
                    newPriceShockScenario.AddToScenarioShockDictionary(groupingIdentifier, shockValue, shockStrategy);

                    _scenarioLogicDictionary[scenarioNumber].Add(newPriceShockScenario);
                }
            }
            else
            {
                // If there is no list of scenarios yet for this scenario number, add it
                var newPriceShockScenario = Activator.CreateInstance<T>();
                newPriceShockScenario.AddToScenarioShockDictionary(groupingIdentifier, shockValue, shockStrategy);

                _scenarioLogicDictionary.Add(scenarioNumber, new List<IScenarioLogic>());
                _scenarioLogicDictionary[scenarioNumber].Add(newPriceShockScenario);
            }
        }

        private void AddCleanUpCallScenario(ScenarioAnalysis scenarioAnalysis, double cleanUpCallPercentage)
        {
            if (_baseInputs is SecuritizationInput baseSecuritizationInputs)
            {
                var scenarioSecuritizationInputs = baseSecuritizationInputs.Copy();
                scenarioSecuritizationInputs.ScenarioDescription = scenarioAnalysis.Description;
                scenarioSecuritizationInputs.CleanUpCallPercentage = cleanUpCallPercentage;

                var inputsScenario = new InputsScenario(scenarioSecuritizationInputs);
                scenarioAnalysis.AddScenarioLogic(inputsScenario);
            }
        }
    }
}
