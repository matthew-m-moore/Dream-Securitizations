using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using System;

namespace Dream.Core.Converters.Database.Securitization
{
    public class TrancheTypeDatabaseConverter
    {
        private const string _fixedRateTranche = "Fixed Rate Tranche";
        private const string _floatingRateTranche = "Floating Rate Tranche";
        private const string _inverseFloatingRateTranche = "Inverse Floating Rate Tranche";
        private const string _fixedRateResidualTranche = "Fixed Rate NIM Tranche";
        private const string _floatingRateResidualTranche = "Floating Rate NIM Tranche";
        private const string _inverseFloatingRateResidualTranche = "Inverse Floating Rate NIM Tranche";

        private const string _interestOnlyTranche = "Interest-Only (IO) Tranche";
        private const string _principalOnlyTranche = "Principal-Only (PO) Tranche";
        private const string _plannedAmortClassTranche = "Planned Amoritization Class (PAC) Tranche";
        private const string _targetedAmortClassTranche = "Targeted Amoritization Class (PAC) Tranche";

        private const string _residualTranche = "NIM-let Tranche";
        private const string _passThruTranche = "Pass-Thru Tranche";

        private const string _flatFeeTranche = "Flat Fee";
        private const string _initialAccrualFlatFeeTranche = "Flat Fee w/Pro-Rata Initial Accrual";
        private const string _increasingFeeTranche = "Periodically Increasing Fee";
        private const string _bondCountBasedFeeTranche = "Bond Count Based Fee";
        private const string _percentOfTrancheBalanceFeeTranche = "Percent of Tranche Balance Fee";
        private const string _percentOfCollateralBalanceFeeTranche = "Percent of Collateral Balance Fee";
        private const string _specialPercentOfCollateralBalanceFeeTranche = "RA Advance Fee";

        private const string _dollarCappedReserveFundTranche = "Dollar Amount Capped Reserve Fund";
        private const string _percentageCappedReserveTranche = "Percent of Collateral Capped Reserve Fund";

        public static (Type TrancheType, bool IsResidualTranche) ConvertString(string trancheTypeText)
        {
            switch (trancheTypeText)
            {
                case _fixedRateTranche:
                    return (TrancheType: typeof(FixedRateTranche), IsResidualTranche: false);

                case _floatingRateTranche:
                    return (TrancheType: typeof(FloatingRateTranche), IsResidualTranche: false);

                case _inverseFloatingRateTranche:
                    return (TrancheType: typeof(InverseFloatingRateTranche), IsResidualTranche: false);

                case _fixedRateResidualTranche:
                    return (TrancheType: typeof(FixedRateTranche), IsResidualTranche: true);

                case _floatingRateResidualTranche:
                    return (TrancheType: typeof(FloatingRateTranche), IsResidualTranche: true);

                case _inverseFloatingRateResidualTranche:
                    return (TrancheType: typeof(InverseFloatingRateTranche), IsResidualTranche: true);

                case _interestOnlyTranche:
                    return (TrancheType: typeof(InterestOnlyTranche), IsResidualTranche: false);

                case _principalOnlyTranche:
                    return (TrancheType: typeof(PrincipalOnlyTranche), IsResidualTranche: false);

                case _plannedAmortClassTranche:
                    return (TrancheType: typeof(PlannedAmortizationClassTranche), IsResidualTranche: false);

                case _targetedAmortClassTranche:
                    return (TrancheType: typeof(TargetedAmortizationClassTranche), IsResidualTranche: false);

                case _residualTranche:
                case _passThruTranche:
                    return (TrancheType: typeof(ResidualTranche), IsResidualTranche: true);

                case _flatFeeTranche:
                    return (TrancheType: typeof(FlatFeeTranche), IsResidualTranche: false);

                case _initialAccrualFlatFeeTranche:
                    return (TrancheType: typeof(InitialPeriodSpecialAccrualFeeTranche), IsResidualTranche: false);

                case _increasingFeeTranche:
                    return (TrancheType: typeof(PeriodicallyIncreasingFeeTranche), IsResidualTranche: false);

                case _bondCountBasedFeeTranche:
                    return (TrancheType: typeof(BondCountBasedFeeTranche), IsResidualTranche: false);

                case _percentOfTrancheBalanceFeeTranche:
                    return (TrancheType: typeof(PercentOfTrancheBalanceFeeTranche), IsResidualTranche: false);

                case _percentOfCollateralBalanceFeeTranche:
                    return (TrancheType: typeof(PercentOfCollateralBalanceFeeTranche), IsResidualTranche: false);

                case _specialPercentOfCollateralBalanceFeeTranche:
                    return (TrancheType: typeof(SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche), IsResidualTranche: false);

                case _dollarCappedReserveFundTranche:
                    return (TrancheType: typeof(CappedReserveFundTranche), IsResidualTranche: false);

                case _percentageCappedReserveTranche:
                    return (TrancheType: typeof(PercentOfCollateralBalanceCappedReserveFundTranche), IsResidualTranche: false);

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The tranche type '{0}' is not supported. Please report this error.",
                        trancheTypeText));
            }
        }

        public static string ConvertType(Type trancheType, bool isResidualTranche)
        {
            if (trancheType == typeof(FixedRateTranche) && !isResidualTranche)
                return _fixedRateTranche;

            if (trancheType == typeof(FloatingRateTranche) && !isResidualTranche)
                return _floatingRateTranche;

            if (trancheType == typeof(InverseFloatingRateTranche) && !isResidualTranche)
                return _inverseFloatingRateTranche;

            if (trancheType == typeof(FixedRateTranche) && isResidualTranche)
                return _fixedRateResidualTranche;

            if (trancheType == typeof(FloatingRateTranche) && isResidualTranche)
                return _floatingRateResidualTranche;

            if (trancheType == typeof(InverseFloatingRateTranche) && isResidualTranche)
                return _inverseFloatingRateResidualTranche;

            if (trancheType == typeof(InterestOnlyTranche) && !isResidualTranche)
                return _interestOnlyTranche;

            if (trancheType == typeof(PrincipalOnlyTranche) && !isResidualTranche)
                return _principalOnlyTranche;

            if (trancheType == typeof(PlannedAmortizationClassTranche) && !isResidualTranche)
                return _plannedAmortClassTranche;

            if (trancheType == typeof(TargetedAmortizationClassTranche) && !isResidualTranche)
                return _targetedAmortClassTranche;

            if (trancheType == typeof(ResidualTranche) && isResidualTranche)
                return _residualTranche;

            if (trancheType == typeof(FlatFeeTranche) && !isResidualTranche)
                return _flatFeeTranche;

            if (trancheType == typeof(InitialPeriodSpecialAccrualFeeTranche) && !isResidualTranche)
                return _initialAccrualFlatFeeTranche;

            if (trancheType == typeof(PeriodicallyIncreasingFeeTranche) && !isResidualTranche)
                return _increasingFeeTranche;

            if (trancheType == typeof(BondCountBasedFeeTranche) && !isResidualTranche)
                return _bondCountBasedFeeTranche;

            if (trancheType == typeof(PercentOfTrancheBalanceFeeTranche) && !isResidualTranche)
                return _percentOfTrancheBalanceFeeTranche;

            if (trancheType == typeof(PercentOfCollateralBalanceFeeTranche) && !isResidualTranche)
                return _percentOfCollateralBalanceFeeTranche;

            if (trancheType == typeof(SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche) && !isResidualTranche)
                return _specialPercentOfCollateralBalanceFeeTranche;

            if (trancheType == typeof(CappedReserveFundTranche) && !isResidualTranche)
                return _dollarCappedReserveFundTranche;

            if (trancheType == typeof(PercentOfCollateralBalanceCappedReserveFundTranche) && !isResidualTranche)
                return _percentageCappedReserveTranche;

            throw new Exception(string.Format("INTERNAL ERROR: The tranche type '{0}' is not supported. Please report this error.",
                trancheType));
        }
    }
}
