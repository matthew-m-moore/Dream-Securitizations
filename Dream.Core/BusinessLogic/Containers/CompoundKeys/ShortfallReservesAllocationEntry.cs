using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.Containers.CompoundKeys
{
    public class ShortfallReservesAllocationEntry
    {
        public string SecuritizationNodeName { get; }
        public string ReserveFundAccountName { get; }
        public TrancheCashFlowType ReservesCashFlowType { get; }
        public TrancheCashFlowType TrancheCashFlowType { get; }

        public ShortfallReservesAllocationEntry(
            string securitizationNodeName, 
            string reserveFundAccountName, 
            TrancheCashFlowType reservesCashFlowType,
            TrancheCashFlowType trancheCashFlowType)
        {
            SecuritizationNodeName = securitizationNodeName;
            ReserveFundAccountName = reserveFundAccountName;
            ReservesCashFlowType = reservesCashFlowType;
            TrancheCashFlowType = trancheCashFlowType;
        }

        public static bool operator ==(
            ShortfallReservesAllocationEntry shortfallReservesAllocationEntryOne, 
            ShortfallReservesAllocationEntry shortfallReservesAllocationEntryTwo)
        {
            if (ReferenceEquals(shortfallReservesAllocationEntryOne, shortfallReservesAllocationEntryTwo))
            {
                return true;
            }

            if (ReferenceEquals(shortfallReservesAllocationEntryOne, null))
            {
                return false;
            }

            return shortfallReservesAllocationEntryOne.Equals(shortfallReservesAllocationEntryTwo);
        }

        public static bool operator !=(
            ShortfallReservesAllocationEntry shortfallReservesAllocationEntryOne, 
            ShortfallReservesAllocationEntry shortfallReservesAllocationEntryTwo)
        {
            return !(shortfallReservesAllocationEntryOne == shortfallReservesAllocationEntryTwo);
        }

        public override bool Equals(object obj)
        {
            // Note, if the obj was null, this safe caste would just return null
            var shortfallReservesAllocationEntry = obj as ShortfallReservesAllocationEntry;
            if (shortfallReservesAllocationEntry == null) return false;

            var isEqual = shortfallReservesAllocationEntry.SecuritizationNodeName == SecuritizationNodeName
                       && shortfallReservesAllocationEntry.ReservesCashFlowType == ReservesCashFlowType
                       && shortfallReservesAllocationEntry.ReserveFundAccountName == ReserveFundAccountName
                       && shortfallReservesAllocationEntry.TrancheCashFlowType == TrancheCashFlowType;

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
                hash = (hash * primeNumberTwo) + ReservesCashFlowType.GetHashCode();
                hash = (hash * primeNumberTwo) + ReserveFundAccountName.GetHashCode();
                hash = (hash * primeNumberTwo) + TrancheCashFlowType.GetHashCode();

                return hash;
            }
        }
    }
}
