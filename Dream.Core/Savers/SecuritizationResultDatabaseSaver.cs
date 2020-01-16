using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.Converters.Database.Securitization;
using Dream.Core.Reporting.Results;
using Dream.Core.Repositories.Database;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dream.Core.Savers
{
    public class SecuritizationResultDatabaseSaver : DatabaseSaver
    {
        private Securitization _securitization;
        private SecuritizationDatabaseRepository _securitizationDatabaseRepository;
        private Dictionary<string, int> _scenarioDescriptionsDictionary;

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        public List<string> SecuritizationNodeNamesOfResultsToSave { get; private set; }
        public List<string> SecuritizationTrancheNamesOfResultsToSave { get; private set; }

        public SecuritizationResultDatabaseSaver(
            Securitization securitization, 
            SecuritizationDatabaseRepository securitizationDatabaseRepository,
            Dictionary<string, int> scenarioDescriptionsDictionary)
        {
            _securitization = securitization;
            _securitizationDatabaseRepository = securitizationDatabaseRepository;
            _scenarioDescriptionsDictionary = scenarioDescriptionsDictionary;

            SecuritizationNodeNamesOfResultsToSave = new List<string>();
            SecuritizationTrancheNamesOfResultsToSave = new List<string>();
        }

        public void SaveSecuritizationResults()
        {
            var resultsDictionary = _securitization.ResultsDictionary;
            SecuritizationNodeNamesOfResultsToSave = resultsDictionary.SecuritizationNodeNamesOfDisplayableResults;
            SecuritizationTrancheNamesOfResultsToSave = resultsDictionary.SecuritizationTrancheNamesOfDisplayableResults;

            var listOfSecuritizationNodeAndTrancheNamesToSave = SecuritizationNodeNamesOfResultsToSave.Select(n => new string(n.ToCharArray())).ToList();
            listOfSecuritizationNodeAndTrancheNamesToSave.AddRange(SecuritizationTrancheNamesOfResultsToSave);

            var securitizationAnalysisResultEntities = new List<SecuritizationAnalysisResultEntity>();
            var securitizationResultDatabaseConverter = new SecuritizationResultDatabaseConverter(_securitizationDatabaseRepository.SecuritizationResultTypes);

            foreach (var resultsDictionaryEntry in resultsDictionary)
            {
                var scenarioDescription = resultsDictionaryEntry.Key;
                var scenarioId = _scenarioDescriptionsDictionary[scenarioDescription];

                var securitizationResult = resultsDictionaryEntry.Value;
                foreach (var securitizationNodeOrTrancheResult in securitizationResult.SecuritizationResultsDictionary)
                {
                    var securitizationNodeOrTrancheName = securitizationNodeOrTrancheResult.Key;
                    if (!listOfSecuritizationNodeAndTrancheNamesToSave.Contains(securitizationNodeOrTrancheName)) continue;

                    int? securitizationTrancheDetailId = (_securitization.TranchesDictionary.ContainsKey(securitizationNodeOrTrancheName))
                        ? _securitization.TranchesDictionary[securitizationNodeOrTrancheName].Tranche.TrancheDetailId
                        : null;

                    var securitizationNodeName = (securitizationTrancheDetailId.HasValue)
                        ? _securitization.TranchesDictionary[securitizationNodeOrTrancheName].Tranche.SecuritizationNode.SecuritizationNodeName
                        : securitizationNodeOrTrancheName;

                    var securitizationNodeOrTrancheResultEntities = CreateListOfSecuritizationAnalysisResultEntities(
                        securitizationResultDatabaseConverter,
                        securitizationNodeOrTrancheResult.Value,
                        securitizationNodeName,
                        securitizationTrancheDetailId,
                        scenarioId);

                    securitizationAnalysisResultEntities.AddRange(securitizationNodeOrTrancheResultEntities);
                }
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.SecuritizationAnalysisResultEntities.AddRange(securitizationAnalysisResultEntities);
                securitizationEngineContext.SaveChanges();
            }
        }

        private List<SecuritizationAnalysisResultEntity> CreateListOfSecuritizationAnalysisResultEntities(
            SecuritizationResultDatabaseConverter securitizationResultDatabaseConverter,
            SecuritizationCashFlowsSummaryResult securitizationCashFlowsSummaryResult, 
            string securitizationNodeName, 
            int? securitizationTrancheDetailId,
            int scenarioId)
        {
            var securitizationAnalysisResultEntities =
                securitizationResultDatabaseConverter.ConvertToSecuritizationAnalysisResultEntities(
                    securitizationCashFlowsSummaryResult,
                    _securitizationDatabaseRepository.SecuritizationResultTypesReversed,
                    securitizationNodeName,
                    securitizationTrancheDetailId,             
                    _securitization.SecuritizationAnalysisDataSetId.Value,
                    _securitization.SecuritizationAnalysisVersionId.Value,
                    scenarioId);

            return securitizationAnalysisResultEntities;
        }
    }
}
