import { PerformanceAssumptionEntry } from './performanceAssumptionEntry';

export class PerformanceAssumptionsModelEntry {
    PerformanceAssumptionDescription: string;
    PrepaymentAssumptionEntry: PerformanceAssumptionEntry;
    DefaultAssumptionEntry: PerformanceAssumptionEntry;
    DelinquencyAssumptionEntry: PerformanceAssumptionEntry;
    SeverityAssumptionEntry: PerformanceAssumptionEntry;
    UseDbrsModel: boolean;
    DbrsModelDefaultRate: number;
    DbrsModelLossGivenDefault: number;
    DbrsModelForeclosureTermInMonths: number;
    DbrsModelReperformanceTermInMonths: number;
    DbrsModelNumberOfDefaultSequences: number;
}