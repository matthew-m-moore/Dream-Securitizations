"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var router_1 = require("@angular/router");
var securitization_inventory_component_1 = require("../components/securitization-inventory.component");
var securitization_model_component_1 = require("../components/securitization-model.component");
var securitization_summary_component_1 = require("../components/securitization-summary.component");
var appRoutes = [
    {
        path: 'securitization-inventory',
        component: securitization_inventory_component_1.SecuritizationInventoryComponent
    },
    {
        path: 'securitization-model/:dataSetId/:versionId/:isResecuritization',
        component: securitization_model_component_1.SecuritizationModelComponent,
        children: [
            {
                path: 'securitization-summary',
                component: securitization_summary_component_1.SecuritizationSummaryComponent,
                outlet: 'securitization-detail'
            },
        ]
    },
];
exports.RoutingModule = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=routing.module.js.map