import { Component, OnInit, OnDestroy } from '@angular/core';

import { AuthService } from './auth/auth.service';
import { Subscription } from 'rxjs';
import { MainMenuItem } from './main-menu-item';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {

  private sub: Subscription;
  public isLoggedIn: boolean;
  public userName: string;
  public mainMenuItems: MainMenuItem[];

  constructor(private auth: AuthService) { }

  ngOnInit(): void {
    this.sub = this.auth.userSignedIn.subscribe(user => {

      this.isLoggedIn = true;
      this.userName = user.profile.name;

      this.prepareMainMenu();
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  onLogOut(): void {
    this.auth.signOut();
  }

  private prepareMainMenu(): void {
    this.mainMenuItems = [];

    if (this.auth.hasPermission('OrdersView')) {
      this.mainMenuItems.push({ title: 'Orders', routerLink: '/orders' });
    }

    if (this.auth.hasPermission('AdminView')) {
      this.mainMenuItems.push({ title: 'Admin', routerLink: '/admin' });
    }
  }
}
