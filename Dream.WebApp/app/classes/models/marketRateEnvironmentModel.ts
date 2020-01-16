import { MarketRateDataModel } from './marketRateDataModel';
import { RateCurveDataModel } from './rateCurveDataModel';

export class MarketRateEnvironmentModel {
    IsModified: boolean;
    MarketRateDataModel: MarketRateDataModel;
    RateCurveDataModel: RateCurveDataModel;
}