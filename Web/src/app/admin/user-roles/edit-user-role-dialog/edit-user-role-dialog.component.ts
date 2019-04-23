import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserRolesService } from '../user-roles.service';
import { Role } from '../../role';
import { EditUserRoleDialogData } from './edit-user-role-dialog-data';
import { User } from '../models/user';

@Component({
  selector: 'app-edit-user-role-dialog',
  templateUrl: './edit-user-role-dialog.component.html',
  styleUrls: ['./edit-user-role-dialog.component.css']
})
export class EditUserRoleDialogComponent {

  public user: User;
  public roles: ReadonlyArray<Role>;

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: EditUserRoleDialogData,
    private svc: UserRolesService,
    public dialogRef: MatDialogRef<EditUserRoleDialogComponent>) {
    this.user = data.user;
    this.roles = data.allRoles;
  }

  public onSave(): void {

    this.svc.update(this.user.id, { roles: this.user.roles }).subscribe(_ => {
      this.dialogRef.close(true);
    });
  }

  public onCancel(): void {
    this.dialogRef.close(false);
  }
}
