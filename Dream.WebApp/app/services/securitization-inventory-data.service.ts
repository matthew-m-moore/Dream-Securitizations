import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Headers } from '@angular/http';

import { SecuritizationDataSet } from '../classes/securitizationDataSet';

import 'rxjs/add/operator/toPromise';

@Injectable()

export class SecuritizationInventoryDataService {
    private webApiUrl = 'api/inventory';
    private headers = new Headers({ 'Content-Type': 'application/json' });

    constructor(private http: Http) { }

    getSecuritizationInventory(): Promise<SecuritizationDataSet[]> {
        return this.http.get(this.webApiUrl)
            .toPromise()
            .then(response => response.json() as SecuritizationDataSet[])
            .catch(this.reportError);
    }

    private reportError(error: any): Promise<any> {
        console.error('Securitization Inventory data-service error: ', error);
        return Promise.reject(error.message || error);
    }
}