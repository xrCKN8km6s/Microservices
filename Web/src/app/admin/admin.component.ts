import { Component, OnInit } from '@angular/core';
import { UserProfileService } from '../auth/user-profile.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {

  public isRolesVisible: boolean;
  public isUsersVisible: boolean;

  constructor(private userProfileService: UserProfileService) { }

  ngOnInit() {
    this.isRolesVisible = this.userProfileService.hasPermission('AdminRolesView');
    this.isUsersVisible = this.userProfileService.hasPermission('AdminUsersView');
  }
}
