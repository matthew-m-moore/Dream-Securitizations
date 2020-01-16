using Dream.Common.Enums;
using Dream.Core.Repositories.Database;
using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Savers.SaveManagers
{
    public abstract class SecuritizationSaveManager
    {
        protected SecuritizationDatabaseSaver _SecuritizationDatabaseSaver;
        protected SecuritizationDatabaseRepository _SecuritizationDatabaseRepository;

        protected Dictionary<SecuritizationComponent, Action> _ModifiedComponentsSaveMethodDictionary;

        public int SecuritizationAnalysisDataSetId => _SecuritizationDatabaseSaver.SecuritizationAnalysisDataSetId;
        public int SecuritizationAnalysisVersionId => _SecuritizationDatabaseSaver.SecuritizationAnalysisVersionId;

        private List<SecuritizationComponent> _securitizationComponentsToSave =
              new List<SecuritizationComponent>
              {
                  SecuritizationComponent.Inputs,
                  SecuritizationComponent.Collateral,
                  SecuritizationComponent.MarketRateEnvironment,
                  SecuritizationComponent.PerformanceAssumptions,
                  SecuritizationComponent.Structure,
                  SecuritizationComponent.Scenarios,
                  SecuritizationComponent.ResultsAndSummary,
              };

        protected Dictionary<SecuritizationComponent, List<string>> _SecuritizationComponentsDescriptionDictionary =
              new Dictionary<SecuritizationComponent, List<string>>
              {
                  [SecuritizationComponent.Inputs] = 
                    new List<string> { SecuritizationDatabaseRepository.SecuritizationAnalysisInput },

                  [SecuritizationComponent.MarketRateEnvironment] = 
                    new List<string> { SecuritizationDatabaseRepository.MarketRateEnvironment },

                  [SecuritizationComponent.PerformanceAssumptions] = 
                    new List<string> { SecuritizationDatabaseRepository.PerformanceAssumptions },

                  [SecuritizationComponent.Structure] = 
                    new List<string> { SecuritizationDatabaseRepository.SecuritizationNodesStructure,
                                       SecuritizationDatabaseRepository.PriorityOfPayments,
                                       SecuritizationDatabaseRepository.RedemptionLogic},

                  [SecuritizationComponent.Scenarios] =
                    new List<string> { SecuritizationDatabaseRepository.SecuritizationAnalysisInput,
                                       SecuritizationDatabaseRepository.MarketRateEnvironment,
                                       SecuritizationDatabaseRepository.PerformanceAssumptions},
              };

        public SecuritizationSaveManager(
            SecuritizationDatabaseRepository securitizationDatabaseRepository,
            SecuritizationDatabaseSaver securitizationDatabaseSaver)
        : this(securitizationDatabaseSaver)
        {
            _SecuritizationDatabaseRepository = securitizationDatabaseRepository;
        }

        public SecuritizationSaveManager(SecuritizationDatabaseSaver securitizationDatabaseSaver)
        {
            _SecuritizationDatabaseSaver = securitizationDatabaseSaver;

            _ModifiedComponentsSaveMethodDictionary =
            new Dictionary<SecuritizationComponent, Action>
            {
                [SecuritizationComponent.Inputs] = _SecuritizationDatabaseSaver.SaveSecuritizationInput,
                [SecuritizationComponent.MarketRateEnvironment] = _SecuritizationDatabaseSaver.SaveMarketRateEnvironment,
                [SecuritizationComponent.PerformanceAssumptions] = _SecuritizationDatabaseSaver.SavePerformanceAssumptions,
                [SecuritizationComponent.Structure] = _SecuritizationDatabaseSaver.SaveSecuritizationStructure,
                [SecuritizationComponent.Scenarios] = _SecuritizationDatabaseSaver.SaveSecuritizationScenarios,
                [SecuritizationComponent.ResultsAndSummary] = _SecuritizationDatabaseSaver.SaveSecuritizationResultsAndSummary,
            };
        }

        /// <summary>
        /// Saves all components of a securitization in their entirety, regardless of whether or not they have been modified.
        /// </summary>
        public void SaveSecuritization()
        {
            // Save all components
            SaveModifiedComponentsOfSecuritization(_securitizationComponentsToSave);

            // Save securitization
            _SecuritizationDatabaseSaver.SaveSecuritization();
        }

        /// <summary>
        /// Saves only the modified components of a securitization, and borrows the known data set IDs to save the unmodified components.
        /// </summary>
        public void SaveSecuritization(List<SecuritizationComponent> modifiedSecuritizationComponents)
        {
            if (_SecuritizationDatabaseRepository == null)
                throw new Exception("INTERNAL ERROR: Cannot save only the modified components of a securitization if no repository was specified. Please report this error.");

            // Save all components
            SaveModifiedComponentsOfSecuritization(modifiedSecuritizationComponents);
            SaveUnmodifiedComponentsOfSecuritization(modifiedSecuritizationComponents);

            // Save securitization
            _SecuritizationDatabaseSaver.SaveSecuritization();
        }

        private void SaveModifiedComponentsOfSecuritization(List<SecuritizationComponent> modifiedSecuritizationComponents)
        {
            foreach (var modifiedSecuritizationComponent in modifiedSecuritizationComponents.Where(c => c != SecuritizationComponent.ResultsAndSummary))
            {
                if (_ModifiedComponentsSaveMethodDictionary.ContainsKey(modifiedSecuritizationComponent))
                {
                    var saveMethod = _ModifiedComponentsSaveMethodDictionary[modifiedSecuritizationComponent];
                    saveMethod();
                }
                else
                {
                    throw new Exception(string.Format("INTERNAL ERROR: No save method exists for the securitzation compononent '{0}'. Please report this error.",
                        modifiedSecuritizationComponent));
                }
            }

            if (modifiedSecuritizationComponents.Contains(SecuritizationComponent.ResultsAndSummary)
                && _ModifiedComponentsSaveMethodDictionary.ContainsKey(SecuritizationComponent.ResultsAndSummary))
            {
                var resultsSaveMethod = _ModifiedComponentsSaveMethodDictionary[SecuritizationComponent.ResultsAndSummary];
                resultsSaveMethod();
            }
        }

        private void SaveUnmodifiedComponentsOfSecuritization(List<SecuritizationComponent> modifiedSecuritizationComponents)
        {
            var unmodifiedSecuritizationComponents =
                _securitizationComponentsToSave.Where(c => !modifiedSecuritizationComponents.Contains(c)).ToList();

            var securitizationAnalysisEntities = new List<SecuritizationAnalysisEntity>();
            foreach (var unmodifiedSecuritizationComponent in unmodifiedSecuritizationComponents)
            {
                var isResultsComponent = (unmodifiedSecuritizationComponent == SecuritizationComponent.ResultsAndSummary);
                var isScenariosComponent = (unmodifiedSecuritizationComponent == SecuritizationComponent.Scenarios);

                var securitizationInputTypeDescriptions = _SecuritizationComponentsDescriptionDictionary[unmodifiedSecuritizationComponent];
                var securitizationInputTypeIds = securitizationInputTypeDescriptions.Select(d => _SecuritizationDatabaseRepository.SecuritizationInputTypes[d]).ToList();                        

                foreach (var securitizationInputTypeId in securitizationInputTypeIds)
                {
                    if (isScenariosComponent)
                    {
                        var scenariosIds = _SecuritizationDatabaseRepository
                            .SecuritizationAnalysisEntityDictionary.Keys.Where(k => k != SecuritizationDatabaseRepository.BaseScenarioId).ToList();

                        foreach (var scenarioId in scenariosIds)
                        {
                            var unmodifiedSecuritizationAnalysisEntities = RetreiveUnmodifiedSecuritizationAnalysisEntities(securitizationInputTypeId, scenarioId);
                            securitizationAnalysisEntities.AddRange(unmodifiedSecuritizationAnalysisEntities);
                        }
                    }
                    else
                    {
                        var unmodifiedSecuritizationAnalysisEntities = RetreiveUnmodifiedSecuritizationAnalysisEntities(securitizationInputTypeId, SecuritizationDatabaseRepository.BaseScenarioId);
                        securitizationAnalysisEntities.AddRange(unmodifiedSecuritizationAnalysisEntities);
                    }
                }
            }

            _SecuritizationDatabaseSaver.SecuritizationAnalysisEntities.AddRange(securitizationAnalysisEntities);
        }

        private List<SecuritizationAnalysisEntity> RetreiveUnmodifiedSecuritizationAnalysisEntities(int securitizationInputTypeId, int scenarioId)
        {
            var unmodifiedSecuritizationAnalysisEntities =
                _SecuritizationDatabaseRepository.SecuritizationAnalysisEntityDictionary[scenarioId]
                .Where(e => e.SecuritizationInputTypeId == securitizationInputTypeId)
                .Select(x =>
                {
                    return new SecuritizationAnalysisEntity
                    {
                        SecuritizationAnalysisDataSetId = SecuritizationAnalysisDataSetId,
                        SecuritizationAnalysisVersionId = SecuritizationAnalysisVersionId,
                        SecuritizationAnalysisScenarioId = scenarioId,
                        SecuritizationInputTypeId = securitizationInputTypeId,
                        SecuritizationInputTypeDataSetId = x.SecuritizationInputTypeDataSetId,
                    };
                }).ToList();

            return unmodifiedSecuritizationAnalysisEntities;
        }
    }
}
