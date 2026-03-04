import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { KardexDetail } from '@/interfaces/kardexInterface';
import { kardexService } from '@/services/kardexService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useKardexStore = defineStore('kardex', () => {
  // Estado
  const kardexDetail = ref<KardexDetail | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastProductId = ref<number | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  // Getters
  const kardex = computed(() => kardexDetail.value);
  const totalKardex = computed(() => totalItems.value || 0);

  // Acciones
  async function fetchKardex(productId: number, params: FilterParams = {}) {
    loading.value = true;
    kardexDetail.value = null;
    lastProductId.value = productId;
    lastFilterParams.value = params;

    try {
      const result = await kardexService.fetchKardex(productId, params);
      if (result.isSuccess) {
        kardexDetail.value = result.data;
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

  async function downloadKardexExcel(params?: FilterParams) {
    try {
      const productId = lastProductId.value!;
      const filterParams = params || lastFilterParams.value || {};
      await kardexService.downloadKardexExcel(productId, filterParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadKardexPdf(params?: FilterParams) {
    try {
      const productId = lastProductId.value!;
      const filterParams = params || lastFilterParams.value || {};
      await kardexService.downloadKardexPdf(productId, filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  function clearKardex() {
    kardexDetail.value = null;
    totalItems.value = 0;
    lastProductId.value = null;
    lastFilterParams.value = undefined;
    error.value = null;
  }

  return {
    // State
    kardexDetail,
    totalItems,
    loading,
    error,
    lastProductId,
    lastFilterParams,

    // Getters
    kardex,
    totalKardex,

    // Actions
    fetchKardex,
    downloadKardexExcel,
    downloadKardexPdf,
    clearKardex,
  };
});