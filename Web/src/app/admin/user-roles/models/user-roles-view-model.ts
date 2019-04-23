import { Role } from '../../role';
import { User } from './user';

export interface UserRolesViewModel {
  readonly users: ReadonlyArray<User>;
  readonly roles: ReadonlyArray<Role>;
}
