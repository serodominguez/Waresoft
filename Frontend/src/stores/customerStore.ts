import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Customer } from '@/interfaces/customerInterface';
import { customerService } from '@/services/customerService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useCustomerStore = defineStore('customer', () => {
  const items = ref<Customer[]>([]);
  const selectedItem = ref<Customer | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const customers = computed(() => items.value);
  const selectedCustomer = computed(() => selectedItem.value);
  const totalCustomers = computed(() => totalItems.value || 0);

  async function fetchCustomers(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const resultado = await customerService.fetchAll(params);
      if (resultado.isSuccess) {
        items.value = resultado.data;
        totalItems.value = resultado.totalRecords;
      } else {
        error.value = resultado.message || resultado.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function downloadCustomersExcel(params?: FilterParams) {
    try {
      const filtrosParams = params || lastFilterParams.value || {};
      await customerService.downloadExcel(filtrosParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadCustomersPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await customerService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function fetchCustomerById(id: number) {
    loading.value = true;

    try {
      const resultado = await customerService.fetchById(id);
      if (resultado.isSuccess) {
        selectedItem.value = resultado.data;
      } else {
        error.value = resultado.message || resultado.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function registerCustomer(cliente: Customer) {
    try {
      const resultado = await customerService.create(cliente);
      if (resultado.isSuccess) {
        await fetchCustomers(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function editCustomer(id: number, cliente: Customer) {
    try {
      const resultado = await customerService.update(id, cliente);
      if (resultado.isSuccess) {
        await fetchCustomers(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function enableCustomer(id: number) {
    try {
      const resultado = await customerService.enable(id);
      if (resultado.isSuccess) {
        await fetchCustomers(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableCustomer(id: number) {
    try {
      const resultado = await customerService.disable(id);
      if (resultado.isSuccess) {
        await fetchCustomers(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function removeCustomer(id: number) {
    try {
      const resultado = await customerService.remove(id);
      if (resultado.isSuccess) {
        await fetchCustomers(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  return {
    items,
    selectedItem,
    totalItems,
    loading,
    error,
    lastFilterParams,

    customers,
    selectedCustomer,
    totalCustomers,

    fetchCustomers,
    downloadCustomersExcel,
    downloadCustomersPdf,
    fetchCustomerById,
    registerCustomer,
    editCustomer,
    enableCustomer,
    disableCustomer,
    removeCustomer,
  };
});