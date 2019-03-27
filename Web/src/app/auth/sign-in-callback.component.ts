import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

@Component({
  template: ''
})
export class SignInCallbackComponent implements OnInit {

  constructor(private auth: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.auth.competeSignIn().then(state => {
      this.router.navigateByUrl(state);
    });
  }
}
