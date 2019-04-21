import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable()
export class RolesService {

  private rolesPath = 'api/roles';

  constructor(private client: HttpClient) { }

  public create(role: CreateEditRoleDto): Observable<void> {
    return this.client.post<void>(`${environment.bffUrl}/${this.rolesPath}`, role);
  }

  public update(roleId: number, role: CreateEditRoleDto): Observable<void> {
    return this.client.put<void>(`${environment.bffUrl}/${this.rolesPath}/${roleId}`, role);
  }

  public delete(roleId: number): Observable<void> {
    return this.client.delete<void>(`${environment.bffUrl}/${this.rolesPath}/${roleId}`);
  }

  public getRoles(): Observable<RoleDto[]> {
    return this.client.get<RoleDto[]>(`${environment.bffUrl}/${this.rolesPath}`);
  }

  public getRolesViewModel(): Observable<RolesViewModel> {
    return this.client.get<RolesViewModel>(`${environment.bffUrl}/${this.rolesPath}/viewmodel`);
  }

  public getRole(roleId: number): Observable<RoleDto> {
    return this.client.get<RoleDto>(`${environment.bffUrl}/${this.rolesPath}/${roleId}`);
  }
}

export class CreateEditRoleDto {
    public name: string;
    public isGlobal: boolean;
    public permissions: number[];
}

export class RoleDto {
  constructor(
    public id: number,
    public name: string,
    public isGlobal: boolean,
    public permissions: number[]) { }
}

export class PermissionDto {
  constructor(
    public id: number,
    public name: string,
    public description: string) { }
}

export interface RoleViewModel {
  role: RoleDto;
  allPermissions: PermissionDto[];
}

export interface RolesViewModel {
  roles: RoleDto[];
  allPermissions: PermissionDto[];
}
