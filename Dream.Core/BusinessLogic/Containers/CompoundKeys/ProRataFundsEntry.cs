using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.Containers
{
    public class ProRataFundsEntry
    {
        public string SecuritizationNodeName { get; }
        public TrancheCashFlowType TrancheCashFlowType { get; }
        public int MonthlyPeriod { get; }

        public ProRataFundsEntry(string securitizationNodeName, TrancheCashFlowType trancheCashFlowType, int monthlyPeriod)
        {
            SecuritizationNodeName = securitizationNodeName;
            TrancheCashFlowType = trancheCashFlowType;
            MonthlyPeriod = monthlyPeriod;
        }

        public static bool operator ==(ProRataFundsEntry proRataFundsEntryOne, ProRataFundsEntry proRataFundsEntryTwo)
        {
            if (ReferenceEquals(proRataFundsEntryOne, proRataFundsEntryTwo))
            {
                return true;
            }

            if (ReferenceEquals(proRataFundsEntryOne, null))
            {
                return false;
            }

            return proRataFundsEntryOne.Equals(proRataFundsEntryTwo);
        }

        public static bool operator !=(ProRataFundsEntry proRataFundsEntryOne, ProRataFundsEntry proRataFundsEntryTwo)
        {
            return !(proRataFundsEntryOne == proRataFundsEntryTwo);
        }

        public override bool Equals(object obj)
        {
            // Note, if the obj was null, this safe caste would just return null
            var proRataFundsEntry = obj as ProRataFundsEntry;
            if (proRataFundsEntry == null) return false;

            var isEqual = proRataFundsEntry.SecuritizationNodeName == SecuritizationNodeName
                       && proRataFundsEntry.TrancheCashFlowType == TrancheCashFlowType
                       && proRataFundsEntry.MonthlyPeriod == MonthlyPeriod;

            return isEqual;
        }

        /// <summary>
        /// Note, this implemenatation was informed by the following thread:
        /// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int primeNumberOne = 13;
                int primeNumberTwo = 5;

                var hash = primeNumberOne;
                hash = (hash * primeNumberTwo) + SecuritizationNodeName.GetHashCode();
                hash = (hash * primeNumberTwo) + TrancheCashFlowType.GetHashCode();
                hash = (hash * primeNumberTwo) + MonthlyPeriod.GetHashCode();

                return hash;
            }
        }
    }
}
