import { Component } from '@angular/core';
import { OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { SecuritizationDataSet } from '../classes/securitizationDataSet';
import { SecuritizationVersion } from '../classes/securitizationVersion';

import { SecuritizationInventoryDataService } from '../services/securitization-inventory-data.service';
import { SecuritizationModelDataService } from '../services/securitization-model-data.service';

@Component({
    selector: 'securitization-inventory',
    templateUrl: '../views/securitization-inventory.component.html',
    styleUrls: ['../styles/securitization-inventory.component.css'],
    moduleId: module.id
})

export class SecuritizationInventoryComponent implements OnInit {
    securitizationDataSets: SecuritizationDataSet[];
    selectedSecuritizationDataSet: SecuritizationDataSet;

    public loadingFlag = false;


    constructor(
        private securitizationInventoryDataService: SecuritizationInventoryDataService,
        private securitizationModelDataService: SecuritizationModelDataService,
        private router: Router,
    ) { }

    onSelect(securitizationDataSet: SecuritizationDataSet): void {
        this.selectedSecuritizationDataSet = securitizationDataSet;
    }

    loadSecuritization(securitizationVersion: SecuritizationVersion): void {
        this.loadingFlag = true;

        var dataSetId = securitizationVersion.SecuritizationDataSetId;
        var versionId = securitizationVersion.SecuritizationVersionId;
        var isResecuritization = this.selectedSecuritizationDataSet.IsRescuritization;

        this.securitizationDataSets = null;
        this.selectedSecuritizationDataSet = null;

        let routingUrl = ['/securitization-model', dataSetId, versionId, isResecuritization];
        this.router.navigate(routingUrl)
            .then(() => { this.loadingFlag = false; });
    }

    ngOnInit(): void {
        this.loadingFlag = true;
        this.securitizationInventoryDataService.getSecuritizationInventory()
            .then(securitizationInventory => {
                this.securitizationDataSets = securitizationInventory;
                this.loadingFlag = false;
            });
    }
}