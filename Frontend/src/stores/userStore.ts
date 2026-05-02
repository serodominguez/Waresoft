import { createBaseStore } from '@/stores/baseStore';
import { User } from '@/interfaces/userInterface';
import { userService } from '@/services/userService';

export const useUserStore = createBaseStore<User>('user', userService)