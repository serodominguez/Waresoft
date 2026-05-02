import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { GoodsIssue, GoodsIssueDetail } from '@/interfaces/goodsIssueInterface';
import { goodsIssueService } from '@/services/goodsIssueService';
import { FilterParams } from '@/interfaces/baseInterface';
import type { GoodsIssueRegister } from '@/interfaces/goodsIssueInterface';

export const useGoodsIssueStore = defineStore('goodsIssue', () => {
  const items = ref<GoodsIssue[]>([]);
  const selectedItem = ref<GoodsIssue | null>(null);
  const selectedIssueDetails = ref<GoodsIssueDetail[]>([]);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const lastFilterParams = ref<FilterParams>({});

  const goodsissue = computed(() => items.value);
  const selectedGoodsIssue = computed(() => selectedItem.value);
  const totalGoodsIssue = computed(() => totalItems.value);

  async function fetchGoodsIssue(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;
    try {
      const result = await goodsIssueService.fetchAll(params);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      items.value = result.data;
      totalItems.value = result.totalRecords;
    } finally {
      loading.value = false;
    }
  }

  async function fetchGoodsIssueById(issueId: number) {
    loading.value = true;
    try {
      const result = await goodsIssueService.getIssueWithDetails(issueId);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      
      selectedItem.value = result.data;
      
      selectedIssueDetails.value = result.data.goodsIssueDetails?.map((detalle: any) => ({
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
    } finally {
      loading.value = false;
    }
  }

  async function downloadGoodsIssueExcel(params?: FilterParams) {
    await goodsIssueService.downloadExcel(params ?? lastFilterParams.value);
  }

  async function downloadGoodsIssuePdf(params?: FilterParams) {
    await goodsIssueService.downloadPdf(params ?? lastFilterParams.value);
  }

  async function exportGoodsIssuePdf(issueId: number) {
    const { blob, filename } = await goodsIssueService.exportPdf(issueId);
    const url = window.URL.createObjectURL(blob);
    const enlace = document.createElement('a');
    enlace.href = url;
    enlace.setAttribute('download', filename);
    document.body.appendChild(enlace);
    enlace.click();
    enlace.parentNode?.removeChild(enlace);
    window.URL.revokeObjectURL(url);
  }

  async function openGoodsIssuePdf(issueId: number) {
    const { blob } = await goodsIssueService.exportPdf(issueId);
    const url = window.URL.createObjectURL(blob);
    window.open(url, '_blank');
    setTimeout(() => {
      window.URL.revokeObjectURL(url);
    }, 100);
  }

  async function getBlobGoodsIssuePdf(issueId: number): Promise<Blob> {
    const { blob } = await goodsIssueService.exportPdf(issueId);
    return blob;
  }

  async function registerGoodsIssue(issueData: GoodsIssueRegister) {
    const result = await goodsIssueService.register(issueData);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await fetchGoodsIssue(lastFilterParams.value);
    return result;
  }

  async function disableGoodsIssue(issueId: number) {
    const result = await goodsIssueService.disable(issueId);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await fetchGoodsIssue(lastFilterParams.value);
    return result;
  }

  return {
    items,
    selectedItem,
    selectedIssueDetails,
    totalItems,
    loading,
    lastFilterParams,
    goodsissue,
    selectedGoodsIssue,
    totalGoodsIssue,

    fetchGoodsIssue,
    fetchGoodsIssueById,
    downloadGoodsIssueExcel,
    downloadGoodsIssuePdf,
    exportGoodsIssuePdf,
    openGoodsIssuePdf,
    getBlobGoodsIssuePdf,
    registerGoodsIssue,
    cancel: disableGoodsIssue
  };
});
