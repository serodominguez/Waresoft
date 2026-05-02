import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { KardexDetail } from '@/interfaces/kardexInterface';
import { kardexService } from '@/services/kardexService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useKardexStore = defineStore('kardex', () => {
  const kardexDetail = ref<KardexDetail | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const lastProductId = ref<number | null>(null);
  const lastFilterParams = ref<FilterParams>({});

  const kardex = computed(() => kardexDetail.value);
  const totalKardex = computed(() => totalItems.value);

  async function fetchKardex(productId: number, params: FilterParams = {}, signal?: AbortSignal) {
    loading.value = true;
    kardexDetail.value = null;
    lastProductId.value = productId;
    lastFilterParams.value = params;

    try {
      const result = await kardexService.fetchKardex(productId, params, signal);
      
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      
      kardexDetail.value = result.data;
      totalItems.value = result.totalRecords;
    } catch (err: any) {
      if (err.name === 'AbortError' || err.code === 'ERR_CANCELED') return;
      throw err;
    } finally {
      if (!signal?.aborted) {
        loading.value = false;
      }
    }
  }

  async function downloadKardexExcel(params?: FilterParams) {
    if (!lastProductId.value) return;
    await kardexService.downloadKardexExcel(lastProductId.value, params ?? lastFilterParams.value);
  }

  async function downloadKardexPdf(params?: FilterParams) {
    if (!lastProductId.value) return;
    await kardexService.downloadKardexPdf(lastProductId.value, params ?? lastFilterParams.value);
  }

  function clearKardex() {
    kardexDetail.value = null;
    totalItems.value = 0;
    lastProductId.value = null;
    lastFilterParams.value = {};
  }

  return {
    kardexDetail,
    totalItems,
    loading,
    lastProductId,
    lastFilterParams,
    kardex,
    totalKardex,

    fetchKardex,
    downloadKardexExcel,
    downloadKardexPdf,
    clearKardex,
  };
});
