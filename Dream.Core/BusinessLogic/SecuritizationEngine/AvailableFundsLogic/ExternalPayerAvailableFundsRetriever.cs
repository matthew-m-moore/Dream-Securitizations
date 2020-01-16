using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic
{
    public class ExternalPayerAvailableFundsRetriever : AvailableFundsRetriever
    {
        public ExternalPayerAvailableFundsRetriever() { }

        public override AvailableFundsRetriever Copy()
        {
            return new ExternalPayerAvailableFundsRetriever();
        }

        public override double RetrieveAvailableFundsForTranche(int monthlyPeriod, AvailableFunds availableFunds, SecuritizationNodeTree securitizationNode)
        {
            // It is assumed that an external payer will always have enough funds available to pay any tranche
            var maximumAmountDouble = double.MaxValue;
            return maximumAmountDouble;
        }
    }
}
