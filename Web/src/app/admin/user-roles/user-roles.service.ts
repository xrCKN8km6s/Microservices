import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RoleDto } from '../roles/roles.service';
import { environment } from 'src/environments/environment';

@Injectable()
export class UserRolesService {

  private usersPath = 'api/users';

  constructor(private client: HttpClient) { }

  public update(userId: number, roles: UpdateUserRolesDto): Observable<void> {
    return this.client.put<void>(`${environment.bffUrl}/${this.usersPath}/${userId}/roles`, roles);
  }

  public getUserRolesViewModel(): Observable<UserRolesViewModel> {
    return this.client.get<UserRolesViewModel>(`${environment.bffUrl}/${this.usersPath}/viewmodel`);
  }
}

export class UserDto {
    public id: number;
    public name: string;
    public roles: number[];
}

export class UserRolesViewModel {
  users: UserDto[];
  roles: RoleDto[];
}

export class UpdateUserRolesDto {
  roles: number[];
}
