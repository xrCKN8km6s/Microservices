import { Component, OnInit, OnDestroy } from '@angular/core';
import { RolesService, RoleDto, PermissionDto } from './roles.service';
import { MatDialog } from '@angular/material/dialog';
import { EditRoleDialogComponent, EditRoleDialogData } from './edit-role-dialog/edit-role-dialog.component';

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
    const dialogRef = this.dialog.open(EditRoleDialogComponent, {
      autoFocus: false,
      width: '400px',
      height: '600px',
      data: new EditRoleDialogData(role.id, this.allPermissions)
    });

    dialogRef.afterClosed().subscribe(_ => this.loadRoles());
  }

  public onDelete(role: RoleDto): void {

  }

  private loadRoles(): void {
    this.svc.getRolesViewModel().subscribe(vm => {
      this.roles = vm.roles;
      this.allPermissions = vm.allPermissions;
    });
  }
}
