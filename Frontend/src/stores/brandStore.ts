import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Brand } from '@/interfaces/brandInterface';
import { brandService } from '@/services/brandService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useBrandStore = defineStore('brand', () => {
  // Estado
  const items = ref<Brand[]>([]);
  const selectedItem = ref<Brand | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  // Getters
  const brands = computed(() => items.value);
  const selectedBrand = computed(() => selectedItem.value);
  const totalBrands = computed(() => totalItems.value || 0);

  // Acciones
  async function fetchBrands(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const result = await brandService.fetchAll(params);
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

  async function downloadBrandsExcel(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await brandService.downloadExcel(filterParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadBrandsPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await brandService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }
  
  async function selectBrand() {
    loading.value = true;
    items.value = [];

    try {
      const result = await brandService.select();
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

  async function fetchBrandById(id: number) {
    loading.value = true;

    try {
      const result = await brandService.fetchById(id);
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

  async function registerBrand(brand: Brand) {
    try {
      const result = await brandService.create(brand);
      if (result.isSuccess) {
        await fetchBrands(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function editBrand(id: number, brand: Brand) {
    try {
      const result = await brandService.update(id, brand);
      if (result.isSuccess) {
        await fetchBrands(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function enableBrand(id: number) {
    try {
      const result = await brandService.enable(id);
      if (result.isSuccess) {
        await fetchBrands(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableBrand(id: number) {
    try {
      const result = await brandService.disable(id);
      if (result.isSuccess) {
        await fetchBrands(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function removeBrand(id: number) {
    try {
      const result = await brandService.remove(id);
      if (result.isSuccess) {
        await fetchBrands(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  return {
    // State
    items,
    selectedItem,
    totalItems,
    loading,
    error,
    lastFilterParams,

    // Getters
    brands,
    selectedBrand,
    totalBrands,

    // Actions
    fetchBrands,
    downloadBrandsExcel,
    downloadBrandsPdf,
    selectBrand,
    fetchBrandById,
    registerBrand,
    editBrand,
    enableBrand,
    disableBrand,
    removeBrand,
  };
});