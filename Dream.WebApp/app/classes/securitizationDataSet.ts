import { SecuritizationVersion } from './securitizationVersion';

export class SecuritizationDataSet {
    SecuritizationDataSetId: number;
    SecuritizationDataSetDescription: string;
    SecuritizationDataSetComment: string;
    SecuritizationOwner: string;
    CreatedDate: Date;
    IsRescuritization: boolean;
    IsTemplate: boolean;
    IsReadOnly: boolean;
    SecuritizationVersions: SecuritizationVersion[];
}