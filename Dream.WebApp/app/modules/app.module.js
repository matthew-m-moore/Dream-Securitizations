"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var platform_browser_1 = require("@angular/platform-browser");
var animations_1 = require("@angular/platform-browser/animations");
var http_1 = require("@angular/http");
var material_1 = require("@angular/material");
var ngx_loading_1 = require("ngx-loading");
var ngx_loading_2 = require("ngx-loading");
var ngx_bootstrap_1 = require("ngx-bootstrap");
var routing_module_1 = require("../modules/routing.module");
var app_component_1 = require("../components/app.component");
var securitization_inventory_component_1 = require("../components/securitization-inventory.component");
var securitization_model_component_1 = require("../components/securitization-model.component");
var securitization_summary_component_1 = require("../components/securitization-summary.component");
var securitization_inventory_data_service_1 = require("../services/securitization-inventory-data.service");
var securitization_model_data_service_1 = require("../services/securitization-model-data.service");
var AppModule = (function () {
    function AppModule() {
    }
    return AppModule;
}());
AppModule = __decorate([
    core_1.NgModule({
        imports: [
            platform_browser_1.BrowserModule,
            http_1.HttpModule,
            routing_module_1.RoutingModule,
            material_1.MaterialModule,
            animations_1.BrowserAnimationsModule,
            ngx_bootstrap_1.TooltipModule.forRoot(),
            ngx_loading_1.LoadingModule.forRoot({
                fullScreenBackdrop: false,
                animationType: ngx_loading_2.ANIMATION_TYPES.threeBounce,
                primaryColour: 'blue',
                secondaryColour: 'blue',
                tertiaryColour: 'blue'
            }),
        ],
        declarations: [
            app_component_1.AppComponent,
            securitization_inventory_component_1.SecuritizationInventoryComponent,
            securitization_model_component_1.SecuritizationModelComponent,
            securitization_summary_component_1.SecuritizationSummaryComponent,
        ],
        providers: [
            securitization_inventory_data_service_1.SecuritizationInventoryDataService,
            securitization_model_data_service_1.SecuritizationModelDataService,
        ],
        bootstrap: [
            app_component_1.AppComponent,
        ]
    })
], AppModule);
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map