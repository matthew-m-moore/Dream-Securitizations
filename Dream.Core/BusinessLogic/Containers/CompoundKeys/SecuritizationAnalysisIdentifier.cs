namespace Dream.Core.BusinessLogic.Containers
{
    public class SecuritizationAnalysisIdentifier
    {
        public int SecuritizationAnalysisDataSetId { get; }
        public int SecuritizationAnalysisVersionId { get; }

        public string UniqueStringIdentifer => SecuritizationAnalysisDataSetId.ToString() + ".v" + SecuritizationAnalysisVersionId.ToString();

        public SecuritizationAnalysisIdentifier(int securitizationAnalysisDataSetId)
        {
            SecuritizationAnalysisDataSetId = securitizationAnalysisDataSetId;
            SecuritizationAnalysisVersionId = default(int);
        }

        public SecuritizationAnalysisIdentifier(int securitizationAnalysisDataSetId, int securitizationAnalysisVersionId)
        {
            SecuritizationAnalysisDataSetId = securitizationAnalysisDataSetId;
            SecuritizationAnalysisVersionId = securitizationAnalysisVersionId;
        }

        public static bool operator ==(SecuritizationAnalysisIdentifier SecuritizationAnalysisIdentifierOne, SecuritizationAnalysisIdentifier SecuritizationAnalysisIdentifierTwo)
        {
            if (ReferenceEquals(SecuritizationAnalysisIdentifierOne, SecuritizationAnalysisIdentifierTwo))
            {
                return true;
            }

            if (ReferenceEquals(SecuritizationAnalysisIdentifierOne, null))
            {
                return false;
            }

            return SecuritizationAnalysisIdentifierOne.Equals(SecuritizationAnalysisIdentifierTwo);
        }

        public static bool operator !=(SecuritizationAnalysisIdentifier SecuritizationAnalysisIdentifierOne, SecuritizationAnalysisIdentifier SecuritizationAnalysisIdentifierTwo)
        {
            return !(SecuritizationAnalysisIdentifierOne == SecuritizationAnalysisIdentifierTwo);
        }

        public override bool Equals(object obj)
        {
            // Note, if the obj was null, this safe caste would just return null
            var SecuritizationAnalysisIdentifier = obj as SecuritizationAnalysisIdentifier;
            if (SecuritizationAnalysisIdentifier == null) return false;

            var isEqual = SecuritizationAnalysisIdentifier.SecuritizationAnalysisDataSetId == SecuritizationAnalysisDataSetId
                       && SecuritizationAnalysisIdentifier.SecuritizationAnalysisVersionId == SecuritizationAnalysisVersionId;

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
                hash = (hash * primeNumberTwo) + SecuritizationAnalysisDataSetId.GetHashCode();
                hash = (hash * primeNumberTwo) + SecuritizationAnalysisVersionId.GetHashCode();

                return hash;
            }
        }
    }
}
