using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using System;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic
{
    public class IrregularInterestRemittanceAvailableFundsRetriever : AvailableFundsRetriever
    {
        public int IrregularInterestCollectionFrequencyInMonths { get; }
        public DateTime IrregularInterestCollectionStartDate { get; }

        private int? _monthlyPeriodOfFirstCollectionDate = null;

        public IrregularInterestRemittanceAvailableFundsRetriever(DateTime irregularInterestCollectionStartDate) : base()
        {
            IrregularInterestCollectionFrequencyInMonths = 6;
            IrregularInterestCollectionStartDate = irregularInterestCollectionStartDate;
        }

        public IrregularInterestRemittanceAvailableFundsRetriever(
            int irregularInterestCollectionFrequencyInMonths,
            DateTime irregularInterestCollectionStartDate) 
        : base()
        {
            IrregularInterestCollectionFrequencyInMonths = irregularInterestCollectionFrequencyInMonths;
            IrregularInterestCollectionStartDate = irregularInterestCollectionStartDate;
        }

        public override AvailableFundsRetriever Copy()
        {
            return new IrregularInterestRemittanceAvailableFundsRetriever(
                IrregularInterestCollectionFrequencyInMonths,
                new DateTime(IrregularInterestCollectionStartDate.Ticks));
        }

        public override double RetrieveAvailableFundsForTranche(int monthlyPeriod, AvailableFunds availableFunds, SecuritizationNodeTree securitizationNode)
        {
            if (!_monthlyPeriodOfFirstCollectionDate.HasValue)
            {
                TryPopulateMonthlyPeriodOfFirstCollectionDate(monthlyPeriod, availableFunds);
            }

            var fundsAvailableToAllocate = availableFunds[monthlyPeriod].TotalAvailableFunds;

            var isCollectionPeriod = false;
            if (_monthlyPeriodOfFirstCollectionDate.HasValue)
            {
                var adjustedMonthlyPeriod = monthlyPeriod - _monthlyPeriodOfFirstCollectionDate.Value;
                isCollectionPeriod = MathUtility.CheckDivisibilityOfIntegers(
                    adjustedMonthlyPeriod,
                    IrregularInterestCollectionFrequencyInMonths);
            }

            if (!isCollectionPeriod)
            {
                var scheduledInterest = availableFunds[monthlyPeriod].AvailableInterest;
                var prepaymentInterest = availableFunds[monthlyPeriod].AvailablePrepaymentInterest;
                var interestRecoveries = availableFunds[monthlyPeriod].AvailableInterestRecoveries;

                fundsAvailableToAllocate -= scheduledInterest;
                fundsAvailableToAllocate -= prepaymentInterest;
                fundsAvailableToAllocate -= interestRecoveries;
            }

            return fundsAvailableToAllocate;
        }

        private void TryPopulateMonthlyPeriodOfFirstCollectionDate(int monthlyPeriod, AvailableFunds availableFunds)
        {
            if (availableFunds[monthlyPeriod].PeriodDate.Ticks == IrregularInterestCollectionStartDate.Ticks)
            {
                _monthlyPeriodOfFirstCollectionDate = monthlyPeriod;
            }
        }
    }
}
