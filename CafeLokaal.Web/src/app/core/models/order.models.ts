export interface CafeOrderModel {
  organizationName: string;
  orderStep: OrderState;
  processTime: number;
  processDate: Date;
}

export enum OrderState {
  OrderReceived = 'OrderReceived',
  OrderPrepared = 'OrderPrepared',
  OrderServed = 'OrderServed',
}