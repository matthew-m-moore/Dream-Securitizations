import { BalanceCapOrFloorEntry } from './balanceCapOrFloorEntry';

export class ReserveAccountModelEntry {
    ReserveAccountId: number;
    ReserveAccountName: string;
    SecuritizationNodeId: number;
    SecuritizationNodeName: string;
    InitialAccountBalanceInDollars: number;
    InitialAccountBalanceAsPercentage: number;
    FirstReserveAccountDrawOrDepositDate: Date;
    ReserveAccountDrawOrDepositFrequencyInMonths: number;
    AvailableFundsRetrieverDescription: string;
    BalanceCapOrFloorEntries: BalanceCapOrFloorEntry[];
}