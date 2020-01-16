using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic
{
    /// <summary>
    /// A an abstract class that determines what types of funds are pulled from the available funds object. For example, only
    /// principal remittances, only prepayments, etc..
    /// </summary>
    public abstract class AvailableFundsRetriever
    {
        public abstract double RetrieveAvailableFundsForTranche(int monthlyPeriod, AvailableFunds availableFunds, SecuritizationNodeTree securitizationNode);
        public abstract AvailableFundsRetriever Copy();
    }
}
