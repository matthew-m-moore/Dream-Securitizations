"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var router_2 = require("@angular/router");
var securitization_model_data_service_1 = require("../services/securitization-model-data.service");
require("rxjs/add/operator/switchMap");
var SecuritizationModelComponent = (function () {
    function SecuritizationModelComponent(securitizationModelDataService, activatedRoute, router) {
        this.securitizationModelDataService = securitizationModelDataService;
        this.activatedRoute = activatedRoute;
        this.router = router;
    }
    SecuritizationModelComponent.prototype.ngAfterViewChecked = function () {
        var _this = this;
        if ((!this.dataSetId && !this.versionId) || this.summaryLoaded)
            return;
        this.summaryLoaded = true;
        setTimeout(function () {
            var routingUrl = "securitization-model/" + _this.dataSetId + "/" + _this.versionId + "/" + _this.isResecuritization + "/(securitization-detail:securitization-summary)";
            _this.router.navigateByUrl(routingUrl);
        }, 0);
    };
    SecuritizationModelComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.securitizationDescription = 'Loading securitization...';
        this.summaryLoaded = false;
        this.activatedRoute.paramMap
            .switchMap(function (params) {
            return _this.securitizationModelDataService.loadSecuritizationModel(+params.get('dataSetId'), +params.get('versionId'), params.get('isResecuritization'));
        })
            .subscribe(function (securitizationModel) {
            _this.securitizationDescription = securitizationModel.SecuritizationDescription;
            _this.dataSetId = securitizationModel.SecuritizationDataSetId;
            _this.versionId = securitizationModel.SecuritizationVersionId;
            _this.isResecuritization = securitizationModel.IsResecuritization;
            _this.securitizationModelDataService.securitizationModel = securitizationModel;
        });
    };
    return SecuritizationModelComponent;
}());
SecuritizationModelComponent = __decorate([
    core_1.Component({
        selector: 'securitization-model',
        templateUrl: '../views/securitization-model.component.html',
        styleUrls: ['../styles/securitization-model.component.css'],
        moduleId: module.id,
        providers: [securitization_model_data_service_1.SecuritizationModelDataService]
    }),
    __metadata("design:paramtypes", [securitization_model_data_service_1.SecuritizationModelDataService,
        router_1.ActivatedRoute,
        router_2.Router])
], SecuritizationModelComponent);
exports.SecuritizationModelComponent = SecuritizationModelComponent;
//# sourceMappingURL=securitization-model.component.js.map