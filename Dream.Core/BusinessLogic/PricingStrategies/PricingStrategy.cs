using Dream.Common;
using Dream.Common.Curves;
using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.Stratifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.PricingStrategies
{
    /// <summary>
    /// This abstract class contains all the functionality needed for pricing a list of cash flows and deriving some common pricing metrics,
    /// such as duration.
    /// </summary>
    public abstract class PricingStrategy
    {
        public double? Price { get; protected set; }
        public double? PresentValue { get; protected set; }
        public double? InternalRateOfReturn { get; protected set; }
        public double? InterpolatedRate { get; protected set; }
        public double? NominalSpread { get; protected set; }
        public double? SpreadOverCurve { get; protected set; }
        public double? SpreadDuration { get; protected set; }

        public MarketDataGrouping MarketDataUsedForNominalSpread { get; protected set; }
        public InterestRateCurveType CurveTypeUsedForSpreadCalculation { get; protected set; }
        public InterestRateCurve CurveUsedForSpreadCalculation { get; protected set; }

        public double? MacaulayDuration { get; protected set; }
        public double? ModifiedDuration { get; protected set; }
        public double? EffectiveDuration { get; protected set; }
        public double? DollarDuration { get; protected set; }
        public double? EffectiveDollarDuration { get; protected set; }      

        public DayCountConvention DayCountConvention { get; private set; }
        public CompoundingConvention CompoundingConvention { get; private set; }
        public double PricingValueMultiplier { get; private set; }

        protected PricingStrategy()
        {
            DayCountConvention = DayCountConvention.None;
            CompoundingConvention = CompoundingConvention.None;
            PricingValueMultiplier = 1.0;
        }

        public PricingStrategy(DayCountConvention dayCountConvention, CompoundingConvention compoundingCovention, double pricingValueMulitplier = 1.0)
        {
            DayCountConvention = dayCountConvention;
            CompoundingConvention = compoundingCovention;
            PricingValueMultiplier = pricingValueMulitplier;
        }

        public abstract PricingStrategy Copy();
        public abstract double CalculatePresentValue<T>(List<T> cashFlows) where T : CashFlow;       
        public abstract double CalculateShockedPresentValue<T>(List<T> cashFlows, double shockSizeInDecimal) where T : CashFlow;

        /// <summary>
        /// Finds the percentage of the initial balance that the present value determined represents.
        /// </summary>
        public virtual double CalculatePrice<T>(List<T> cashFlows) where T : CashFlow
        {
            var contractualCashFlow = cashFlows.First() as ContractualCashFlow;
            if (contractualCashFlow == null) return double.NaN;

            var presentValue = PresentValue ?? CalculatePresentValue(cashFlows);
            var balance = contractualCashFlow.StartingBalance;

            var price = presentValue / balance;

            Price = price;
            return price;
        }

        /// <summary>
        /// Finds the internal rate of return that will set the cash flows given to a specific present value;
        /// </summary>
        public virtual double CalculateInternalRateOfReturn<T>(List<T> cashFlows) where T : CashFlow
        {
            var targetValueForSearch = PresentValue ?? CalculatePresentValue(cashFlows);
            var targetPrecision = 1e-6;

            var internalRateOfReturn = NumericalSearchUtility.NewtonRaphsonWithBisection(
                yield => DiscountCashFlows(yield, cashFlows),
                targetValueForSearch,
                targetPrecision);

            InternalRateOfReturn = internalRateOfReturn;
            return internalRateOfReturn;
        }

        /// <summary>
        /// Finds the fixed spread over a specified interest rate curve that will set the cash flows given to a specific present value.
        /// Either a forward-spread or a zero-volatility spread can be found currently, not both.
        /// </summary>
        public virtual double CalculateSpread<T>(List<T> cashFlows, MarketRateEnvironment rateEnvironment, InterestRateCurveType interestRateCurveType) 
            where T : CashFlow
        {
            if (rateEnvironment == null || interestRateCurveType == InterestRateCurveType.None) return double.NaN;
            if (!rateEnvironment.RateCurveDictionary.ContainsKey(interestRateCurveType))
            {
                throw new Exception("ERROR: The rate environment provided does not include the interest rate curve type specified");
            }

            var interestRateCurve = rateEnvironment[interestRateCurveType];
            var discountingFunction = DetermineDiscountingFunctionForSpread(interestRateCurve, cashFlows);

            var targetValueForSearch = PresentValue ?? CalculatePresentValue(cashFlows);
            var targetPrecision = 1e-6;

            var spreadOverRateCurve = NumericalSearchUtility.NewtonRaphsonWithBisection(
                spread => discountingFunction(spread, interestRateCurve, cashFlows),
                targetValueForSearch,
                targetPrecision);

            SpreadOverCurve = spreadOverRateCurve;
            CurveTypeUsedForSpreadCalculation = interestRateCurveType;
            CurveUsedForSpreadCalculation = interestRateCurve;
            return spreadOverRateCurve;
        }

        /// <summary>
        /// Finds the fixed spread over a set of market rate data at the point equal to the WAL of the cash flows that will set 
        /// the cash flows given to a specific present value. When the market rate data is a swaps, this colloquialy called an
        /// "N-Spread". Then the market rate data is treasuries, this is often called an "I-Spread".
        /// </summary>
        public virtual double CalculateNominalSpread<T>(List<T> cashFlows, MarketRateEnvironment rateEnvironment, MarketDataGrouping marketDataGrouping) where T : CashFlow
        {
            if (rateEnvironment == null || marketDataGrouping == MarketDataGrouping.None) return double.NaN;
            var linearlyInterpolatedRate = GetLinearlyInterpolatedRate(cashFlows, rateEnvironment, marketDataGrouping);

            var targetValueForSearch = PresentValue ?? CalculatePresentValue(cashFlows);
            var targetPrecision = 1e-6;

            var nominalSpread = NumericalSearchUtility.NewtonRaphsonWithBisection(
                spread => DiscountCashFlows(spread + linearlyInterpolatedRate, cashFlows),
                targetValueForSearch,
                targetPrecision);

            MarketDataUsedForNominalSpread = marketDataGrouping;
            InterpolatedRate = linearlyInterpolatedRate;
            NominalSpread = nominalSpread;
            return nominalSpread;
        }

        /// <summary>
        /// Calculates the expected percentage change in price for a 1.0% change in spread numerically. Note, this is sometimes
        /// called a "CS01" or "Credit Spread 01".
        /// </summary>
        public virtual double CalculateSpreadDuration<T>(
            List<T> cashFlows, 
            MarketRateEnvironment rateEnvironment, 
            InterestRateCurveType interestRateCurveType, 
            double shockSizeInDecimal) where T : CashFlow
        {
            if (rateEnvironment == null || interestRateCurveType == InterestRateCurveType.None) return double.NaN;

            var baseSpread = 0.0;
            if (interestRateCurveType == CurveTypeUsedForSpreadCalculation)
            {
                baseSpread = SpreadOverCurve ?? CalculateSpread(cashFlows, rateEnvironment, interestRateCurveType);
            }
            else
            {
                baseSpread = CalculateSpread(cashFlows, rateEnvironment, interestRateCurveType);
            }

            var interestRateCurve = rateEnvironment[interestRateCurveType];
            var discountingFunction = DetermineDiscountingFunctionForSpread(interestRateCurve, cashFlows);

            var upShockedSpread = baseSpread + shockSizeInDecimal;
            var downShockedSpread = baseSpread - shockSizeInDecimal;

            var unShockedPresentValue = discountingFunction(baseSpread, interestRateCurve, cashFlows);
            var upShockedPresentValue = discountingFunction(upShockedSpread, interestRateCurve, cashFlows);
            var downShockedPresentValue = discountingFunction(downShockedSpread, interestRateCurve, cashFlows);

            var numerator = downShockedPresentValue - upShockedPresentValue;
            var denominator = 2.0 * shockSizeInDecimal * unShockedPresentValue;
            var spreadDuration = numerator / denominator;

            SpreadDuration = spreadDuration;
            return spreadDuration;
        }

        /// <summary>
        /// Calculates the time-weighted present value of a specified set of future cash flows.
        /// </summary>
        public virtual double CalculateMacaulayDuration<T>(List<T> cashFlows) where T : CashFlow
        {
            var firstCashFlow = cashFlows.First();
            var totalPresentValue = PresentValue ?? CalculatePresentValue(cashFlows);

            var sumOfTimeWeightedPresentValues = 0.0;         
            foreach (var cashFlow in cashFlows)
            {
                var timeExpiredInYears = DateUtility.CalculateTimePeriodInYears(
                    DayCountConvention,
                    firstCashFlow.PeriodDate,
                    cashFlow.PeriodDate);

                var presentValueOfSpecificCashFlow = CalculatePresentValue(new List<CashFlow> { firstCashFlow, cashFlow });
                var timeWeightedPresentValue = timeExpiredInYears * presentValueOfSpecificCashFlow;
                sumOfTimeWeightedPresentValues += timeWeightedPresentValue;              
            }

            var macaulayDuration = sumOfTimeWeightedPresentValues / totalPresentValue;

            ClearCachedValues();
            MacaulayDuration = macaulayDuration;
            return macaulayDuration;
        }

        /// <summary>
        /// Calculates the expected percentage change in price for a 1.0% change in rates analytically for cases where the
        /// internal rate of return is periodically or continously compounded.
        /// </summary>
        public virtual double CalculateModifiedDuration<T>(List<T> cashFlows) where T : CashFlow
        {
            var macaulayDuration = MacaulayDuration ?? CalculateMacaulayDuration(cashFlows);
            if (CompoundingConvention == CompoundingConvention.Continuously) return macaulayDuration;

            var internalRateOfReturn = CalculateInternalRateOfReturn(cashFlows);
            var compoundingFactor = GetCompoundingFactor();

            var divisor = 1 + (internalRateOfReturn / compoundingFactor);
            var modifiedDuration = macaulayDuration / divisor;

            ModifiedDuration = modifiedDuration;
            return modifiedDuration;
        }

        /// <summary>
        /// Calculates the expected percentage change in price for a 1.0% change in rates numerically, ignoring any possible
        /// change in cash flows associated with that change in rates.
        /// </summary>
        public virtual double CalculateModifiedDuration<T>(List<T> cashFlows, double shockSizeInDecimal) where T : CashFlow
        {
            var unShockedPresentValue = CalculatePresentValue(cashFlows);
            var upShockedPresentValue = CalculateShockedPresentValue(cashFlows, 1.0 * shockSizeInDecimal);
            var downShockedPresentValue = CalculateShockedPresentValue(cashFlows, -1.0 * shockSizeInDecimal);

            var numerator = downShockedPresentValue - upShockedPresentValue;
            var denominator = 2.0 * shockSizeInDecimal * unShockedPresentValue;
            var modifiedDuration = numerator / denominator;

            ModifiedDuration = modifiedDuration;
            return modifiedDuration;
        }   

        /// <summary>
        /// Calculates the expected percentage change in price for a 1.0% change in rates, taking into account the
        /// change in cash flows associated with that change in rates.
        /// </summary>
        public double CalculateEffectiveDuration<T>(
            List<T> unShockedCashFlows,
            List<T> upShockedCashFlows, 
            List<T> downShockedCashFlows, 
            double shockSizeInDecimal) where T : CashFlow
        {
            var unShockedPresentValue = CalculatePresentValue(unShockedCashFlows);
            var upShockedPresentValue = CalculateShockedPresentValue(upShockedCashFlows, shockSizeInDecimal);
            var downShockedPresentValue = CalculateShockedPresentValue(downShockedCashFlows, shockSizeInDecimal);

            var numerator = downShockedPresentValue - upShockedPresentValue;
            var denominator = 2.0 * shockSizeInDecimal * unShockedPresentValue;
            var effectiveDuration = numerator / denominator;

            EffectiveDuration = effectiveDuration;
            return effectiveDuration;
        }

        /// <summary>
        /// Calculates the dollar duration (or "DV01") which is the expected price change for a 1.0 bp change in rates.
        /// Note that 10,000 times this value divided by the present value equals the modified duration.
        /// </summary>
        public double CalculateDollarDuration<T>(List<T> cashFlows) where T : CashFlow
        {
            var presentValue = PresentValue ?? CalculatePresentValue(cashFlows);
            var modifiedDuration = ModifiedDuration ?? CalculateModifiedDuration(cashFlows);

            var dollarDuration = (presentValue * modifiedDuration) / Constants.BpsPerOneHundredPercentagePoints;

            DollarDuration = dollarDuration;
            return dollarDuration;
        }

        /// <summary>
        /// Calculates the effecive dollar duration (or "DV01") which is the expected price change for a 1.0 bp change in rates.
        /// Note that 10,000 times this value divided by the present value equals the effective duration.
        /// </summary>
        public double CalculateEffectiveDollarDuration<T>(
            List<T> unShockedCashFlows,
            List<T> upShockedCashFlows,
            List<T> downShockedCashFlows,
            double shockSizeInDecimal) where T : CashFlow
        {
            var presentValue = CalculatePresentValue(unShockedCashFlows);
            var effectiveDuration = CalculateEffectiveDuration(
                unShockedCashFlows, 
                upShockedCashFlows, 
                downShockedCashFlows, 
                shockSizeInDecimal);

            var effectiveDollarDuration = (presentValue * effectiveDuration) / Constants.BpsPerOneHundredPercentagePoints;

            EffectiveDollarDuration = effectiveDollarDuration;
            return effectiveDollarDuration;
        }

        /// <summary>
        /// Clears all cached values of the specified pricing strategy object.
        /// </summary>
        public virtual void ClearCachedValues()
        {
            Price = null;
            PresentValue = null;
            InternalRateOfReturn = null;
            InterpolatedRate = null;
            NominalSpread = null;
            SpreadOverCurve = null;
            SpreadDuration = null;

            CurveTypeUsedForSpreadCalculation = InterestRateCurveType.None;
            MarketDataUsedForNominalSpread = MarketDataGrouping.None;
            CurveUsedForSpreadCalculation = null;

            MacaulayDuration = null;
            ModifiedDuration = null;
            EffectiveDuration = null;
            DollarDuration = null;
            EffectiveDollarDuration = null;
        }

        protected InterestRateCurve ConvertDiscountFactorsToDiscountRates<T>(List<T> cashFlows, InterestRateCurve interestRateCurve) where T : CashFlow
        {
            var discountRateList = new List<double>();
            var cashFlowCount = cashFlows.Count;
            var initialPeriodDate = cashFlows.First().PeriodDate;

            for (var periodCounter = 0; periodCounter < cashFlowCount; periodCounter++)
            {
                var discountRate = ConvertDiscountFactorToDiscountRate(
                    interestRateCurve.RateCurve[periodCounter],
                    cashFlows[periodCounter].PeriodDate,
                    initialPeriodDate);

                discountRateList.Add(discountRate);
            }

            var discountRateCurve = new Curve<double>(discountRateList);
            var discountRateCurveType = interestRateCurve.GetCorrespondingDiscountRateCurveType();
            return new InterestRateCurve(discountRateCurveType, initialPeriodDate, discountRateCurve, interestRateCurve.TenorInMonths);
        }

        private InterestRateCurve ConvertForwardRatesToDiscountFactors<T>(List<T> cashFlows, InterestRateCurve interestRateCurve) where T : CashFlow
        {
            if (!interestRateCurve.TenorInMonths.HasValue)
            {
                throw new Exception("ERROR: The tenor of the rate curve was not properly set. Discount factors cannot be computed from this curve.");
            }

            var discountFactorList = new List<double>();
            var cashFlowCount = cashFlows.Count;
            var currentPeriodDate = cashFlows.First().PeriodDate;
            var currentDiscountFactor = 1.0;
            discountFactorList.Add(currentDiscountFactor);

            // The approriate math is the following:
            // DF = 1 / ((1 + rf_1)*(1 + rf_2)* ... *(1 + rf_T))
            // But, a quicker way to approach that is to recognize:
            // DF_i = DF_i-1 / (1 + rf_i)
            // Where, "rf" is the forward rate.
            for (var periodCounter = 1; periodCounter < cashFlowCount; periodCounter++)
            {
                var timePeriodInYears = DateUtility.CalculateTimePeriodInYears(
                    DayCountConvention,
                    currentPeriodDate,
                    cashFlows[periodCounter].PeriodDate);

                var forwardRatePeriodInYears = DateUtility.CalculateTimePeriodInYears(
                    interestRateCurve.DayCountConvention,
                    currentPeriodDate,
                    currentPeriodDate.AddMonths(interestRateCurve.TenorInMonths.Value));

                // The compounded forward rate factor below is the (1 + rf_i) from the math above
                var forwardRate = interestRateCurve.RateCurve[periodCounter];
                var compoundedForwardRateFactor = Math.Pow(1.0 + (forwardRate * forwardRatePeriodInYears), timePeriodInYears / forwardRatePeriodInYears);

                // Note, that the starting curent discount factor is always assumed to be unity
                var discountFactor = currentDiscountFactor / compoundedForwardRateFactor;

                discountFactorList.Add(discountFactor);
                currentPeriodDate = cashFlows[periodCounter].PeriodDate;
                currentDiscountFactor = discountFactor;
            }

            var discountFactorCurve = new Curve<double>(discountFactorList);
            var discountRateCurveType = interestRateCurve.GetCorrespondingDiscountFactorCurveType();
            return new InterestRateCurve(discountRateCurveType, currentPeriodDate, discountFactorCurve, interestRateCurve.TenorInMonths);
        }

        protected double GetLinearlyInterpolatedRate<T>(List<T> cashFlows, MarketRateEnvironment rateEnvironment, MarketDataGrouping marketDataGrouping) where T : CashFlow
        {
            if (!rateEnvironment.MarketDataPointsDictionary.ContainsKey(marketDataGrouping))
            {
                throw new Exception("ERROR: The rate environment provided does not include the market data specified");
            }

            var contractualCashFlows = cashFlows.OfType<ContractualCashFlow>().ToList();
            if (!contractualCashFlows.Any())
            {
                throw new Exception("ERROR: None of the cash flows provided are a cash flow with a principal payment");
            }

            var weightedAverageLife = CashFlowMetrics.CalculateWeightedAverageLife(contractualCashFlows);
            var marketRateData = rateEnvironment.MarketDataPointsDictionary[marketDataGrouping];
            var orderedMarketRateData = marketRateData.OrderBy(d => d.TenorInMonths);

            var xValues = orderedMarketRateData.Select(d => d.TenorInMonths.GetValueOrDefault() / Constants.MonthsInOneYear).ToList();
            var yValues = orderedMarketRateData.Select(d => d.Value).ToList();
            if (weightedAverageLife > xValues.Max()) return double.NaN;

            var addOriginPoint = !(xValues.First() == 0 && yValues.First() == 0.0);
            var linearlyInterpolatedRate =
                InterpolationUtility.LinearlyInterpolate(weightedAverageLife, xValues, yValues, addOriginPoint);

            return linearlyInterpolatedRate;
        }

        protected double DiscountCashFlows<T>(double yieldToMaturity, List<T> cashFlows) where T : CashFlow
        {
            var firstCashFlow = cashFlows.First();
            var totalPresentValue = 0.0;
            foreach (var cashFlow in cashFlows)
            {
                var presentValue = DiscountCashFlow(yieldToMaturity, firstCashFlow.PeriodDate, cashFlow);
                totalPresentValue += presentValue;
            }

            return totalPresentValue;
        }

        protected double DiscountCashFlowsWithZeroVolatilitySpread<T>(double spreadOverCurve, InterestRateCurve interestRateCurve, List<T> cashFlows) where T : CashFlow
        {
            var firstCashFlow = cashFlows.First();
            var cashFlowCount = cashFlows.Count;

            var totalPresentValue = 0.0;
            for (var monthlyPeriod = 0; monthlyPeriod < cashFlowCount; monthlyPeriod++)
            {
                var interestRate = interestRateCurve.RateCurve[monthlyPeriod];
                var discountRate = interestRate + spreadOverCurve;
                var cashFlow = cashFlows[monthlyPeriod];

                var presentValue = DiscountCashFlow(discountRate, firstCashFlow.PeriodDate, cashFlow);
                totalPresentValue += presentValue;
            }

            return totalPresentValue;
        }

        protected double DiscountCashFlowsWithForwardCurveSpread<T>(double spreadOverCurve, InterestRateCurve interestRateCurve, List<T> cashFlows) where T : CashFlow
        {
            var firstCashFlow = cashFlows.First();
            var cashFlowCount = cashFlows.Count;
            var discountFactors = ConvertForwardRatesToDiscountFactors(cashFlows, interestRateCurve);

            var totalPresentValue = 0.0;
            for (var monthlyPeriod = 0; monthlyPeriod < cashFlowCount; monthlyPeriod++)
            {
                var cashFlow = cashFlows[monthlyPeriod];
                var discountFactor = discountFactors.RateCurve[monthlyPeriod];

                var presentValue = cashFlow.Payment * discountFactor;
                totalPresentValue += presentValue;
            }

            return totalPresentValue;
        }

        protected Func<double, InterestRateCurve, List<T>, double> DetermineDiscountingFunctionForSpread<T>(
            InterestRateCurve interestRateCurve,
            List<T> cashFlows) where T : CashFlow
        {
            Func<double, InterestRateCurve, List<T>, double> discountingFunction = DiscountCashFlowsWithZeroVolatilitySpread;
            if (interestRateCurve.IsForwardCurve)
            {
                discountingFunction = DiscountCashFlowsWithForwardCurveSpread;
            }
            if (interestRateCurve.IsDiscountFactorCurve)
            {
                interestRateCurve = ConvertDiscountFactorsToDiscountRates(cashFlows, interestRateCurve);
            }

            return discountingFunction;
        }

        private double DiscountCashFlow<T>(double discountRate, DateTime presentValueDate, T cashFlow) where T : CashFlow
        {
            var timeExpiredInYears = DateUtility.CalculateTimePeriodInYears(
                DayCountConvention,
                presentValueDate,
                cashFlow.PeriodDate);

            var accrualFactor = 0.0;
            if (timeExpiredInYears > 0.0)
            {
                accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                    DayCountConvention,
                    CompoundingConvention,
                    timeExpiredInYears,
                    discountRate);
            }

            var discountFactor = 1.0 / (1.0 + accrualFactor);
            var presentValue = cashFlow.Payment * discountFactor;
            return presentValue;
        }

        private double ConvertDiscountFactorToDiscountRate(double discountFactor, DateTime periodDate, DateTime initialPeriodDate)
        {
            var timePeriodInYears = DateUtility.CalculateTimePeriodInYears(
                DayCountConvention,
                initialPeriodDate,
                periodDate);

            var discountRate = MathUtility.CalculateDiscountRateFromDiscountFactor(
                DayCountConvention,
                CompoundingConvention,
                timePeriodInYears,
                discountFactor);

            return discountRate;
        }

        private int GetCompoundingFactor()
        {
            switch (CompoundingConvention)
            {
                case CompoundingConvention.Daily:
                    return MathUtility.GetAssumedNumberOfDaysInOneYear(DayCountConvention);

                case CompoundingConvention.Annually:
                case CompoundingConvention.SemiAnnually:
                case CompoundingConvention.Quarterly:
                case CompoundingConvention.Monthly:
                    return (int) CompoundingConvention;

                default:
                    throw new Exception("ERROR: Unhandled compounding convention in the calculation of modified duration");
            }
        }
    }
}
