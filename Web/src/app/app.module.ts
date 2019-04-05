import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatListModule } from '@angular/material/list';
import { MatCheckboxModule } from '@angular/material/checkbox';

import { AppComponent } from './app.component';

import { OrdersComponent } from './orders/orders.component';
import { OrdersService } from './orders/orders.service';
import { OrdersGuard } from './auth/orders.guard';

import { MainMenuComponent } from './main-menu/main-menu.component';

import { AuthGuard } from './auth/auth.guard';

import { SignInCallbackComponent } from './auth/sign-in-callback.component';
import { TokenInterceptor } from './auth/token.interceptor';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { LandingComponent } from './landing/landing.component';
import { ContentTypeInterceptor } from './auth/content-type.interceptor';
import { AdminComponent } from './admin/admin.component';
import { RolesComponent } from './admin/roles/roles.component';
import { RolesService } from './admin/roles/roles.service';

@NgModule({
  declarations: [
    AppComponent,
    SignInCallbackComponent,
    OrdersComponent,
    MainMenuComponent,
    UnauthorizedComponent,
    LandingComponent,
    AdminComponent,
    RolesComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot(
      [
        {
          path: '', canActivate: [AuthGuard], children: [
            { path: 'landing', component: LandingComponent },
            { path: 'orders', component: OrdersComponent, canActivate: [OrdersGuard] },
            { path: 'admin', component: AdminComponent },
            { path: '', pathMatch: 'full', redirectTo: '/landing' },
          ]
        },
        { path: 'unauthorized', component: UnauthorizedComponent },
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
    MatTableModule,
    MatSnackBarModule,
    MatTabsModule,
    MatListModule,
    MatCheckboxModule
  ],
  providers: [
    OrdersService,
    RolesService,
    AuthGuard,
    OrdersGuard,
    { provide: HTTP_INTERCEPTORS, useClass: ContentTypeInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
