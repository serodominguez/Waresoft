import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Transfer, TransferDetail } from '@/interfaces/transferInterface';
import { transferService } from '@/services/transferService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useTransferStore = defineStore('transfer', () => {

  const items = ref<Transfer[]>([]);
  const selectedItem = ref<Transfer | null>(null);
  const selectedTransferDetails = ref<TransferDetail[]>([]);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const transfers = computed(() => items.value);
  const selectedTransfer = computed(() => selectedItem.value);
  const totalTransfers = computed(() => totalItems.value || 0);

  async function fetchTransfers(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const resultado = await transferService.fetchAll(params);
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

  async function downloadTransferExcel(params?: FilterParams) {
    try {
      const filtrosParams = params || lastFilterParams.value || {};
      await transferService.downloadExcel(filtrosParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadTransferPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await transferService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function fetchTransferById(transferId: number) {
    loading.value = true;

    try {
      const resultado = await transferService.getTransferWithDetails(transferId);
      if (resultado.isSuccess) {
        selectedItem.value = resultado.data;

        selectedTransferDetails.value = resultado.data.transferDetails?.map((detail: any) => ({
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
      } else {
        error.value = resultado.message || resultado.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function sendTrasnfer(transferData: any) {
    try {
      const resultado = await transferService.create(transferData);
      if (resultado.isSuccess) {
        await fetchTransfers(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function receiveTransfer(transferId: number) {
    try {
      const resultado = await transferService.receive(transferId);
      if (resultado.isSuccess) {
        await fetchTransfers(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableTransfer(transferId: number) {
    try {
      const resultado = await transferService.disable(transferId);
      if (resultado.isSuccess) {
        await fetchTransfers(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function exportTransferPdf(transferId: number) {
    try {
      const { blob, filename } = await transferService.exportPdf(transferId);

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

  async function openTransferPdf(transferId: number) {
    try {
      const { blob } = await transferService.exportPdf(transferId);
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
    selectedTransferDetails,
    totalItems,
    loading,
    error,
    lastFilterParams,

    transfers,
    selectedTransfer,
    totalTransfers,

    fetchTransfers,
    downloadTransferExcel,
    downloadTransferPdf,
    fetchTransferById,
    sendTrasnfer,
    receiveTransfer,
    disableTransfer,
    exportTransferPdf,
    openTransferPdf,
  };
});