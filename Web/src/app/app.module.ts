import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTableModule } from '@angular/material/table';

import { HomeService } from './home/home.service';
import { AppComponent } from './app.component';

import { HomeComponent } from './home/home.component';

import { MainMenuComponent } from './main-menu/main-menu.component';

import { AuthGuard } from './auth/auth.guard';
import { OrdersGuard } from './auth/orders.guard';

import { SignInCallbackComponent } from './auth/sign-in-callback.component';
import { TokenInterceptor } from './auth/token.interceptor';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';

@NgModule({
  declarations: [
    AppComponent,
    SignInCallbackComponent,
    HomeComponent,
    MainMenuComponent,
    UnauthorizedComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    RouterModule.forRoot(
      [
        {
          path: '', canActivate: [AuthGuard], children: [
            { path: 'unauthorized', component: UnauthorizedComponent },
            { path: 'home', pathMatch: 'full', component: HomeComponent, canActivate: [OrdersGuard] },
          ]
        },
        { path: 'signin-callback', component: SignInCallbackComponent }
      ],
      { enableTracing: false }
    ),
    BrowserAnimationsModule,
    MatMenuModule,
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    MatBadgeModule,
    MatTableModule
  ],
  providers: [
    HomeService,
    AuthGuard,
    OrdersGuard,
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
