import { Component, OnInit } from '@angular/core';
import { AuthService, User } from '../auth/auth.service';

@Component({
  selector: 'app-main-menu',
  templateUrl: './main-menu.component.html',
  styleUrls: ['./main-menu.component.css']
})
export class MainMenuComponent implements OnInit {

  isLoggedIn: boolean;
  userName: string;

  constructor(private auth: AuthService) { }

  ngOnInit() {
    this.auth.userSignedIn.subscribe(user => {

      if (!user) {
        return;
      }

      this.isLoggedIn = true;
      this.userName = user.profile.name;
    });

    this.auth.userSignedOut.subscribe(_ => {

      this.isLoggedIn = false;
      this.userName = null;
    });
  }

  logOut(): void {
    this.auth.signOut();
  }

}
