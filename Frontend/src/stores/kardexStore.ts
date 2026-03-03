import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { KardexDetail } from '@/interfaces/kardexInterface';
import { kardexService } from '@/services/kardexService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useKardexStore = defineStore('kardex', () => {
  const kardexDetail = ref<KardexDetail | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastProductId = ref<number | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const kardex = computed(() => kardexDetail.value);
  const totalKardex = computed(() => totalItems.value || 0);

  async function fetchKardex(productId: number, params: FilterParams = {}) {
    loading.value = true;
    kardexDetail.value = null;
    lastProductId.value = productId;
    lastFilterParams.value = params;

    try {
      const resultado = await kardexService.fetchKardex(productId, params);
      if (resultado.isSuccess) {
        kardexDetail.value = resultado.data;
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

  function clearKardex() {
    kardexDetail.value = null;
    totalItems.value = 0;
    lastProductId.value = null;
    lastFilterParams.value = undefined;
    error.value = null;
  }

  return {
    kardexDetail,
    totalItems,
    totalKardex,
    loading,
    error,
    lastProductId,
    lastFilterParams,

    kardex,
    fetchKardex,
    clearKardex,
  };
});