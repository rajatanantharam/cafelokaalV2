import { Component, Inject, OnInit, ViewChild, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { filter } from 'rxjs/operators';

import { MsalBroadcastService, MsalGuardConfiguration, MsalService, MSAL_GUARD_CONFIG } from '@azure/msal-angular';
import { AuthenticationResult, InteractionStatus, InteractionType } from '@azure/msal-browser';
import { Chart } from 'chart.js';
import { CafeOrderModel, OrderItem, OrderState } from '../../core/models/order.models';
import { OrderService } from '../../core/services/order.service';
import { ChartService } from '../../core/services/chart.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('orderChart', { static: false }) orderChart!: ElementRef<HTMLCanvasElement>;
  
  loginDisplay = false;
  private readonly _destroying$ = new Subject<void>();
  private chart?: Chart;
  private cafeOrderModel: CafeOrderModel | null = null;

  constructor(
    @Inject(MSAL_GUARD_CONFIG)
    private msalGuardConfig: MsalGuardConfiguration,
    private authService: MsalService,
    private msalBroadcastService: MsalBroadcastService,
    private orderService: OrderService,
    private chartService: ChartService
  ) { }

  ngOnInit(): void {
    this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None)
      )
      .subscribe(() => {
        this.setLoginDisplay();
      });
  }

  ngAfterViewInit(): void {
   if (this.loginDisplay && this.orderChart) {
      this.orderService.getOrders().subscribe(x => {
          this.cafeOrderModel = x;
          setTimeout(() => this.createChart(), 100);
       });
    }
  }

  setLoginDisplay() {
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
    if (this.loginDisplay && this.orderChart) {
      this.orderService.getOrders().subscribe(x => {
          this.cafeOrderModel = x;
          setTimeout(() => this.createChart(), 100);
       });
    }
  }

  signUp() {
    if (this.msalGuardConfig.interactionType === InteractionType.Popup) {
      this.authService.loginPopup({
        scopes: [],
        prompt: 'create',
      })
        .subscribe((response: AuthenticationResult) => {
          this.authService.instance.setActiveAccount(response.account);
        });
    } else {
      this.authService.loginRedirect({
        scopes: [],
        prompt: 'create',
      });
    }

  }

  login() {
    if (this.msalGuardConfig?.interactionType === InteractionType.Popup) {
      this.authService?.loginPopup({ scopes: [] })
        .subscribe((response: AuthenticationResult) => {
          this.authService?.instance.setActiveAccount(response.account);
        });
    } else {
      this.authService?.loginRedirect({ scopes: [] });
    }
  }

  createChart(): void {
    if (!this.orderChart || !this.cafeOrderModel || !this.cafeOrderModel.orders) return;
    this.chart = this.chartService.createChart(this.orderChart, this.cafeOrderModel.orders, this.chart);
  }

  // unsubscribe to events when component is destroyed
  ngOnDestroy(): void {
    this._destroying$.next(undefined);
    this._destroying$.complete();
    
    if (this.chart) {
      this.chart.destroy();
    }
  }
}
