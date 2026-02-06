import { BaseService } from './baseService';
import { Customer } from '@/interfaces/customerInterface';

class CustomerService extends BaseService<Customer> {
  constructor() {
    super({
      endpoint: 'Customer',
      downloadFileName: 'Clientes',
    });
  }
}

export const customerService = new CustomerService();