import { PrepaymentPenaltyPlanEntry } from './prepaymentPenaltyPlanEntry';
import { PaceAssessmentRatePlanTermSetEntry } from './paceAssessmentRatePlanTermSetEntry';

export class PaceAssessmentRecordModelEntry {
    LoanId: number;
    BondId: string;
    ReplineId: string;
    PropertyState: string;
    Balance: number;
    ProjectCost: number;
    CouponRate: number;
    BuyDownRate: number;
    TermInYears: number;
    FundingDate: Date;
    BondFirstPaymentDate: Date;
    BondFirstPrincipalPaymentDate: Date;
    BondMaturityDate: Date;
    CashFlowStartDate: Date;
    InterestAccrualStartDate: Date;
    InterestAccrualEndMonth: number;
    InterestPaymentFrequencyInMonths: number;
    PrincipalPaymentFrequencyInMonths: number;
    NumberOfUnderlyingBonds: number;
    AccruedInterest: number;
    ActualPrepaymentsReceived: number;
    PrepaymentPenaltyPlanEntry: PrepaymentPenaltyPlanEntry;
    PaceAssessmentRatePlanTermSetEntry: PaceAssessmentRatePlanTermSetEntry;
}