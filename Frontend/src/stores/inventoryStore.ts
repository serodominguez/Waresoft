import { defineStore, storeToRefs } from 'pinia';
import { ref, computed } from 'vue';
import { Inventory, InventoryPivot } from '@/interfaces/inventoryInterface';
import { inventoryService } from '@/services/inventoryService';
import { FilterParams } from '@/interfaces/baseInterface';
import { useAuthStore } from '@/stores/authStore';
import { createBaseStore } from '@/stores/baseStore';

const useBaseInventoryStore = createBaseStore<Inventory>('inventory-base', inventoryService);

export const useInventoryStore = defineStore('inventory', () => {
  const base = useBaseInventoryStore();

  const {
    items, selectedItem, totalItems, loading, lastFilterParams,
    list, selected, total
  } = storeToRefs(base);

  // Estado extra
  const totalPivotItems = ref<number>(0);
  const inventoryPivot = ref<InventoryPivot | null>(null);
  const lastPivotFilterParams = ref<FilterParams>({});
  const totalRows = computed(() => totalPivotItems.value);

  // Computed para mantener compatibilidad con InventoryView
  const inventories = computed(() => items.value);
  const totalInventories = computed(() => totalItems.value);

  // Vista principal → /Calculated
  async function fetchInventories(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;
    try {
      const result = await inventoryService.fetchCalculated(params);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      items.value = result.data;
      totalItems.value = result.totalRecords;
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
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      inventoryPivot.value = result.data;
      totalPivotItems.value = result.totalRecords;
    } finally {
      loading.value = false;
    }
  }

  async function editInventoryPrice(inventory: Inventory) {
    const result = await inventoryService.updatePrice(inventory);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await fetchInventories(lastFilterParams.value);
    return result;
  }

  async function downloadInventorySheet(params?: FilterParams) {
    const authStore = useAuthStore();
    const storeName = authStore.currentUser?.storeName;
    await inventoryService.inventorySheet(params ?? lastFilterParams.value, storeName);
  }

  async function downloadInventoriesExcel(params?: FilterParams) {
    await inventoryService.downloadCalculatedExcel(params ?? lastFilterParams.value);
  }

  async function downloadInventoriesPdf(params?: FilterParams) {
    await inventoryService.downloadCalculatedPdf(params ?? lastFilterParams.value);
  }

  async function downloadInventoryPivotExcel(params?: FilterParams) {
    await inventoryService.downloadPivotExcel(params ?? lastPivotFilterParams.value);
  }

  async function downloadInventoryPivotPdf(params?: FilterParams) {
    await inventoryService.downloadPivotPdf(params ?? lastPivotFilterParams.value);
  }

  return {
    // Estado base
    items, selectedItem, totalItems, loading, lastFilterParams,
    list, selected, total,

    // Estado extra pivot
    totalPivotItems, totalRows, inventoryPivot, lastPivotFilterParams,

    // Vista principal → /Calculated
    fetchInventories,
    downloadInventoriesExcel,
    downloadInventoriesPdf,

    // Modal → /StoreInventory base
    fetchAll: base.fetchAll,

    // Pivot
    fetchInventoryPivot,
    downloadInventoryPivotExcel,
    downloadInventoryPivotPdf,

    // Otros
    downloadInventorySheet,
    editInventoryPrice,
  };
});