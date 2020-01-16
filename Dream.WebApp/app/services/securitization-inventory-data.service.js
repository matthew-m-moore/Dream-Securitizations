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
var http_1 = require("@angular/http");
var http_2 = require("@angular/http");
require("rxjs/add/operator/toPromise");
var SecuritizationInventoryDataService = (function () {
    function SecuritizationInventoryDataService(http) {
        this.http = http;
        this.webApiUrl = 'api/inventory';
        this.headers = new http_2.Headers({ 'Content-Type': 'application/json' });
    }
    SecuritizationInventoryDataService.prototype.getSecuritizationInventory = function () {
        return this.http.get(this.webApiUrl)
            .toPromise()
            .then(function (response) { return response.json(); })
            .catch(this.reportError);
    };
    SecuritizationInventoryDataService.prototype.reportError = function (error) {
        console.error('Securitization Inventory data-service error: ', error);
        return Promise.reject(error.message || error);
    };
    return SecuritizationInventoryDataService;
}());
SecuritizationInventoryDataService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [http_1.Http])
], SecuritizationInventoryDataService);
exports.SecuritizationInventoryDataService = SecuritizationInventoryDataService;
//# sourceMappingURL=securitization-inventory-data.service.js.map