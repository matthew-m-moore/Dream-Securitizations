using System;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.IO.Database;
using System.Data.Entity;
using Dream.IO.Database.Entities.Collateral;
using Dream.IO.Database.Contexts;

namespace Dream.Core.Savers
{
    public class CollateralizedSecuritizationDatabaseSaver : DatabaseSaver
    {
        private Resecuritization _resecuritization;

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        public CollateralizedSecuritizationDatabaseSaver(Resecuritization resecuritization, DateTime cutOffDate, string description)
        {
            _resecuritization = resecuritization;
            _CutOffDate = cutOffDate;
            _Description = description;
        }

        public int SaveCollateralizedSecuritizations()
        {
            var collateralizedSecuritizationDataSetEntity = new CollateralizedSecuritizationDataSetEntity
            {
                CutOffDate = _CutOffDate,
                CollateralizedSecuritizationDataSetDescription = _Description,
            };

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.CollateralizedSecuritizationDataSeEntities.Add(collateralizedSecuritizationDataSetEntity);
                securitizationEngineContext.SaveChanges();
            }

            SaveCollateralizedSecuritizationTranches(collateralizedSecuritizationDataSetEntity.CollateralizedSecuritizationDataSetId);

            return collateralizedSecuritizationDataSetEntity.CollateralizedSecuritizationDataSetId;
        }

        public void SaveCollateralizedSecuritizationTranches(int collateralizedSecuritizationDataSetId)
        {
            var listOfCollateralizedSecuritizationTrancheEntities = new List<CollateralizedSecuritizationTrancheEntity>();
            var collateralizedTranchePercentageDictionary = _resecuritization.CollateralizedTranchePercentageDictionary;

            foreach (var entry in collateralizedTranchePercentageDictionary)
            {
                var securitizationName = entry.Key;
                var collateralizedTranchePercentages = entry.Value;
                var collateralizedSecuritization = _resecuritization.CollateralizedSecuritizationsDictionary[securitizationName];

                foreach (var collateralizedTranchePercentage in collateralizedTranchePercentages)
                {
                    var securitizationTrancheName = collateralizedTranchePercentage.Key;
                    var securitizationTranchePercentage = collateralizedTranchePercentage.Value;
                    var securitizationTrancheNodeStruct = collateralizedSecuritization.TranchesDictionary[securitizationTrancheName];

                    if (!collateralizedSecuritization.SecuritizationAnalysisDataSetId.HasValue ||
                        !collateralizedSecuritization.SecuritizationAnalysisVersionId.HasValue ||
                        !securitizationTrancheNodeStruct.Tranche.TrancheDetailId.HasValue)
                    {
                        throw new Exception(string.Format("INTERNAL ERROR: For securitization '{0}', tranche '{1}' - " +
                            " One of the following was not properly set: SecuritizationAnalysisDataSetId, SecuritizationAnalysisVersionId, or TrancheDetailId." +
                            " Please report this error.",
                            securitizationName,
                            securitizationTrancheName));
                    }

                    var collateralizedSecuritizationTrancheEntity = new CollateralizedSecuritizationTrancheEntity
                    {
                        CollateralizedSecuritizationDataSetId = collateralizedSecuritizationDataSetId,
                        SecuritizationAnalysisDataSetId = collateralizedSecuritization.SecuritizationAnalysisDataSetId.Value,
                        SecuritizationAnalysisVersionId = collateralizedSecuritization.SecuritizationAnalysisVersionId.Value,
                        SecuritizatizedTrancheDetailId = securitizationTrancheNodeStruct.Tranche.TrancheDetailId.Value,
                        SecuritizatizedTranchePercentage = securitizationTranchePercentage,
                    };

                    listOfCollateralizedSecuritizationTrancheEntities.Add(collateralizedSecuritizationTrancheEntity);
                }
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.CollateralizedSecuritizationTrancheEntities.AddRange(listOfCollateralizedSecuritizationTrancheEntities);
                securitizationEngineContext.SaveChanges();
            }
        }
    }
}
