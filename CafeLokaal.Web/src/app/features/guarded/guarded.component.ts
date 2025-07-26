import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { CafeOrderModel } from '../../core/models/order.models';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-guarded',
  templateUrl: './guarded.component.html',
  styleUrls: ['./guarded.component.css']
})
export class GuardedComponent implements OnInit {
  orders: CafeOrderModel[] = [];
  loading = false;
  error: string | null = null;

  constructor(private orderService: OrderService) { }

  ngOnInit(): void {
    this.fetchOrders();
  }

  fetchOrders(): void {
    this.loading = true;
    this.error = null;

    this.orderService.getOrders()
      .pipe(
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: (orders) => {
          this.orders = orders;
        },
        error: (error) => {
          console.error('Error fetching orders:', error);
          this.error = error.message || 'Failed to fetch orders. Please check your permissions and try again.';
        }
      });
  }
}
