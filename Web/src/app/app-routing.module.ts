import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';
import { LandingComponent } from './landing/landing.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { SignInCallbackComponent } from './auth/sign-in-callback.component';

const routes: Routes = [{
  path: '', canActivate: [AuthGuard], children: [
    { path: 'landing', component: LandingComponent },
    { path: 'orders', loadChildren: './orders/orders.module#OrdersModule' },
    { path: 'admin', loadChildren: './admin/admin.module#AdminModule' },
    { path: '', pathMatch: 'full', redirectTo: '/landing' },
  ]
},
{ path: 'unauthorized', component: UnauthorizedComponent },
{ path: 'signin-callback', component: SignInCallbackComponent }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
