import { createBaseStore } from '@/stores/baseStore';
import { Role } from '@/interfaces/roleInterface';
import { roleService } from '@/services/roleService';

export const useRoleStore = createBaseStore<Role>('role', roleService)