import { Role } from '../../role';
import { User } from '../models/user';

export interface EditUserRoleDialogData {
  readonly allRoles: ReadonlyArray<Role>;
  readonly user: User;
}
