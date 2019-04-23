import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { UserProfileService } from '../auth/user-profile.service';

@Injectable({
  providedIn: 'root'
})
export class OrdersGuard implements CanActivate {

  constructor(private userProfileService: UserProfileService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    return this.userProfileService.hasPermission('OrdersView');
  }
}
