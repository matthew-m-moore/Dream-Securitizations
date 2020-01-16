using System;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;

namespace Dream.Core.Converters.Database.Securitization
{
    public class DistributionRuleDatabaseConverter
    {
        private const string _proRataDistributionRule = "Pro-Rata";
        private const string _proRataFeesDistributionRule = "Pro-Rata Fees";
        private const string _proRataSubSequentialDistributionRule = "Pro-Rata Sub-Sequential";
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
                    throw new Exception(string.Format("INTERNAL ERROR: No such funds distribution rule is implemented for the type called '{0}'",
                        distributionRuleDescription));
            }
        }

        public static string DetermineDescriptionFromDistributionRule(Type distributionRuleType)
        {
            if (distributionRuleType == typeof(InitialBalanceProRataDistributionRule))
                return _proRataDistributionRule;

            if (distributionRuleType == typeof(NextFeePaymentProRataDistributionRule))
                return _proRataFeesDistributionRule;

            if (distributionRuleType == typeof(SubSequentialProRataDistributionRule))
                return _proRataSubSequentialDistributionRule;

            if (distributionRuleType == typeof(SequentialDistributionRule))
                return _sequentialDistributionRule;


            throw new Exception(string.Format("INTERNAL ERROR: No such funds distribution description exists for the type called '{0}'",
                distributionRuleType));
        }
    }
}
