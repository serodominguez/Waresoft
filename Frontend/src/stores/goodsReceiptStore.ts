import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { GoodsReceipt, GoodsReceiptDetail } from '@/interfaces/goodsReceiptInterface';
import { goodsReceiptService } from '@/services/goodsReceiptService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useGoodsReceiptStore = defineStore('goodsReceipt', () => {
  const items = ref<GoodsReceipt[]>([]);
  const selectedItem = ref<GoodsReceipt | null>(null);
  const selectedReceiptDetails = ref<GoodsReceiptDetail[]>([]);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const goodsreceipt = computed(() => items.value);
  const selectedGoodsReceipt = computed(() => selectedItem.value);
  const totalGoodsReceipt = computed(() => totalItems.value || 0);

  async function fetchGoodsReceipt(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const resultado = await goodsReceiptService.fetchAll(params);
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

  async function downloadGoodsReceiptExcel(params?: FilterParams) {
    try {
      const filtrosParams = params || lastFilterParams.value || {};
      await goodsReceiptService.downloadExcel(filtrosParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadGoodsReceiptPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await goodsReceiptService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function fetchGoodsReceiptById(receiptId: number) {
    loading.value = true;

    try {
      const resultado = await goodsReceiptService.getReceiptWithDetails(receiptId);
      if (resultado.isSuccess) {
        selectedItem.value = resultado.data;

        selectedReceiptDetails.value = resultado.data.goodsReceiptDetails?.map((detalle: any) => ({
          idProduct: detalle.idProduct,
          code: detalle.code,
          description: detalle.description,
          material: detalle.material,
          color: detalle.color,
          categoryName: detalle.categoryName,
          brandName: detalle.brandName,
          quantity: detalle.quantity,
          unitCost: detalle.unitCost,
          totalCost: detalle.totalCost
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

  async function registerGoodsReceipt(receiptData: any) {
    try {
      const resultado = await goodsReceiptService.register(receiptData);
      if (resultado.isSuccess) {
        await fetchGoodsReceipt(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableGoodsReceipt(receiptId: number) {
    try {
      const resultado = await goodsReceiptService.disable(receiptId);
      if (resultado.isSuccess) {
        await fetchGoodsReceipt(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function exportGoodsReceiptPdf(receiptId: number) {
    try {
      const { blob, filename } = await goodsReceiptService.exportPdf(receiptId);

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

  async function openGoodsReceiptPdf(receiptId: number) {
    try {
      const { blob } = await goodsReceiptService.exportPdf(receiptId);
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
    selectedReceiptDetails,
    totalItems,
    loading,
    error,
    lastFilterParams,

    goodsreceipt,
    selectedGoodsReceipt,
    totalGoodsReceipt,

    fetchGoodsReceipt,
    downloadGoodsReceiptExcel,
    downloadGoodsReceiptPdf,
    fetchGoodsReceiptById,
    registerGoodsReceipt,
    disableGoodsReceipt,
    exportGoodsReceiptPdf,
    openGoodsReceiptPdf,
  };
});