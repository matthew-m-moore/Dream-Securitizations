using Dream.Common.Enums;
using System;
using System.Linq;

namespace Dream.Common.Curves
{
    public class InterestRateCurve
    {
        public InterestRateCurveType Type { get; set; }
        public DateTime MarketDate { get; set; }
        public Curve<double> RateCurve { get; set; }
        public int? TenorInMonths { get; set; }
        public DayCountConvention DayCountConvention { get; set; }

        public bool IsDiscountFactorCurve => Constants.DiscountFactorCurves.Contains(Type);
        public bool IsForwardCurve => (!Constants.DiscountFactorCurves.Contains(Type) &&
                                       !Constants.ZeroVolatilitySpotRateCurves.Contains(Type));

        public InterestRateCurve(
            InterestRateCurveType type, 
            DateTime marketDate,
            Curve<double> rateCurve, 
            int? tenorInMonths = null, 
            DayCountConvention dayCountConvention = DayCountConvention.None)
        {
            Type = type;
            MarketDate = marketDate;
            RateCurve = rateCurve;
            TenorInMonths = tenorInMonths;
            DayCountConvention = dayCountConvention;
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public InterestRateCurve Copy()
        {
            return new InterestRateCurve(Type,
                new DateTime(MarketDate.Ticks),
                new Curve<double>(RateCurve.ToList()),
                TenorInMonths,
                DayCountConvention);
        }

        /// <summary>
        /// Finds the appropriate discount factor type for the given curve type of the object.
        /// </summary>
        public InterestRateCurveType GetCorrespondingDiscountFactorCurveType()
        {
            switch(Type)
            {
                case InterestRateCurveType.LiborSpot:
                case InterestRateCurveType.Libor1Mo:
                case InterestRateCurveType.Libor2Mo:
                case InterestRateCurveType.Libor3Mo:
                case InterestRateCurveType.Libor6Mo:
                case InterestRateCurveType.Libor1Yr:
                case InterestRateCurveType.Swap6Mo:
                case InterestRateCurveType.Swap1Yr:
                case InterestRateCurveType.Swap2Yr:
                case InterestRateCurveType.Swap5Yr:
                case InterestRateCurveType.Swap7Yr:
                case InterestRateCurveType.Swap10Yr:
                case InterestRateCurveType.Swap15Yr:
                case InterestRateCurveType.Swap20Yr:
                case InterestRateCurveType.Swap30Yr:
                    return InterestRateCurveType.LiborDiscount;

                case InterestRateCurveType.Treasury1Mo:
                case InterestRateCurveType.Treasury3Mo:
                case InterestRateCurveType.Treasury1Yr:
                case InterestRateCurveType.Treasury5Yr:
                case InterestRateCurveType.Treasury7Yr:
                case InterestRateCurveType.Treasury10Yr:
                case InterestRateCurveType.Treasury30Yr:
                    return InterestRateCurveType.TreasuryDiscount;

                default:
                    return InterestRateCurveType.None;
            }
        }

        /// <summary>
        /// Finds the discount rate curve (i.e. zero rates) for the given curve type of the object.
        /// </summary>
        public InterestRateCurveType GetCorrespondingDiscountRateCurveType()
        {
            switch (Type)
            {
                case InterestRateCurveType.LiborDiscount:
                    return InterestRateCurveType.LiborSpot;

                case InterestRateCurveType.TreasuryDiscount:
                    return InterestRateCurveType.TreasurySpot;

                default:
                    return InterestRateCurveType.None;
            }
        }
    }
}
