using Dream.Common.Curves;
using Dream.Core.BusinessLogic.Containers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows
{
    public class PrepaymentPenaltyPlan
    {
        public string Description { get; private set; }

        private List<PrepaymentPenalty> _prepaymentPenaltyThresholds;

        public PrepaymentPenaltyPlan(string description)
        {
            Description = description;
            _prepaymentPenaltyThresholds = new List<PrepaymentPenalty>();
        }

        /// <summary>
        /// Adds a non-redundant prepayment penalty to the interal collection of penalties.
        /// </summary>
        public void AddPrepaymentPenalty(PrepaymentPenalty prepaymentPenalty)
        {
            if (_prepaymentPenaltyThresholds.Any(p => p.PenaltyEndYear == prepaymentPenalty.PenaltyEndYear))
            {
                throw new Exception(string.Format("ERROR: Cannot add duplicate ending year for prepayment penalty with description '{0}'",
                    Description));
            }

            _prepaymentPenaltyThresholds.Add(prepaymentPenalty);
        }

        /// <summary>
        /// Generates a penalty schedule from the underlying collection of prepayment penalties. The schedule is assumed to
        /// stay constant after it ends, hence the use a Curve object.
        /// </summary>
        public Curve<double> GeneratePenaltySchedule(int monthsOfSeasoning)
        {
            var penaltyScheduleList = new List<double> { 0.0 };
            var monthlyStartPeriod = 0;

            foreach (var prepaymentPenalty in _prepaymentPenaltyThresholds.OrderBy(p => p.PenaltyEndYear))
            {
                var monthlyEndPeriod = Math.Max(0, prepaymentPenalty.PenaltyEndMonth - monthsOfSeasoning);
                var monthsOfPenalty = monthlyEndPeriod - monthlyStartPeriod;

                var penaltySchedulePortion = Enumerable.Repeat(
                    prepaymentPenalty.PenaltyPercentageAmount,
                    monthsOfPenalty);

                penaltyScheduleList.AddRange(penaltySchedulePortion);

                monthlyStartPeriod = monthlyEndPeriod;
            }

            return new Curve<double>(penaltyScheduleList);
        }
    }
}
