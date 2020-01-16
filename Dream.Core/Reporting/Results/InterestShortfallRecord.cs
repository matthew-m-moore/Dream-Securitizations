using System;

namespace Dream.Core.Reporting.Results
{
    public class InterestShortfallRecord
    {
        private const char _enDash = (char)0x2013;
        private const string _interestShortfallOccured = "Yes";

        public int FirstInterestShortfallPeriod { get; set; }
        public DateTime FirstInterestShortfallPeriodDate { get; set; }

        public int LastInterestShortfallPeriod { get; set; }
        public DateTime LastInterestShortfallPeriodDate { get; set; }

        public double MaximumInterestShortfall { get; set; }

        public override string ToString()
        {
            if (FirstInterestShortfallPeriodDate.Ticks > DateTime.MinValue.Ticks)
            {
                return _interestShortfallOccured + " " + _enDash + " Period: " + FirstInterestShortfallPeriod;
            }

            return string.Empty;
        }
    }
}
