export interface CafeOrderModel {
  orderId: string;
  cafeId: string;
  customerName: string;
  items: OrderItem[];
  totalAmount: number;
  orderStates: OrderStates;
}

export interface OrderItem {
  id: string;
  name: string;
  quantity: number;
  price: number;
}

export interface OrderStates {
  orderReceived: OrderState;
  orderPrepared: OrderState;
  orderServed: OrderState;
}

export interface OrderState {
  startTimestamp: Date;
  endTimestamp: Date;
}

export interface OrderSyncRequest {
  orders: CafeOrderModel[];
}
