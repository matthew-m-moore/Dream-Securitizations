import { SecuritizationSummaryModel } from './securitizationSummaryModel';
import { SecuritizationNodeModel } from './securitizationNodeModel';
import { SecuritizationTrancheModel } from './securitizationTrancheModel';
import { FeeTrancheModel } from './feeTrancheModel';
import { ReserveAccountModel } from './reserveAccountModel';
import { PriorityOfPaymentsModel } from './priorityOfPaymentsModel';
import { MarketRateEnvironmentModel } from './MarketRateEnvironmentModel';
import { PerformanceAssumptionsModel } from './performanceAssumptionsModel';
import { ScenarioOptionsModel } from './scenarioOptionsModel';
import { PaceAssessmentRecordModel } from './paceAssessmentRecordModel';
import { CollateralizedTranchesModel } from './collateralizedTranchesModel';

export class SecuritizationModel {
    SecuritizationDescription: string;
    SecuritizationDataSetId: number;
    SecuritizationVersionId: number;
    SecuritizationSummaryModel: SecuritizationSummaryModel;
    SecuritizationNodeModel: SecuritizationNodeModel;
    SecuritizationTrancheModel: SecuritizationTrancheModel;
    FeeTrancheModel: FeeTrancheModel;
    ReserveAccountModel: ReserveAccountModel;
    PriorityOfPaymentsModel: PriorityOfPaymentsModel;
    MarketRateEnvironmentModel: MarketRateEnvironmentModel;
    PerformanceAssumptionsModel: PerformanceAssumptionsModel;
    ScenarioOptionsModel: ScenarioOptionsModel;
    PaceAssessmentRecordModel: PaceAssessmentRecordModel;
    CollateralizedTranchesModel: CollateralizedTranchesModel;
    CollateralCutOffDate: Date;
    CashFlowStartDate: Date;
    InterestAccrualStartDate: Date;
    SecuritizationClosingDate: Date;
    SecuritizationFirstCashFlowDate: Date;
    RedemptionLogicDescription: string;
    NominalSpreadRateIndexGroup: string;
    CurveSpreadRateIndexGroup: string;
    IsResecuritization: boolean;
    UseReplines: boolean;
    SeparatePrepaymentInterest: boolean
    PreFundingAmount: number;
    PreFundingBondCount: number;
    CleanUpCallPercentage: number;
}