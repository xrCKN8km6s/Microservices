import { Component, AfterViewInit } from '@angular/core';
import { OrdersService } from './orders.service';
import { Order } from './order.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent implements AfterViewInit {

  public orders: Observable<Order[]>;
  public displayedColumns: string[] = ['id', 'name', 'creationDateTime', 'orderStatus'];

  constructor(private svc: OrdersService) { }

  ngAfterViewInit() {
    this.orders = this.svc.getOrders();
  }
}
