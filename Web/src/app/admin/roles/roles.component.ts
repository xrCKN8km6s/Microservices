import { Component, OnInit, OnDestroy } from '@angular/core';
import { RolesService, RoleDto, PermissionDto } from './roles.service';
import { MatDialog } from '@angular/material/dialog';
import { EditRoleDialogComponent, EditRoleDialogData, DialogMode } from './edit-role-dialog/edit-role-dialog.component';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit {

  public displayedColumns: string[] = ['name', 'isGlobal', 'actions'];
  public roles: RoleDto[];
  private allPermissions: PermissionDto[];

  constructor(private svc: RolesService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.loadRoles();
  }

  public onEdit(role: RoleDto): void {
    this.openDialog(DialogMode.Edit, this.allPermissions, role.id);
  }

  public onCreate(): void {
    this.openDialog(DialogMode.Create, this.allPermissions);
  }

  public onDelete(role: RoleDto): void {

  }

  private openDialog(mode: DialogMode, allPermissions: PermissionDto[], roleId: number = null): void {
    const dialogRef = this.dialog.open(EditRoleDialogComponent, {
      autoFocus: false,
      width: '400px',
      height: '600px',
      data: new EditRoleDialogData(mode, allPermissions, roleId)
    });

    dialogRef.afterClosed().subscribe(_ => this.loadRoles());
  }

  private loadRoles(): void {
    this.svc.getRolesViewModel().subscribe(vm => {
      this.roles = vm.roles;
      this.allPermissions = vm.allPermissions;
    });
  }
}
