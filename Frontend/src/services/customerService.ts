import axios from 'axios';
import { BaseService } from './baseService';
import { Customer, CustomerStats } from '@/interfaces/customerInterface';

class CustomerService extends BaseService<Customer> {
  constructor() {
    super({
      endpoint: 'Customer',
      downloadFileName: 'Clientes',
    });
  }

  async getStats(): Promise<CustomerStats> {
    const response = await axios.get<{ data: CustomerStats }>(`api/Customer/Stats`);
    return response.data.data;
  }
}

export const customerService = new CustomerService();