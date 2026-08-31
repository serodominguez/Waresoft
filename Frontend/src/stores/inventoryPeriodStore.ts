import { defineStore, storeToRefs } from 'pinia';
import { ref } from 'vue';
import {
  InventoryPeriod,
  InventoryPeriodDetail,
  InventoryPeriodOpeningResponse,
  InventoryPeriodClosingResponse,
  InventoryPeriodOpenRequest,
  InventoryPeriodCloseRequest,
} from '@/interfaces/inventoryPeriodInterface';
import { inventoryPeriodService } from '@/services/inventoryPeriodService';
import { FilterParams } from '@/interfaces/baseInterface';
import { createBaseStore } from '@/stores/baseStore';

const useBaseInventoryPeriodStore = createBaseStore<InventoryPeriod>('inventory-period-base', inventoryPeriodService);

export const useInventoryPeriodStore = defineStore('inventory-period', () => {
  const base = useBaseInventoryPeriodStore();

  const {
    items, selectedItem, totalItems, loading, lastFilterParams,
    list, selected, total
  } = storeToRefs(base);

  // Estado extra
  const periodDetail = ref<InventoryPeriodDetail | null>(null);
  const openingItems = ref<InventoryPeriodOpeningResponse[]>([]);
  const closingItems = ref<InventoryPeriodClosingResponse[]>([]);
  const systemStockItems = ref<InventoryPeriodClosingResponse[]>([]);
  const totalOpeningItems = ref<number>(0);
  const totalClosingItems = ref<number>(0);
  const totalSystemStockItems = ref<number>(0);

  async function fetchPeriods(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;
    try {
      const result = await inventoryPeriodService.fetchPeriods(params);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      items.value = result.data;
      totalItems.value = result.totalRecords;
    } finally {
      loading.value = false;
    }
  }

  async function fetchPeriodDetail(periodId: number) {
    loading.value = true;
    periodDetail.value = null;
    try {
      const result = await inventoryPeriodService.fetchDetail(periodId);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      periodDetail.value = result.data;
    } finally {
      loading.value = false;
    }
  }

  async function fetchOpening(periodId: number) {
    loading.value = true;
    openingItems.value = [];
    try {
      const result = await inventoryPeriodService.fetchOpening(periodId);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      openingItems.value = result.data;
      totalOpeningItems.value = result.totalRecords;
    } finally {
      loading.value = false;
    }
  }

  async function fetchClosing(periodId: number) {
    loading.value = true;
    closingItems.value = [];
    try {
      const result = await inventoryPeriodService.fetchClosing(periodId);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      closingItems.value = result.data;
      totalClosingItems.value = result.totalRecords;
    } finally {
      loading.value = false;
    }
  }

  async function fetchSystemStock(periodId: number) {
    loading.value = true;
    systemStockItems.value = [];
    try {
      const result = await inventoryPeriodService.fetchSystemStock(periodId);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      systemStockItems.value = result.data;
      totalSystemStockItems.value = result.totalRecords;
    } finally {
      loading.value = false;
    }
  }

  async function openPeriod(data: InventoryPeriodOpenRequest) {
    const result = await inventoryPeriodService.openPeriod(data);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    return result;
  }

  async function closePeriod(data: InventoryPeriodCloseRequest) {
    const result = await inventoryPeriodService.closePeriod(data);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    return result;
  }

  async function downloadPeriodsExcel(params?: FilterParams) {
    await base.downloadExcel(params);
  }

  async function downloadPeriodsPdf(params?: FilterParams) {
    await base.downloadPdf(params);
  }

  return {
    // Estado base
    items, 
    selectedItem, 
    totalItems, 
    loading, 
    lastFilterParams,
    list, 
    selected, 
    total,

    // Estado extra
    periodDetail,
    openingItems, 
    totalOpeningItems,
    closingItems, 
    totalClosingItems,
    systemStockItems, 
    totalSystemStockItems,

    // Acciones
    fetchPeriods,
    fetchPeriodDetail,
    fetchOpening,
    fetchClosing,
    fetchSystemStock,
    openPeriod,
    closePeriod,
    downloadPeriodsExcel,
    downloadPeriodsPdf,
  };
});