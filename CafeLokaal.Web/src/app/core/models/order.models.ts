export interface CafeOrderModel {
  organizationId: string;
  organizationName: string;
  orders: OrderItem[];
}
export interface OrderItem {
  orderId: string;
  orderState: OrderState;
  processTime: number;
  processDate: Date;
}

export enum OrderState {
  OrderReceived = 'OrderReceived',
  OrderPrepared = 'OrderPrepared',
  OrderServed = 'OrderServed',
}