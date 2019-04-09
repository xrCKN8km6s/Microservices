import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserDto, UserRolesService } from '../user-roles.service';
import { RoleDto } from '../../roles/roles.service';

@Component({
  selector: 'app-edit-user-role-dialog',
  templateUrl: './edit-user-role-dialog.component.html',
  styleUrls: ['./edit-user-role-dialog.component.css']
})
export class EditUserRoleDialogComponent {

  public user: UserDto;
  public roles: RoleDto[];

  constructor(
    private svc: UserRolesService,
    public dialogRef: MatDialogRef<EditUserRoleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: EditUserRoleDialogData) {
      this.user = data.user;
      this.roles = data.allRoles;
     }

  public onSave(): void {

    this.svc.update(this.user.id, { roles: this.user.roles }).subscribe(_ => {
      this.dialogRef.close();
    });
  }

  public onCancel(): void {
    this.dialogRef.close();
  }
}

export class EditUserRoleDialogData {
  constructor(
    public allRoles: RoleDto[],
    public user: UserDto
  ) { }
}
