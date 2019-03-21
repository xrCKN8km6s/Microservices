import { Component, OnInit } from '@angular/core';
import { AppService } from './app.service';
import { Order } from './Order';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'EntryPage';
  orders: Order[];

  constructor(private service: AppService) { }


  ngOnInit(): void {
    this.service.getOrders().subscribe((data: Order[]) => {
      this.orders = data;
    });
  }


}




