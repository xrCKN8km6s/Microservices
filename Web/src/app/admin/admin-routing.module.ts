import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminComponent } from './admin.component';
import { RolesComponent } from './roles/roles.component';
import { UserRolesComponent } from './user-roles/user-roles.component';
import { AdminGuard, AdminRolesGuard, AdminUsersGuard } from './admin.guard';

const routes: Routes = [{
  path: '', component: AdminComponent, canActivate: [AdminGuard], children: [
    { path: 'roles', component: RolesComponent, canActivate: [AdminRolesGuard] },
    { path: 'users', component: UserRolesComponent, canActivate: [AdminUsersGuard] }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
