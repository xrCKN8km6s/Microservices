import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';
import { LandingComponent } from './landing/landing.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { SignInCallbackComponent } from './auth/sign-in-callback.component';

const routes: Routes = [{
  path: '', canActivate: [AuthGuard], children: [
    { path: 'landing', component: LandingComponent },
    { path: 'orders', loadChildren: () => import('./orders/orders.module').then(m => m.OrdersModule) },
    { path: 'admin', loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule) },
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
