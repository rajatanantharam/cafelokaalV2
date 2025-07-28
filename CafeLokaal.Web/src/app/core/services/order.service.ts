import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CafeOrderModel, OrderState } from '../models/order.models';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = environment.apiUrl;
  private httpClient: HttpClient;

  constructor(http: HttpClient) {
    this.httpClient = http;
  }

  getOrders(): Observable<CafeOrderModel> {
    // return this.httpClient.get<CafeOrderModel[]>(`${this.apiUrl}/api/orders`);
    return of(this.getMockOrders());
  }

  // Mock function to simulate fetching orders
  getMockOrders(): CafeOrderModel {
    return  { 
        organizationId: 'org1',
        organizationName: 'Cafe Willem',
        orders: [
          { orderId: '001', orderState: OrderState.OrderReceived, processTime: 5, processDate: new Date() },
          { orderId: '002', orderState: OrderState.OrderPrepared, processTime: 12, processDate: new Date() },
          { orderId: '003', orderState: OrderState.OrderServed, processTime: 8, processDate: new Date() },
          { orderId: '004', orderState: OrderState.OrderReceived, processTime: 15, processDate: new Date() },
          { orderId: '005', orderState: OrderState.OrderPrepared, processTime: 7, processDate: new Date() },
          { orderId: '006', orderState: OrderState.OrderServed, processTime: 10, processDate: new Date() },
          { orderId: '007', orderState: OrderState.OrderReceived, processTime: 6, processDate: new Date() },
          { orderId: '008', orderState: OrderState.OrderPrepared, processTime: 14, processDate: new Date() },
        ]
      }
  } 
}