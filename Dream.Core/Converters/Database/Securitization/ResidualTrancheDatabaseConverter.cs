using System;
using Dream.IO.Database.Entities.Securitization;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.PricingStrategies;

namespace Dream.Core.Converters.Database.Securitization
{
    public class ResidualTrancheDatabaseConverter
    {
        public static ResidualTranche CreateTranche(
            (Type TrancheType, bool IsResidualTranche) trancheTypeInformation,
             PricingStrategy pricingStrategy,
             TrancheDetailEntity trancheDetailEntity,
             AvailableFundsRetriever availableFundsRetriever)
        {
            var residualTranche = new ResidualTranche(
                trancheDetailEntity.TrancheName,
                pricingStrategy,
                availableFundsRetriever);

            residualTranche.AccruedPayment = trancheDetailEntity.TrancheAccruedPayment;
            residualTranche.MonthsToNextPayment = trancheDetailEntity.MonthsToNextPayment;
            residualTranche.PaymentFrequencyInMonths = trancheDetailEntity.PaymentFrequencyInMonths;
            residualTranche.IncludePaymentShortfall = trancheDetailEntity.IncludePaymentShortfall.GetValueOrDefault();
            residualTranche.IsShortfallPaidFromReserves = trancheDetailEntity.IsShortfallPaidFromReserves.GetValueOrDefault();

            if (trancheTypeInformation.IsResidualTranche)
            {
                residualTranche.AbsorbsRemainingAvailableFunds = true;
                residualTranche.AbsorbsAssociatedReservesReleased = true;
            }

            return residualTranche;
        }
    }
}
