import { Role } from '../../role';
import { Permission } from './permission';

export interface RolesViewModel {
  roles: ReadonlyArray<Role>;
  allPermissions: ReadonlyArray<Permission>;
}
