import { FeeDetailEntry } from './feeDetailEntry';

export class FeeTrancheModelEntry {
    FeeTrancheId: number;
    FeeTrancheName: string;
    FeeTrancheTypeDescription: string;
    AccruedFeePayment: number;
    FeePaymentConvention: string;
    FeeAccrualDayCountConvention: string;
    InitialPeriodFeeAccrualDayCountConvention: string;
    FirstFeePaymentDate: Date;
    InitialPeriodFeeAccuralEndDate: Date;
    AvailableFundsRetrieverDescription: string;
    AnnualMinimumFeeAmount: number;
    AnnualMaximumFeeAmount: number;
    AnnualFeePerUnit: number;
    RollingAverageBalanceMonths: number;
    BalanceUpdateFrequenceInMonths: number;
    FeeIncreaseFrequncyInMonths: number;
    FeeIncreasePercentage: number;
    PercentOfTrancheBalance: number;
    PercentOfCollateralBalance: number;
    AssociatedTrancheNames: string[];
    AssociatedReserveAccountNames: string[];
    FeeDetailEntries: FeeDetailEntry[];
    SecuritizationNodeId: number;
    SecuritizationNodeName: string;
    UseStartingBalance: boolean;
    PaysOutAtRedemption: boolean;
    IsShortfallRecoverable: boolean;
    IsShortfallPaidFromReserves: boolean;
}