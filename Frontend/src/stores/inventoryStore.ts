import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Inventory, InventoryPivot } from '@/interfaces/inventoryInterface';
import { inventoryService } from '@/services/inventoryService';
import { FilterParams } from '@/interfaces/baseInterface';
import { useAuthStore } from '@/stores/auth';

export const useInventoryStore = defineStore('inventory', () => {
  // Estado
  const items = ref<Inventory[]>([]);
  const selectedItem = ref<Inventory | null>(null);
  const totalItems = ref<number>(0);
  const totalPivotItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);
  const inventoryPivot = ref<InventoryPivot | null>(null);
  const lastPivotFilterParams = ref<FilterParams | undefined>(undefined);

  // Getters
  const inventories = computed(() => items.value);
  const selectedInventory = computed(() => selectedItem.value);
  const totalInventories = computed(() => totalItems.value || 0);
  const totalRows = computed(() => totalPivotItems.value || 0);

  // Acciones
  async function fetchInventories(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const result = await inventoryService.fetchAll(params);
      if (result.isSuccess) {
        items.value = result.data;
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

  async function fetchInventoryPivot(params: FilterParams = {}) {
    loading.value = true;
    inventoryPivot.value = null;
    lastPivotFilterParams.value = params;

    try {
      const result = await inventoryService.fetchPivot(params);
      if (result.isSuccess) {
        inventoryPivot.value = result.data;
        totalPivotItems.value = result.totalRecords;
      } else {
        error.value = result.message;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function downloadInventoriesExcel(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await inventoryService.downloadExcel(filterParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadInventoriesPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await inventoryService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function downloadInventoryPivotExcel(params?: FilterParams) {
    try {
      const filterParams = params || lastPivotFilterParams.value || {};
      await inventoryService.downloadPivotExcel(filterParams);
    } catch (err: any) {
      console.error('Error al descargar Excel Pivot:', err);
      throw err;
    }
  }

  async function downloadInventoryPivotPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastPivotFilterParams.value || {};
      await inventoryService.downloadPivotPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF Pivot:', err);
      throw err;
    }
  }

  async function downloadInventorySheet(params?: FilterParams) {
    try {
      const authStore = useAuthStore();
      const storeName = authStore.currentUser?.storeName;
      const filterParams = params || lastFilterParams.value || {};
      await inventoryService.inventorySheet(filterParams, storeName);
    } catch (err: any) {
      console.error('Error al descargar planilla de inventario:', err);
      throw err;
    }
  }

  async function editInventoryPrice(inventory: Inventory) {
    try {
      const result = await inventoryService.updatePrice(inventory);
      if (result.isSuccess) {
        await fetchInventories(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  return {
    // State
    items,
    selectedItem,
    totalItems,
    totalPivotItems,
    totalRows,
    loading,
    error,
    lastFilterParams,

    // Getters
    inventories,
    inventoryPivot,
    lastPivotFilterParams,
    selectedInventory,
    totalInventories,

    // Actions
    fetchInventories,
    fetchInventoryPivot,
    downloadInventoriesExcel,
    downloadInventoryPivotExcel,
    downloadInventoryPivotPdf,
    downloadInventoriesPdf,
    downloadInventorySheet,
    editInventoryPrice,
  };
});