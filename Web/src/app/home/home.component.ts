import { Component, AfterViewInit } from '@angular/core';
import { HomeService } from './home.service';
import { Order } from '../Order';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements AfterViewInit {

  public orders: Order[];
  public displayedColumns: string[] = ['id', 'name', 'creationDateTime', 'orderStatus'];

  constructor(private svc: HomeService) { }

  ngAfterViewInit() {
    this.svc.getOrders().subscribe(res => {
      this.orders = res;
    });
  }

}
