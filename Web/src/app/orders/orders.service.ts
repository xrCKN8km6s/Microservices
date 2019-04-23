import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Order } from './order.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {

  private ordersPath = 'api/orders';

  constructor(private http: HttpClient) { }

  getOrders() {
    return this.http.get<Order[]>(`${environment.bffUrl}/${this.ordersPath}`);
  }
}
