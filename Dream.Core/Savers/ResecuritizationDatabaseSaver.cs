using Dream.IO.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using Dream.Core.BusinessLogic.Scenarios;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.Repositories.Database;

namespace Dream.Core.Savers
{
    public class ResecuritizationDatabaseSaver : SecuritizationDatabaseSaver
    {
        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        protected override bool _IsResecuritization => true;

        public ResecuritizationDatabaseSaver(Securitization resecuritization, List<ScenarioAnalysis> scenariosToAnalyze,
            bool isNewVersionOfExistingSecuritization = false,
            int? securitizationAnalysisDataSetId = null)
        : base(resecuritization, 
               scenariosToAnalyze,
               isNewVersionOfExistingSecuritization,
               securitizationAnalysisDataSetId)
        { }

        public void SaveCollateralizedSecuritizations()
        {
            if (_Securitization is Resecuritization _resecuritization)
            {
                var collateralizedSecuritizationDatabaseSaver = new CollateralizedSecuritizationDatabaseSaver(
                    _resecuritization,
                    _CutOffDate,
                    _Description);

                var securitizationInputDataSetId = collateralizedSecuritizationDatabaseSaver.SaveCollateralizedSecuritizations();
                var securitizationAnalysisEntity = CreateSecuritizationAnalysisEntity(
                    ResecuritizationDatabaseRepository.CollateralizedSecuritizations,
                    securitizationInputDataSetId,
                    _BaseId);

                SecuritizationAnalysisEntities.Add(securitizationAnalysisEntity);
            }
            else
            {
                throw new Exception("INTERNAL ERROR: The securitization provided is not a resecuritization. Please report this error.");
            }
        }
    }
}
