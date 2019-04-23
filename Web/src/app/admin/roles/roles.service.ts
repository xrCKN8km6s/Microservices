import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Role } from '../role';
import { RolesViewModel } from './models/roles-view-model';
import { CreateEditRole } from './models/create-edit-role';

@Injectable({
  providedIn: 'root'
})
export class RolesService {

  private rolesPath = 'api/roles';

  constructor(private client: HttpClient) { }

  public create(role: CreateEditRole): Observable<void> {
    return this.client.post<void>(`${environment.bffUrl}/${this.rolesPath}`, role);
  }

  public update(roleId: number, role: CreateEditRole): Observable<void> {
    return this.client.put<void>(`${environment.bffUrl}/${this.rolesPath}/${roleId}`, role);
  }

  public delete(roleId: number): Observable<void> {
    return this.client.delete<void>(`${environment.bffUrl}/${this.rolesPath}/${roleId}`);
  }

  public getRoles(): Observable<Role[]> {
    return this.client.get<Role[]>(`${environment.bffUrl}/${this.rolesPath}`);
  }

  public getRolesViewModel(): Observable<RolesViewModel> {
    return this.client.get<RolesViewModel>(`${environment.bffUrl}/${this.rolesPath}/viewmodel`);
  }

  public getRole(roleId: number): Observable<Role> {
    return this.client.get<Role>(`${environment.bffUrl}/${this.rolesPath}/${roleId}`);
  }
}
