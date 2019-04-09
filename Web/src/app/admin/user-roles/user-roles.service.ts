import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RoleDto } from '../roles/roles.service';

@Injectable()
export class UserRolesService {

  private basePath = 'http://localhost:5000/api/users';

  constructor(private client: HttpClient) { }

  public update(userId: number, roles: UpdateUserRolesDto): Observable<void> {
    return this.client.put<void>(`${this.basePath}/${userId}/roles`, roles);
  }

  public getUserRolesViewModel(): Observable<UserRolesViewModel> {
    return this.client.get<UserRolesViewModel>(`${this.basePath}/viewmodel`);
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
