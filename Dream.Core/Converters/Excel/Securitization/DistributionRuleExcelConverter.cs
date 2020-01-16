using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;

namespace Dream.Core.Converters.Excel.Securitization
{
    public class DistributionRuleExcelConverter
    {
        private const string _proRataDistributionRule = "Pro-Rata";
        private const string _proRataFeesDistributionRule = "Pro-Rata Fees";
        private const string _proRataSubSequentialDistributionRule = "Pro-Rata Sub-Seq.";
        private const string _sequentialDistributionRule = "Sequential";

        public static DistributionRule DetermineDistributionRuleFromDescription(string distributionRuleDescription)
        {
            switch (distributionRuleDescription)
            {
                case _proRataDistributionRule:
                    return new InitialBalanceProRataDistributionRule();

                case _proRataFeesDistributionRule:
                    return new NextFeePaymentProRataDistributionRule();

                case _proRataSubSequentialDistributionRule:
                    return new SubSequentialProRataDistributionRule();

                case _sequentialDistributionRule:
                    return new SequentialDistributionRule();

                default:
                    return null;
            }
        }
    }
}
