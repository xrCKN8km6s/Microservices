import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { Subscription } from 'rxjs';
import { MainMenuItem } from './main-menu/main-menu-item';
import { UserProfileService } from './auth/user-profile.service';

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

  constructor(private auth: AuthService, private userProfileService: UserProfileService) { }

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

    if (this.userProfileService.hasPermission('OrdersView')) {
      this.mainMenuItems.push({ title: 'Orders', routerLink: '/orders' });
    }

    if (this.userProfileService.hasPermission('AdminView')) {
      this.mainMenuItems.push({ title: 'Admin', routerLink: '/admin' });
    }
  }
}
