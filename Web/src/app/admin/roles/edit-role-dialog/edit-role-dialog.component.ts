import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RolesService, PermissionDto, CreateEditRoleDto } from '../roles.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-edit-role-dialog',
  templateUrl: './edit-role-dialog.component.html',
  styleUrls: ['./edit-role-dialog.component.css']
})
export class EditRoleDialogComponent implements OnInit {

  public role: CreateEditRoleDto;
  public title: string;

  constructor(
    private svc: RolesService,
    public dialogRef: MatDialogRef<EditRoleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: EditRoleDialogData) {
    this.title = DialogMode[data.mode];
  }

  public ngOnInit() {
    if (this.data.mode === DialogMode.Edit) {
      this.svc.getRole(this.data.roleId).subscribe(res => {
        this.role = { name: res.name, isGlobal: res.isGlobal, permissions: res.permissions };
      });
    } else {
      this.role = new CreateEditRoleDto();
    }
  }

  public onSaveClick(): void {

    if (this.role.isGlobal) {
      this.role.permissions.length = 0;
    }

    let action: Observable<void>;

    if (this.data.mode === DialogMode.Edit) {
      action = this.svc.update(this.data.roleId, this.role);
    } else {
      action = this.svc.create(this.role);
    }

    action.subscribe(_ => {
      this.dialogRef.close();
    });
  }

  public onCancelClick(): void {
    this.dialogRef.close();
  }
}

export class EditRoleDialogData {
  constructor(
    public mode: DialogMode,
    public allPermissions: PermissionDto[],
    public roleId: number
  ) { }
}

export enum DialogMode {
  Create,
  Edit
}
