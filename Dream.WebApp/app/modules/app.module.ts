import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpModule } from '@angular/http';
import { MaterialModule } from '@angular/material';

import { LoadingModule } from 'ngx-loading';
import { ANIMATION_TYPES } from 'ngx-loading';
import { TooltipModule } from 'ngx-bootstrap';

import { RoutingModule } from '../modules/routing.module';

import { AppComponent } from '../components/app.component';
import { SecuritizationInventoryComponent } from '../components/securitization-inventory.component';
import { SecuritizationModelComponent } from '../components/securitization-model.component';
import { SecuritizationSummaryComponent } from '../components/securitization-summary.component';

import { SecuritizationInventoryDataService } from '../services/securitization-inventory-data.service';
import { SecuritizationModelDataService } from '../services/securitization-model-data.service';

@NgModule({
    imports: [
        BrowserModule,
        HttpModule,
        RoutingModule,
        MaterialModule,
        BrowserAnimationsModule,
        TooltipModule.forRoot(),
        LoadingModule.forRoot({
            fullScreenBackdrop: false,
            animationType: ANIMATION_TYPES.threeBounce,
            primaryColour: 'blue',
            secondaryColour: 'blue',
            tertiaryColour: 'blue'
        }),
    ],

    declarations: [
        AppComponent,
        SecuritizationInventoryComponent,
        SecuritizationModelComponent,
        SecuritizationSummaryComponent,
    ],

    providers: [
        SecuritizationInventoryDataService,
        SecuritizationModelDataService,
    ],

    bootstrap: [
        AppComponent,
    ]
})
export class AppModule { }
