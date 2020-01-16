export class SecuritizationSummaryModelEntry {  
    SecuritizationNodeId: number
    SecuritizationNodeTrancheId: number
    SecuritizationNodeName: string;
    SecuritizationNodeOrTrancheName: string;   
    SecuritizationNodeOrTrancheType: string;
    SecuritizationNodeOrTrancheRating: string;
    SecuritizationNodeOrTrancheBalance: number;
    SecuritizationNodeOrTrancheSizePercentage: number;
    SecuritizationNodeOrTrancheInitialCoupon: number;
    WeightedAverageLife: number;
    ModifiedDuration: number;
    PaymentWindow: string;
    BenchmarkYield: number;
    NominalSpread: number;
    InternalRateOfReturn: number;
    PresentValue: number;
    DollarPrice: number;
    PercentageOfCollateral: number;
    IsSecuritizationNode: boolean;
}