import { RateCurveDataEntry } from './rateCurveDataEntry';

export class RateCurveDataModelEntry {
    RateIndexName: string;
    RateIndexGrouping: string;
    RateCurveTypeDescription: string;
    RateCurveDataEntries: RateCurveDataEntry[];
    DayCountConvention: string;
    MarketDate: Date;
}