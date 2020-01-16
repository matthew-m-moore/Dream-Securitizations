import { Component } from '@angular/core';
import { Input } from '@angular/core';
import { OnInit } from '@angular/core';
import { AfterViewChecked } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ParamMap } from '@angular/router';
import { Router } from '@angular/router';

import { SecuritizationModel } from '../classes/models/securitizationModel';

import { SecuritizationModelDataService } from '../services/securitization-model-data.service';

import 'rxjs/add/operator/switchMap';

@Component({
    selector: 'securitization-model',
    templateUrl: '../views/securitization-model.component.html',
    styleUrls: ['../styles/securitization-model.component.css'],
    moduleId: module.id,
    providers: [SecuritizationModelDataService]
})

export class SecuritizationModelComponent implements OnInit, AfterViewChecked { 
    securitizationDescription: string;
    dataSetId: number;
    versionId: number;
    isResecuritization: boolean;
    summaryLoaded: boolean;

    constructor(
        private securitizationModelDataService: SecuritizationModelDataService,
        private activatedRoute: ActivatedRoute,
        private router: Router,
    ) { }

    ngAfterViewChecked(): void {
        if ((!this.dataSetId && !this.versionId) || this.summaryLoaded) return;
        this.summaryLoaded = true;

        setTimeout(() => {
            var routingUrl = `securitization-model/${this.dataSetId}/${this.versionId}/${this.isResecuritization}/(securitization-detail:securitization-summary)`;
            this.router.navigateByUrl(routingUrl);
        }, 0);
    }

    ngOnInit(): void {
        this.securitizationDescription = 'Loading securitization...';
        this.summaryLoaded = false;

        this.activatedRoute.paramMap
            .switchMap((params: ParamMap) =>
                this.securitizationModelDataService.loadSecuritizationModel(+params.get('dataSetId'), +params.get('versionId'), params.get('isResecuritization')))
            .subscribe(securitizationModel => {
                this.securitizationDescription = securitizationModel.SecuritizationDescription;
                this.dataSetId = securitizationModel.SecuritizationDataSetId;
                this.versionId = securitizationModel.SecuritizationVersionId;
                this.isResecuritization = securitizationModel.IsResecuritization;
                this.securitizationModelDataService.securitizationModel = securitizationModel;
            });
    }
}