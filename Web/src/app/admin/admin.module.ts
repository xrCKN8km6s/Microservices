import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { EditRoleDialogComponent } from './roles/edit-role-dialog/edit-role-dialog.component';
import { EditUserRoleDialogComponent } from './user-roles/edit-user-role-dialog/edit-user-role-dialog.component';
import { ConfirmDeleteDialogComponent } from './roles/confirm-delete-dialog/confirm-delete-dialog.component';
import { AdminComponent } from './admin.component';
import { RolesComponent } from './roles/roles.component';
import { UserRolesComponent } from './user-roles/user-roles.component';
import { MaterialModule } from '../material.module';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AdminComponent,
    RolesComponent,
    UserRolesComponent,
    EditRoleDialogComponent,
    EditUserRoleDialogComponent,
    ConfirmDeleteDialogComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    FormsModule,
    MaterialModule
  ],
  entryComponents: [
    EditRoleDialogComponent,
    EditUserRoleDialogComponent,
    ConfirmDeleteDialogComponent
  ]
})
export class AdminModule { }
