using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Core.Reporting.Results;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches
{
    public class AccretionBondTranche : Tranche
    {
        public AccretionBondTranche(
            string trancheName,
            PricingStrategy pricingStrategy,
            AllFundsAvailableFundsRetriever availableFundsRetriever) :
            base(trancheName, pricingStrategy, availableFundsRetriever)
        {
        }

        public override void AllocatePaymentCashFlows(
            AvailableFunds availableFunds,
            DistributionRule distributionRule,
            int monthlyPeriod)
        {
            throw new NotImplementedException();
        }

        public override Tranche Copy()
        {
            throw new NotImplementedException();
        }
    }
}
