import { Component, OnInit } from '@angular/core';
import { RolesService } from './roles.service';
import { Permission } from './models/permission';
import { MatDialog } from '@angular/material/dialog';
import { EditRoleDialogComponent } from './edit-role-dialog/edit-role-dialog.component';
import { DialogMode } from './edit-role-dialog/dialog-mode';
import { EditRoleDialogData } from './edit-role-dialog/edit-role-dialog-data';
import { ConfirmDeleteDialogComponent } from './confirm-delete-dialog/confirm-delete-dialog.component';
import { UserProfileService } from 'src/app/auth/user-profile.service';
import { Role } from '../role';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit {

  public displayedColumns: string[] = ['name', 'isGlobal', 'actions'];
  public roles: ReadonlyArray<Role>;
  private allPermissions: ReadonlyArray<Permission>;
  public canEdit: boolean;
  public canDelete: boolean;

  constructor(private svc: RolesService, private userProfileService: UserProfileService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.canEdit = this.userProfileService.hasPermission('AdminRolesEdit');
    this.canDelete = this.userProfileService.hasPermission('AdminRolesDelete');

    this.loadRoles();
  }

  public onEdit(role: Role): void {
    this.openDialog(DialogMode.Edit, this.allPermissions, role.id);
  }

  public onCreate(): void {
    this.openDialog(DialogMode.Create, this.allPermissions);
  }

  public onDelete(role: Role): void {
    const dialogRef = this.dialog.open(ConfirmDeleteDialogComponent, {
      data: `Are you sure you want to delete role ${role.name}?`,
      autoFocus: false
    });

    dialogRef.afterClosed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.svc.delete(role.id).subscribe(_ => this.loadRoles());
      }
    });
  }

  private openDialog(mode: DialogMode, allPermissions: ReadonlyArray<Permission>, roleId: number = null): void {

    const dialogData: EditRoleDialogData = {
      mode, allPermissions, roleId
    };

    const dialogRef = this.dialog.open(EditRoleDialogComponent, {
      autoFocus: false,
      width: '400px',
      height: '600px',
      data: dialogData
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
