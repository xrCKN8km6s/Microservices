import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class RolesService {

  private basePath = 'http://localhost:5000/api/roles';

  constructor(private client: HttpClient) { }

  public create(role: CreateEditRoleDto): Observable<void> {
    return this.client.post<void>(this.basePath, role);
  }

  public update(roleId: number, role: CreateEditRoleDto): Observable<void> {
    return this.client.put<void>(`${this.basePath}/${roleId}`, role);
  }

  public delete(roleId: number): Observable<void> {
    return this.client.delete<void>(`${this.basePath}/${roleId}`);
  }

  public getRoles(): Observable<RoleDto[]> {
    return this.client.get<RoleDto[]>(this.basePath);
  }

  public getRolesViewModel(): Observable<RolesViewModel> {
    return this.client.get<RolesViewModel>(`${this.basePath}/viewmodel`);
  }

  public getRole(roleId: number): Observable<RoleDto> {
    return this.client.get<RoleDto>(`${this.basePath}/${roleId}`);
  }
}

export class CreateEditRoleDto {
  constructor(
    public name: string,
    public isGlobal: boolean,
    public permissions: number[]) { }
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
