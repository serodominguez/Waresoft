import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Store } from '@/interfaces/storeInterface';
import { storeService } from '@/services/storeService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useStoreStore = defineStore('store', () => {
  const items = ref<Store[]>([]);
  const selectedItem = ref<Store | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const stores = computed(() => items.value);
  const selectedStore = computed(() => selectedItem.value);
  const totalStores = computed(() => totalItems.value || 0);

  async function fetchStores(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const result = await storeService.fetchAll(params);
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

  async function downloadStoresExcel(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await storeService.downloadExcel(filterParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadStoresPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await storeService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function selectStore() {
    loading.value = true;
    items.value = [];

    try {
      const result = await storeService.select();
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

  async function fetchStoreById(id: number) {
    loading.value = true;

    try {
      const result = await storeService.fetchById(id);
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

  async function registerStore(store: Store) {
    try {
      const result = await storeService.create(store);
      if (result.isSuccess) {
        await fetchStores(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function editStore(id: number, store: Store) {
    try {
      const result = await storeService.update(id, store);
      if (result.isSuccess) {
        await fetchStores(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function enableStore(id: number) {
    try {
      const result = await storeService.enable(id);
      if (result.isSuccess) {
        await fetchStores(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableStore(id: number) {
    try {
      const result = await storeService.disable(id);
      if (result.isSuccess) {
        await fetchStores(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function removeStore(id: number) {
    try {
      const result = await storeService.remove(id);
      if (result.isSuccess) {
        await fetchStores(lastFilterParams.value || {});
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

    stores,
    selectedStore,
    totalStores,

    fetchStores,
    downloadStoresExcel,
    downloadStoresPdf,
    selectStore,
    fetchStoreById,
    registerStore,
    editStore,
    enableStore,
    disableStore,
    removeStore,
  };
});