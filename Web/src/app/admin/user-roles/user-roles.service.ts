import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserRolesViewModel } from './models/user-roles-view-model';
import { UserRoles } from './models/user-roles';

@Injectable()
export class UserRolesService {

  private usersPath = 'api/users';

  constructor(private client: HttpClient) { }

  public update(userId: number, roles: UserRoles): Observable<void> {
    return this.client.put<void>(`${environment.bffUrl}/${this.usersPath}/${userId}/roles`, roles);
  }

  public getUserRolesViewModel(): Observable<UserRolesViewModel> {
    return this.client.get<UserRolesViewModel>(`${environment.bffUrl}/${this.usersPath}/viewmodel`);
  }
}
