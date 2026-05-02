import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Transfer, TransferDetail } from '@/interfaces/transferInterface';
import { transferService } from '@/services/transferService';
import { FilterParams } from '@/interfaces/baseInterface';
import type { TransferRegister } from '@/interfaces/transferInterface';

export const useTransferStore = defineStore('transfer', () => {
  const items = ref<Transfer[]>([]);
  const selectedItem = ref<Transfer | null>(null);
  const selectedTransferDetails = ref<TransferDetail[]>([]);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const lastFilterParams = ref<FilterParams>({});

  const transfers = computed(() => items.value);
  const selectedTransfer = computed(() => selectedItem.value);
  const totalTransfers = computed(() => totalItems.value);

  async function fetchTransfers(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;
    try {
      const result = await transferService.fetchAll(params);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      items.value = result.data;
      totalItems.value = result.totalRecords;
    } finally {
      loading.value = false;
    }
  }

  async function fetchTransferById(transferId: number) {
    loading.value = true;
    try {
      const result = await transferService.getTransferWithDetails(transferId);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      
      selectedItem.value = result.data;
      selectedTransferDetails.value = result.data.transferDetails?.map((detail: any) => ({
        idProduct: detail.idProduct,
        code: detail.code,
        description: detail.description,
        material: detail.material,
        color: detail.color,
        categoryName: detail.categoryName,
        brandName: detail.brandName,
        quantity: detail.quantity,
        unitPrice: detail.unitPrice,
        totalPrice: detail.totalPrice
      })) || [];
    } finally {
      loading.value = false;
    }
  }

  async function downloadTransferExcel(params?: FilterParams) {
    await transferService.downloadExcel(params ?? lastFilterParams.value);
  }

  async function downloadTransferPdf(params?: FilterParams) {
    await transferService.downloadPdf(params ?? lastFilterParams.value);
  }

  async function exportTransferPdf(transferId: number) {
    const { blob, filename } = await transferService.exportPdf(transferId);
    const url = window.URL.createObjectURL(blob);
    const enlace = document.createElement('a');
    enlace.href = url;
    enlace.setAttribute('download', filename);
    document.body.appendChild(enlace);
    enlace.click();
    enlace.parentNode?.removeChild(enlace);
    window.URL.revokeObjectURL(url);
  }

  async function openTransferPdf(transferId: number) {
    const { blob } = await transferService.exportPdf(transferId);
    const url = window.URL.createObjectURL(blob);
    window.open(url, '_blank');
    setTimeout(() => {
      window.URL.revokeObjectURL(url);
    }, 100);
  }

  async function getBlobTransferPdf(transferId: number): Promise<Blob> {
    const { blob } = await transferService.exportPdf(transferId);
    return blob;
  }

  async function sendTrasnfer(transferData: TransferRegister) {
    const result = await transferService.send(transferData);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await fetchTransfers(lastFilterParams.value);
    return result;
  }

  async function receiveTransfer(transferId: number) {
    const result = await transferService.receive(transferId);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await fetchTransfers(lastFilterParams.value);
    return result;
  }

  async function disableTransfer(transferId: number) {
    const result = await transferService.disable(transferId);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await fetchTransfers(lastFilterParams.value);
    return result;
  }

  return {
    items,
    selectedItem,
    selectedTransferDetails,
    totalItems,
    loading,
    lastFilterParams,
    transfers,
    selectedTransfer,
    totalTransfers,

    fetchTransfers,
    fetchTransferById,
    downloadTransferExcel,
    downloadTransferPdf,
    exportTransferPdf,
    openTransferPdf,
    getBlobTransferPdf,
    sendTrasnfer,
    receiveTransfer,
    cancel: disableTransfer
  };
});
