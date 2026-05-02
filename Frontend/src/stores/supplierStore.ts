import { createBaseStore } from '@/stores/baseStore';
import { Supplier } from '@/interfaces/supplierInterface';
import { supplierService } from '@/services/supplierService';

export const useSupplierStore = createBaseStore<Supplier>('supplier', supplierService)