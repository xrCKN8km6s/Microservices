import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';

import { LandingComponent } from './landing/landing.component';
import { MainMenuComponent } from './main-menu/main-menu.component';
import { SignInCallbackComponent } from './auth/sign-in-callback.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';

import { TokenInterceptor } from './auth/token.interceptor';
import { ContentTypeInterceptor } from './content-type.interceptor';

import { AuthGuard } from './auth/auth.guard';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';

@NgModule({
  declarations: [
    AppComponent,
    SignInCallbackComponent,
    MainMenuComponent,
    UnauthorizedComponent,
    LandingComponent
  ],
  imports: [
    BrowserModule,
    MaterialModule,
    BrowserAnimationsModule,
    HttpClientModule,
    RouterModule.forRoot(
      [
        {
          path: '', canActivate: [AuthGuard], children: [
            { path: 'landing', component: LandingComponent },
            { path: 'orders', loadChildren: './orders/orders.module#OrdersModule' },
            { path: 'admin', loadChildren: './admin/admin.module#AdminModule' },
            { path: '', pathMatch: 'full', redirectTo: '/landing' },
          ]
        },
        { path: 'unauthorized', component: UnauthorizedComponent },
        { path: 'signin-callback', component: SignInCallbackComponent }
      ]
    )
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ContentTypeInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
