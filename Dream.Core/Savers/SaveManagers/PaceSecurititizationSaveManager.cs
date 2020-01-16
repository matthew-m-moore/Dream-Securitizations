using Dream.Common.Enums;
using Dream.Core.Repositories.Database;
using System.Collections.Generic;

namespace Dream.Core.Savers.SaveManagers
{
    public class PaceSecurititizationSaveManager : SecuritizationSaveManager
    {
        public PaceSecurititizationSaveManager(SecuritizationDatabaseSaver securitizationDatabaseSaver) : base(securitizationDatabaseSaver)
        {
            _ModifiedComponentsSaveMethodDictionary.Add(
                SecuritizationComponent.Collateral, 
                _SecuritizationDatabaseSaver.SavePaceAssessmentCollateral);

            _SecuritizationComponentsDescriptionDictionary.Add(
                SecuritizationComponent.Collateral,
                new List<string> { SecuritizationDatabaseRepository.PaceAssessmentCollateral });

            _SecuritizationComponentsDescriptionDictionary[SecuritizationComponent.Scenarios].Add(SecuritizationDatabaseRepository.PaceAssessmentCollateral);
        }
    }
}
