import { BaseService } from './baseService';
import { Store } from '@/interfaces/storeInterface';

class StoreService extends BaseService<Store> {
  constructor() {
    super({
      endpoint: 'Store',
      downloadFileName: 'Tiendas',
    });
  }
}

export const storeService = new StoreService();