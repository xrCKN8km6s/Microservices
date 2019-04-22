import { Injectable } from '@angular/core';
import { UserProfile } from './user-profile';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService {

  private profile: UserProfile;

  public setProfile(profile: UserProfile) {
    this.profile = profile;
  }

  public hasProfile(): boolean {
    return !!this.profile;
  }

  public hasPermission(permissionName: Permission): boolean {
    return this.profile.permissions.some(el => el.name === permissionName);
  }
}

export type Permission =
  'OrdersView' |
  'OrdersEdit' |
  'AdminView' |
  'AdminRolesView' |
  'AdminRolesEdit' |
  'AdminRolesDelete' |
  'AdminUsersView' |
  'AdminUsersEdit';
