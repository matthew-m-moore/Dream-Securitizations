using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.WebApp.Models;

namespace Dream.WebApp.Adapters
{
    public class CollateralizedTranchesModelAdapter
    {
        public static CollateralizedTranchesModel GetCollateralizedTranchesModel(Securitization securitizationBusinessObject, bool isResecuritization)
        {
            if (!isResecuritization) return null;

            throw new NotImplementedException();
        }
    }
}