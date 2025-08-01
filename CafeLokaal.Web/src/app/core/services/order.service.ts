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

  getOrders(): Observable<CafeOrderModel[]> {
    return this.httpClient.get<CafeOrderModel[]>(`${this.apiUrl}/api/orders`);
  }
}