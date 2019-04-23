import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { UserProfileService } from '../auth/user-profile.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  constructor(private userProfileService: UserProfileService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    return this.userProfileService.hasPermission('AdminView');
  }
}

@Injectable({
  providedIn: 'root'
})
export class AdminUsersGuard implements CanActivate {

  constructor(private userProfileService: UserProfileService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    return this.userProfileService.hasPermission('AdminUsersView');
  }
}

@Injectable({
  providedIn: 'root'
})
export class AdminRolesGuard implements CanActivate {

  constructor(private userProfileService: UserProfileService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    return this.userProfileService.hasPermission('AdminRolesView');
  }
}
