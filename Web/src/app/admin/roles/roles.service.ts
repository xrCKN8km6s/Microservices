import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Role } from './role.type';

@Injectable()
export class RolesService {

  constructor(private client: HttpClient) { }

  public getRoles(): Observable<Role[]> {
    return this.client.get<Role[]>('http://localhost:5000/api/roles');
  }
}
