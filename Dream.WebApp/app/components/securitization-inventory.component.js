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
var securitization_inventory_data_service_1 = require("../services/securitization-inventory-data.service");
var securitization_model_data_service_1 = require("../services/securitization-model-data.service");
var SecuritizationInventoryComponent = (function () {
    function SecuritizationInventoryComponent(securitizationInventoryDataService, securitizationModelDataService, router) {
        this.securitizationInventoryDataService = securitizationInventoryDataService;
        this.securitizationModelDataService = securitizationModelDataService;
        this.router = router;
        this.loadingFlag = false;
    }
    SecuritizationInventoryComponent.prototype.onSelect = function (securitizationDataSet) {
        this.selectedSecuritizationDataSet = securitizationDataSet;
    };
    SecuritizationInventoryComponent.prototype.loadSecuritization = function (securitizationVersion) {
        var _this = this;
        this.loadingFlag = true;
        var dataSetId = securitizationVersion.SecuritizationDataSetId;
        var versionId = securitizationVersion.SecuritizationVersionId;
        var isResecuritization = this.selectedSecuritizationDataSet.IsRescuritization;
        this.securitizationDataSets = null;
        this.selectedSecuritizationDataSet = null;
        var routingUrl = ['/securitization-model', dataSetId, versionId, isResecuritization];
        this.router.navigate(routingUrl)
            .then(function () { _this.loadingFlag = false; });
    };
    SecuritizationInventoryComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.loadingFlag = true;
        this.securitizationInventoryDataService.getSecuritizationInventory()
            .then(function (securitizationInventory) {
            _this.securitizationDataSets = securitizationInventory;
            _this.loadingFlag = false;
        });
    };
    return SecuritizationInventoryComponent;
}());
SecuritizationInventoryComponent = __decorate([
    core_1.Component({
        selector: 'securitization-inventory',
        templateUrl: '../views/securitization-inventory.component.html',
        styleUrls: ['../styles/securitization-inventory.component.css'],
        moduleId: module.id
    }),
    __metadata("design:paramtypes", [securitization_inventory_data_service_1.SecuritizationInventoryDataService,
        securitization_model_data_service_1.SecuritizationModelDataService,
        router_1.Router])
], SecuritizationInventoryComponent);
exports.SecuritizationInventoryComponent = SecuritizationInventoryComponent;
//# sourceMappingURL=securitization-inventory.component.js.map