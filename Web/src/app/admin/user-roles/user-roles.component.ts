import { Component, OnInit } from '@angular/core';
import { UserRolesService, UserDto } from './user-roles.service';
import { MatDialog } from '@angular/material/dialog';
import { RoleDto } from '../roles/roles.service';
import { EditUserRoleDialogData, EditUserRoleDialogComponent } from './edit-user-role-dialog/edit-user-role-dialog.component';
import { UserProfileService } from 'src/app/auth/user-profile.service';

@Component({
  selector: 'app-user-roles',
  templateUrl: './user-roles.component.html',
  styleUrls: ['./user-roles.component.css']
})
export class UserRolesComponent implements OnInit {

  public displayedColumns: string[] = ['name', 'actions'];
  private allRoles: RoleDto[];
  public users: UserDto[];
  public canEdit: boolean;

  constructor(private svc: UserRolesService, private userProfileService: UserProfileService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.canEdit = this.userProfileService.hasPermission('AdminUsersEdit');
    this.loadUserRoles();
  }

  public onEdit(user: UserDto): void {
    this.openDialog(this.allRoles, user);
  }

  private openDialog(allRoles: RoleDto[], user: UserDto): void {
    const dialogRef = this.dialog.open(EditUserRoleDialogComponent, {
      autoFocus: false,
      width: '400px',
      height: '600px',
      data: new EditUserRoleDialogData(allRoles, user)
    });

    dialogRef.afterClosed().subscribe(_ => this.loadUserRoles());
  }

  private loadUserRoles(): void {
    this.svc.getUserRolesViewModel().subscribe(vm => {
      this.allRoles = vm.roles;
      this.users = vm.users;
    });
  }
}
