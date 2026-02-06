import { BaseService } from './baseService';
import { Role } from '@/interfaces/roleInterface';

class RoleService extends BaseService<Role> {
  constructor() {
    super({
      endpoint: 'Role',
      downloadFileName: 'Roles',
    });
  }
}

export const roleService = new RoleService();