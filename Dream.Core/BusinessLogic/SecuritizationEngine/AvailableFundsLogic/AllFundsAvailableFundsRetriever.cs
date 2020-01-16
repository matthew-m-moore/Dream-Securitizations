using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic
{
    public class AllFundsAvailableFundsRetriever : AvailableFundsRetriever
    {
        public AllFundsAvailableFundsRetriever() { }

        public override AvailableFundsRetriever Copy()
        {
            return new AllFundsAvailableFundsRetriever();
        }

        public override double RetrieveAvailableFundsForTranche(int monthlyPeriod, AvailableFunds availableFunds, SecuritizationNodeTree securitizationNode)
        {
            var fundsAvailableToAllocate = availableFunds[monthlyPeriod].TotalAvailableFunds;

            return fundsAvailableToAllocate;
        }
    }
}
