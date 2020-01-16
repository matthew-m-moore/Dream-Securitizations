import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Headers } from '@angular/http';

import { SecuritizationVersion } from '../classes/securitizationVersion';
import { SecuritizationModel } from '../classes/models/securitizationModel';

import 'rxjs/add/operator/toPromise';

@Injectable()

export class SecuritizationModelDataService {
    private webApiUrl = 'api/securitization';
    private headers = new Headers({ 'Content-Type': 'application/json' });

    public securitizationModel: SecuritizationModel;

    constructor(private http: Http) { }

    loadSecuritizationModel(dataSetId: number, versionId: number, isResecuritization: string)
        : Promise<SecuritizationModel> {
        return this.http.get(`${this.webApiUrl}/load/${dataSetId}/${versionId}/${isResecuritization}`)
            .toPromise()
            .then(response => response.json() as SecuritizationModel)
            .catch(this.reportError);
    }

    private reportError(error: any): Promise<any> {
        console.error('Securitization Inventory data-service error: ', error);
        return Promise.reject(error.message || error);
    }
}