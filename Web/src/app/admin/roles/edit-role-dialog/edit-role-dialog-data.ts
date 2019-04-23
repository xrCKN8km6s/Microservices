import { Permission } from '../models/permission';
import { DialogMode } from './dialog-mode';

export interface EditRoleDialogData {
  readonly mode: DialogMode;
  readonly allPermissions: ReadonlyArray<Permission>;
  readonly roleId: number;
}
