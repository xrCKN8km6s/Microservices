import { Component, OnInit, OnDestroy } from '@angular/core';

import { AuthService } from './auth/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {

  private sub: Subscription;
  public isLoggedIn: boolean;
  public userName: string;

  constructor(private auth: AuthService) { }

  ngOnInit(): void {
    this.sub = this.auth.userSignedIn.subscribe(user => {

      if (!user) {
        return;
      }

      this.isLoggedIn = true;
      this.userName = user.profile.name;
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  onLogOut(): void {
    this.auth.signOut();
  }
}
