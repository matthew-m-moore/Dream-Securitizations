import { ModuleWithProviders } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Routes } from '@angular/router';

import { SecuritizationInventoryComponent } from '../components/securitization-inventory.component';
import { SecuritizationModelComponent } from '../components/securitization-model.component';
import { SecuritizationSummaryComponent } from '../components/securitization-summary.component';

const appRoutes: Routes = [
    {
        path: 'securitization-inventory',
        component: SecuritizationInventoryComponent
    },
    {
        path: 'securitization-model/:dataSetId/:versionId/:isResecuritization',
        component: SecuritizationModelComponent,
        children: [
            {
                path: 'securitization-summary',
                component: SecuritizationSummaryComponent,
                outlet: 'securitization-detail'
            },
        ]
    },

];

export const RoutingModule: ModuleWithProviders = RouterModule.forRoot(appRoutes);