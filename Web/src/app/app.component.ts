import { Component, OnInit, OnDestroy } from '@angular/core';
import { AppService } from './app.service';
import { Order } from './Order';

import { AuthService } from './auth/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {

  title = 'EntryPage';
  orders: Order[];

  private sub: Subscription;
  isLoggedIn: boolean;
  userName: any;

  constructor(private service: AppService, private auth: AuthService) { }

  ngOnInit(): void {


    this.sub = this.auth.userSignedIn.subscribe(user => {

      if (!user) {
        return;
      }

      this.isLoggedIn = true;
      this.userName = user.profile.name;

      // this.service.getOrders().subscribe((data: Order[]) => {
      //   this.orders = data;
      // });
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  onLogOut(): void {
    this.auth.signOut();
  }
}
