import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Order } from './Order';

@Injectable()
export class AppService {
  constructor(private http: HttpClient) { }

  getOrders() {
    return this.http.get('https://localhost:5001/api/orders');
  }
}
