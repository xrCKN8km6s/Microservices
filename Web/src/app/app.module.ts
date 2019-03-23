import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatBadgeModule } from '@angular/material/badge';

import { AppService } from './app.service';
import { AppComponent } from './app.component';

import { HomeComponent } from './home/home.component';

import { MainMenuComponent } from './main-menu/main-menu.component';

import { AuthGuard } from './auth/auth.guard';

import { SignInCallbackComponent } from './sign-in-callback.component';
import { SilentCallbackComponent } from './silent-callback.component';

@NgModule({
  declarations: [
    AppComponent,
    SignInCallbackComponent,
    SilentCallbackComponent,
    HomeComponent,
    MainMenuComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    RouterModule.forRoot(
      [
        { path: 'signin-callback', component: SignInCallbackComponent },
        { path: 'silent-callback', component: SilentCallbackComponent },
        { path: 'home', pathMatch: 'full', component: HomeComponent, canActivate: [AuthGuard] },
        { path: '', pathMatch: 'full', redirectTo: '/home' }
      ],
      { enableTracing: false }
    ),
    BrowserAnimationsModule,
    MatMenuModule,
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    MatBadgeModule
  ],
  providers: [
    AppService,
    AuthGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
