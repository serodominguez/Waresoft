import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { GoodsIssue, GoodsIssueDetail } from '@/interfaces/goodsIssueInterface';
import { goodsIssueService } from '@/services/goodsIssueService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useGoodsIssueStore = defineStore('goodsIssue', () => {

  const items = ref<GoodsIssue[]>([]);
  const selectedItem = ref<GoodsIssue | null>(null);
  const selectedIssueDetails = ref<GoodsIssueDetail[]>([]);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const goodsissue = computed(() => items.value);
  const selectedGoodsIssue = computed(() => selectedItem.value);
  const totalGoodsIssue = computed(() => totalItems.value || 0);

  async function fetchGoodsIssue(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const resultado = await goodsIssueService.fetchAll(params);
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

  async function downloadGoodsIssueExcel(params?: FilterParams) {
    try {
      const filtrosParams = params || lastFilterParams.value || {};
      await goodsIssueService.downloadExcel(filtrosParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadGoodsIssuePdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await goodsIssueService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function fetchGoodsIssueById(issueId: number) {
    loading.value = true;

    try {
      const resultado = await goodsIssueService.getIssueWithDetails(issueId);
      if (resultado.isSuccess) {
        selectedItem.value = resultado.data;

        selectedIssueDetails.value = resultado.data.goodsIssueDetails?.map((detalle: any) => ({
          idProduct: detalle.idProduct,
          code: detalle.code,
          description: detalle.description,
          material: detalle.material,
          color: detalle.color,
          categoryName: detalle.categoryName,
          brandName: detalle.brandName,
          quantity: detalle.quantity,
          unitPrice: detalle.unitPrice,
          totalPrice: detalle.totalPrice
        })) || [];
      } else {
        error.value = resultado.message || resultado.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function registerGoodsIssue(issueData: any) {
    try {
      const resultado = await goodsIssueService.register(issueData);
      if (resultado.isSuccess) {
        await fetchGoodsIssue(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableGoodsIssue(issueId: number) {
    try {
      const resultado = await goodsIssueService.disable(issueId);
      if (resultado.isSuccess) {
        await fetchGoodsIssue(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function exportGoodsIssuePdf(issueId: number) {
    try {
      const { blob, filename } = await goodsIssueService.exportPdf(issueId);

      const url = window.URL.createObjectURL(blob);
      const enlace = document.createElement('a');
      enlace.href = url;
      enlace.setAttribute('download', filename);
      document.body.appendChild(enlace);
      enlace.click();
      enlace.parentNode?.removeChild(enlace);
      window.URL.revokeObjectURL(url);

      return { isSuccess: true };
    } catch (err: any) {
      console.error('Error al exportar PDF:', err);
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function openGoodsIssuePdf(issueId: number) {
    try {
      const { blob } = await goodsIssueService.exportPdf(issueId);
      const url = window.URL.createObjectURL(blob);
      window.open(url, '_blank');
      setTimeout(() => {
        window.URL.revokeObjectURL(url);
      }, 100);

      return { isSuccess: true };
    } catch (err: any) {
      console.error('Error al abrir PDF:', err);
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  return {
    items,
    selectedItem,
    selectedIssueDetails,
    totalItems,
    loading,
    error,
    lastFilterParams,

    goodsissue,
    selectedGoodsIssue,
    totalGoodsIssue,

    fetchGoodsIssue,
    downloadGoodsIssueExcel,
    downloadGoodsIssuePdf,
    fetchGoodsIssueById,
    registerGoodsIssue,
    disableGoodsIssue,
    exportGoodsIssuePdf,
    openGoodsIssuePdf,
  };
});