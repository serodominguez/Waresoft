import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { GoodsReceipt, GoodsReceiptDetail } from '@/interfaces/goodsReceiptInterface';
import { goodsReceiptService } from '@/services/goodsReceiptService';
import { FilterParams } from '@/interfaces/baseInterface';
import type { GoodsReceiptRegister } from '@/interfaces/goodsReceiptInterface';

export const useGoodsReceiptStore = defineStore('goodsReceipt', () => {
  const items = ref<GoodsReceipt[]>([]);
  const selectedItem = ref<GoodsReceipt | null>(null);
  const selectedReceiptDetails = ref<GoodsReceiptDetail[]>([]);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const lastFilterParams = ref<FilterParams>({});

  const goodsreceipt = computed(() => items.value);
  const selectedGoodsReceipt = computed(() => selectedItem.value);
  const totalGoodsReceipt = computed(() => totalItems.value);

  async function fetchGoodsReceipt(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;
    try {
      const result = await goodsReceiptService.fetchAll(params);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      items.value = result.data;
      totalItems.value = result.totalRecords;
    } finally {
      loading.value = false;
    }
  }

  async function fetchGoodsReceiptById(receiptId: number) {
    loading.value = true;
    try {
      const result = await goodsReceiptService.getReceiptWithDetails(receiptId);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      
      selectedItem.value = result.data;
      selectedReceiptDetails.value = result.data.goodsReceiptDetails?.map((detalle: any) => ({
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
    } finally {
      loading.value = false;
    }
  }

  async function downloadGoodsReceiptExcel(params?: FilterParams) {
    await goodsReceiptService.downloadExcel(params ?? lastFilterParams.value);
  }

  async function downloadGoodsReceiptPdf(params?: FilterParams) {
    await goodsReceiptService.downloadPdf(params ?? lastFilterParams.value);
  }

  async function exportGoodsReceiptPdf(receiptId: number) {
    const { blob, filename } = await goodsReceiptService.exportPdf(receiptId);
    const url = window.URL.createObjectURL(blob);
    const enlace = document.createElement('a');
    enlace.href = url;
    enlace.setAttribute('download', filename);
    document.body.appendChild(enlace);
    enlace.click();
    enlace.parentNode?.removeChild(enlace);
    window.URL.revokeObjectURL(url);
  }

  async function openGoodsReceiptPdf(receiptId: number) {
    const { blob } = await goodsReceiptService.exportPdf(receiptId);
    const url = window.URL.createObjectURL(blob);
    window.open(url, '_blank');
    setTimeout(() => {
      window.URL.revokeObjectURL(url);
    }, 100);
  }

  async function getBlobGoodsReceiptPdf(receiptId: number): Promise<Blob> {
    const { blob } = await goodsReceiptService.exportPdf(receiptId);
    return blob;
  }

  async function registerGoodsReceipt(receiptData: GoodsReceiptRegister) {
    const result = await goodsReceiptService.register(receiptData);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await fetchGoodsReceipt(lastFilterParams.value);
    return result;
  }

  async function disableGoodsReceipt(receiptId: number) {
    const result = await goodsReceiptService.disable(receiptId);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await fetchGoodsReceipt(lastFilterParams.value);
    return result;
  }

  return {
    items,
    selectedItem,
    selectedReceiptDetails,
    totalItems,
    loading,
    lastFilterParams,
    goodsreceipt,
    selectedGoodsReceipt,
    totalGoodsReceipt,

    fetchGoodsReceipt,
    fetchGoodsReceiptById,
    downloadGoodsReceiptExcel,
    downloadGoodsReceiptPdf,
    exportGoodsReceiptPdf,
    openGoodsReceiptPdf,
    getBlobGoodsReceiptPdf,
    registerGoodsReceipt,
    cancel: disableGoodsReceipt
  };
});
