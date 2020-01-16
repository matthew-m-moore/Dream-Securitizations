using System;

namespace Dream.Core.BusinessLogic.Containers
{
    public class DelayedFee
    {
        public double DelayedFeeValue { get; set; }
        public DateTime DelayedUntilDate { get; set; }
        public int DelayedUntilMonthlyPeriod { get; set; }

        public DelayedFee(double delayedFeeValue, DateTime delayedUntilDate)
        {
            DelayedFeeValue = delayedFeeValue;
            DelayedUntilDate = delayedUntilDate;
            DelayedUntilMonthlyPeriod = default(int);
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public DelayedFee Copy()
        {
            return new DelayedFee(DelayedFeeValue, DelayedUntilDate);
        }
    }
}
