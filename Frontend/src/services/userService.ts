import { BaseService } from './baseService';
import { User } from '@/interfaces/userInterface';

class UserService extends BaseService<User> {
  constructor() {
    super({
      endpoint: 'User',
      downloadFileName: 'Usuarios',
    });
  }
}

export const userService = new UserService();