export class SecuritizationTrancheModelEntry {
    SecuritizationTrancheId: number;
    SecuritizationTrancheName: string;
    SecuritizationNodeId: number;
    SecuritizationNodeName: string;
    SecuritizationTrancheTypeDescription: string;
    SecuritizationTrancheBalance: number;
    SecuritizationTrancheInitialCoupon: number;
    AccruedInterest: number;
    FirstPrincipalPaymentDate: Date;
    FirstInterestPaymentDate: Date;
    InterestAccrualDayCountConvention: string;
    InitialPeriodInterestAccrualDayCountConvention: string;
    InitialPeriodInterestAccuralEndDate: Date;
    PrincipalAvailableFundsRetrieverDescription: string;
    InterestAvailableFundsRetrieverDescription: string;
    PrincipalAdvanceRate: number;
    PrincipalPaymentFrequencyInMonths: number;
    InterestPaymentFrequencyeInMonths: number;
    PaysOutAtRedemption: boolean;
    IsShortfallRecoverable: boolean;
    IsShortfallPaidFromReserves: boolean;
    AssociatedReserveAccountNames: string[];
    PricingMethodologyDescription: string;
    PricingDayCountConvention: string;
    PricingCompoundingConvention: string;
    PricingValue: number;
}