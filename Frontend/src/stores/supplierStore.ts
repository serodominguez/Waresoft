import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Supplier } from '@/interfaces/supplierInterface';
import { supplierService } from '@/services/supplierService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useSupplierStore = defineStore('supplier', () => {
  const items = ref<Supplier[]>([]);
  const selectedItem = ref<Supplier | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const suppliers = computed(() => items.value);
  const selectedSupplier = computed(() => selectedItem.value);
  const totalSuppliers = computed(() => totalItems.value || 0);

  async function fetchSuppliers(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const result = await supplierService.fetchAll(params);
      if (result.isSuccess) {
        items.value = result.data;
        totalItems.value = result.totalRecords;
      } else {
        error.value = result.message || result.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function downloadSuppliersExcel(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await supplierService.downloadExcel(filterParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadSuppliersPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await supplierService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function selectSupplier() {
    loading.value = true;
    items.value = [];

    try {
      const result = await supplierService.select();
      if (result.isSuccess) {
        items.value = result.data;
      } else {
        error.value = result.message || result.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function fetchSupplierById(id: number) {
    loading.value = true;

    try {
      const result = await supplierService.fetchById(id);
      if (result.isSuccess) {
        selectedItem.value = result.data;
      } else {
        error.value = result.message || result.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function registerSupplier(supplier: Supplier) {
    try {
      const result = await supplierService.create(supplier);
      if (result.isSuccess) {
        await fetchSuppliers(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function editSupplier(id: number, supplier: Supplier) {
    try {
      const result = await supplierService.update(id, supplier);
      if (result.isSuccess) {
        await fetchSuppliers(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function enableSupplier(id: number) {
    try {
      const result = await supplierService.enable(id);
      if (result.isSuccess) {
        await fetchSuppliers(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableSupplier(id: number) {
    try {
      const result = await supplierService.disable(id);
      if (result.isSuccess) {
        await fetchSuppliers(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function removeSupplier(id: number) {
    try {
      const result = await supplierService.remove(id);
      if (result.isSuccess) {
        await fetchSuppliers(lastFilterParams.value || {});
      }
      return result;
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

    suppliers,
    selectedSupplier,
    totalSuppliers,

    fetchSuppliers,
    downloadSuppliersExcel,
    downloadSuppliersPdf,
    selectSupplier,
    fetchSupplierById,
    registerSupplier,
    editSupplier,
    enableSupplier,
    disableSupplier,
    removeSupplier,
  };
});