import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RolesService, PermissionDto, CreateEditRoleDto } from '../roles.service';

@Component({
  selector: 'app-edit-role-dialog',
  templateUrl: './edit-role-dialog.component.html',
  styleUrls: ['./edit-role-dialog.component.css']
})
export class EditRoleDialogComponent implements OnInit {

  public role: CreateEditRoleDto;

  constructor(
    private svc: RolesService,
    public dialogRef: MatDialogRef<EditRoleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: EditRoleDialogData) { }

  public ngOnInit() {
    this.svc.getRole(this.data.roleId).subscribe(res => {
      this.role = new CreateEditRoleDto(res.name, res.isGlobal, res.permissions);
    });
  }

  public onSaveClick(): void {

    if (this.role.isGlobal) {
      this.role.permissions.length = 0;
    }

    this.svc.update(this.data.roleId, this.role).subscribe(_ => {
      this.dialogRef.close();
    });
  }

  public onCancelClick(): void {
    this.dialogRef.close();
  }
}

export class EditRoleDialogData {
  constructor(
    public roleId: number,
    public allPermissions: PermissionDto[]) { }
}
