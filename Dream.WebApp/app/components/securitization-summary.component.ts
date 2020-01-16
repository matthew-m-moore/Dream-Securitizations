import { Component } from '@angular/core';
import { OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { SecuritizationSummaryModel } from '../classes/models/securitizationSummaryModel';

import { SecuritizationModelDataService } from '../services/securitization-model-data.service';

@Component({
    selector: 'securitization-summary',
    templateUrl: '../views/securitization-summary.component.html',
    styleUrls: ['../styles/securitization-summary.component.css'],
    moduleId: module.id
})

export class SecuritizationSummaryComponent implements OnInit {
    securitizationSummaryModel: SecuritizationSummaryModel;

    constructor(
        private securitizationModelDataService: SecuritizationModelDataService,
        private activatedRoute: ActivatedRoute
    ) { }

    ngOnInit(): void {
        this.securitizationSummaryModel = this.securitizationModelDataService.securitizationModel.SecuritizationSummaryModel;
    }
}