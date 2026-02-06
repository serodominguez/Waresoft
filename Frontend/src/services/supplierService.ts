import { BaseService } from './baseService';
import { Supplier } from '@/interfaces/supplierInterface';

class SupplierService extends BaseService<Supplier> {
  constructor() {
    super({
      endpoint: 'Supplier',
      downloadFileName: 'Proveedores',
    });
  }
}

export const supplierService = new SupplierService();