import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';

@Component({
  template: ''
})
export class SilentCallbackComponent implements OnInit {

  constructor(private auth: AuthService) { }

  ngOnInit(): void {
    this.auth.renewToken();
  }
}
