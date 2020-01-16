using Dream.IO.Database;
using System.Collections.Generic;
using System.Data.Entity;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.IO.Database.Entities.Securitization;
using Dream.IO.Database.Contexts;

namespace Dream.Core.Savers
{
    public class SecuritizationSummaryDatabaseSaver : DatabaseSaver
    {
        private Securitization _securitization;
        private Dictionary<string, int> _scenarioDescriptionsDictionary;
        private List<SecuritizationAnalysisSummaryEntity> _securitizationAnalysisSummaryEntities;

        public SecuritizationSummaryDatabaseSaver(Securitization securitization, Dictionary<string, int> scenarioDescriptionsDictionary)
        {
            _securitization = securitization;
            _scenarioDescriptionsDictionary = scenarioDescriptionsDictionary;
            _securitizationAnalysisSummaryEntities = new List<SecuritizationAnalysisSummaryEntity>();
        }

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        public void PrepareSecuritizationNodeSummary(List<string> securitizationNodeNames)
        {
            foreach(var securitizationNodeName in securitizationNodeNames)
            {
                if (!_securitization.NodesDictionary.ContainsKey(securitizationNodeName)) continue;
                var securitizationNode = _securitization.NodesDictionary[securitizationNodeName];

                if (string.IsNullOrEmpty(securitizationNode.SecuritizationNodePricingScenario)) continue;
                if (!_scenarioDescriptionsDictionary.ContainsKey(securitizationNode.SecuritizationNodePricingScenario)) continue;
                var scenarioId = _scenarioDescriptionsDictionary[securitizationNode.SecuritizationNodePricingScenario];

                var securitizationAnalysisSummaryEntity = new SecuritizationAnalysisSummaryEntity
                {
                    SecuritizationAnalysisDataSetId = _securitization.SecuritizationAnalysisDataSetId.Value,
                    SecuritizationAnalysisVersionId = _securitization.SecuritizationAnalysisVersionId.Value,
                    SecuritizationAnalysisScenarioId = scenarioId,
                    SecuritizationNodeName = securitizationNodeName, 
                    SecuritizationTrancheType = securitizationNode.SecuritizationNodeType,
                    SecuritizationTrancheRating = securitizationNode.SecuritizationNodeRating,
                };

                _securitizationAnalysisSummaryEntities.Add(securitizationAnalysisSummaryEntity);
            }
        }

        public void PrepareSecuritizationTrancheSummary(List<string> securitizationTrancheNames)
        {
            foreach (var securitizationTrancheName in securitizationTrancheNames)
            {
                if (!_securitization.TranchesDictionary.ContainsKey(securitizationTrancheName)) continue;
                var securitizationTranche = _securitization.TranchesDictionary[securitizationTrancheName].Tranche;
                var securitiztaionNode = _securitization.TranchesDictionary[securitizationTrancheName].SecuritizationNode;

                if (!_scenarioDescriptionsDictionary.ContainsKey(securitizationTranche.TranchePricingScenario)) continue;
                var scenarioId = _scenarioDescriptionsDictionary[securitizationTranche.TranchePricingScenario];

                var securitizationAnalysisSummaryEntity = new SecuritizationAnalysisSummaryEntity
                {
                    SecuritizationAnalysisDataSetId = _securitization.SecuritizationAnalysisDataSetId.Value,
                    SecuritizationAnalysisVersionId = _securitization.SecuritizationAnalysisVersionId.Value,
                    SecuritizationAnalysisScenarioId = scenarioId,
                    SecuritizationTrancheDetailId = securitizationTranche.TrancheDetailId.Value,
                    SecuritizationNodeName = securitiztaionNode.SecuritizationNodeName,
                    SecuritizationTrancheType = securitizationTranche.TrancheDescription,
                    SecuritizationTrancheRating = securitizationTranche.TrancheRating,
                };

                _securitizationAnalysisSummaryEntities.Add(securitizationAnalysisSummaryEntity);
            }
        }

        public void SaveSecuritizationSummaries()
        {
            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.SecuritizationAnalysisSummaryEntities.AddRange(_securitizationAnalysisSummaryEntities);
                securitizationEngineContext.SaveChanges();
            }
        }
    }
}
