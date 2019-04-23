import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RolesService } from '../roles.service';
import { CreateEditRole } from '../models/create-edit-role';
import { Observable } from 'rxjs';
import { EditRoleDialogData } from './edit-role-dialog-data';
import { DialogMode } from './dialog-mode';

@Component({
  selector: 'app-edit-role-dialog',
  templateUrl: './edit-role-dialog.component.html',
  styleUrls: ['./edit-role-dialog.component.css']
})
export class EditRoleDialogComponent implements OnInit {

  public role: CreateEditRole;
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
        this.role = { name: res.name, isGlobal: res.isGlobal, permissions: res.permissions as [] };
      });
    } else {
      this.role = { name: '', isGlobal: false, permissions: [] };
    }
  }

  public onSave(): void {

    if (this.role.isGlobal) {
      this.role.permissions = [];
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

  public onCancel(): void {
    this.dialogRef.close();
  }
}
