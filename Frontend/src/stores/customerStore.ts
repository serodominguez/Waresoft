import { createBaseStore } from '@/stores/baseStore';
import { Customer } from '@/interfaces/customerInterface';
import { customerService } from '@/services/customerService';

export const useCustomerStore = createBaseStore<Customer>('customer', customerService);