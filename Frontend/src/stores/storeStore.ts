import { createBaseStore } from '@/stores/baseStore';
import { Store } from '@/interfaces/storeInterface';
import { storeService } from '@/services/storeService';

export const useStoreStore = createBaseStore<Store>('store', storeService)