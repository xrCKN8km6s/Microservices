import { Component, OnInit } from '@angular/core';
import { HomeService } from './home.service';
import { Order } from '../Order';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  orders: Order[];

  constructor(private svc: HomeService) { }

  ngOnInit() {
    this.svc.getOrders().subscribe(res => {
      this.orders = res;
    });
  }

}
