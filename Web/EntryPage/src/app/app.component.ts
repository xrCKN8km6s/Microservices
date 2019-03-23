import { Component, OnInit } from '@angular/core';
import { AppService } from './app.service';
import { Order } from './Order';

import { AuthService, User } from './auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'EntryPage';
  orders: Order[];
  currentUser: User;

  constructor(private service: AppService, private authService: AuthService) { }


  ngOnInit(): void {

    // this.service.getOrders().subscribe((data: Order[]) => {
    //   this.orders = data;
    // });
  }


}




