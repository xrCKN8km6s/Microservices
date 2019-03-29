import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { first } from 'rxjs/operators';

@Component({
  template: ''
})
export class SignInCallbackComponent implements OnInit {

  constructor(private auth: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.auth.competeSignIn().pipe(first())
      .subscribe(state => {
        this.router.navigateByUrl(state);
      },
        () => this.router.navigateByUrl('/unauthorized'));
  }
}
