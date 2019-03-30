import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Order } from './order.model';

@Injectable()
export class OrdersService {
  constructor(private http: HttpClient) { }

  getOrders() {
    return this.http.get<Order[]>('http://localhost:5000/api/orders');
  }
}
