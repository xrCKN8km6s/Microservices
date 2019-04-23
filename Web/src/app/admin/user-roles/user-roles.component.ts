import { Component, OnInit } from '@angular/core';
import { UserRolesService } from './user-roles.service';
import { MatDialog } from '@angular/material/dialog';
import { EditUserRoleDialogComponent } from './edit-user-role-dialog/edit-user-role-dialog.component';
import { UserProfileService } from 'src/app/auth/user-profile.service';
import { Role } from '../role';
import { EditUserRoleDialogData } from './edit-user-role-dialog/edit-user-role-dialog-data';
import { User } from './models/user';

@Component({
  selector: 'app-user-roles',
  templateUrl: './user-roles.component.html',
  styleUrls: ['./user-roles.component.css']
})
export class UserRolesComponent implements OnInit {

  public displayedColumns = ['name', 'actions'];
  private allRoles: ReadonlyArray<Role>;
  public users: ReadonlyArray<User>;
  public canEdit: boolean;

  constructor(private svc: UserRolesService, private userProfileService: UserProfileService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.canEdit = this.userProfileService.hasPermission('AdminUsersEdit');
    this.loadUserRoles();
  }

  public onEdit(user: User): void {
    this.openDialog(this.allRoles, user);
  }

  private openDialog(allRoles: ReadonlyArray<Role>, user: User): void {

    const dialogData: EditUserRoleDialogData = { allRoles, user };

    const dialogRef = this.dialog.open(EditUserRoleDialogComponent, {
      autoFocus: false,
      width: '400px',
      height: '600px',
      data: dialogData
    });

    dialogRef.afterClosed().subscribe((changed) => {
      if (changed) {
        this.loadUserRoles();
      }
    });
  }

  private loadUserRoles(): void {
    this.svc.getUserRolesViewModel().subscribe(vm => {
      this.allRoles = vm.roles;
      this.users = vm.users;
    });
  }
}
