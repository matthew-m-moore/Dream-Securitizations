using Dream.Common.Enums;
using Dream.Core.Repositories.Database;
using System;
using System.Collections.Generic;

namespace Dream.Core.Savers.SaveManagers
{
    public class ResecuritizationSaveManager : SecuritizationSaveManager
    {
        public ResecuritizationSaveManager(ResecuritizationDatabaseSaver resecuritizationDatabaseSaver) : base(resecuritizationDatabaseSaver)
        {
            if (_SecuritizationDatabaseSaver is ResecuritizationDatabaseSaver _resecuritizationDatabaseSaver)
            {
                _ModifiedComponentsSaveMethodDictionary.Add(
                    SecuritizationComponent.Collateral,
                    _resecuritizationDatabaseSaver.SaveCollateralizedSecuritizations);

                _SecuritizationComponentsDescriptionDictionary.Add(
                    SecuritizationComponent.Collateral,
                    new List<string> { ResecuritizationDatabaseRepository.CollateralizedSecuritizations });
            }
            else
            {
                throw new Exception("INTERNAL ERROR: The securitization database saver provided cannot be used for a resecuritization. Please report this error.");
            }
        }
    }
}
