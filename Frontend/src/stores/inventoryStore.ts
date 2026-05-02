import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Inventory, InventoryPivot } from '@/interfaces/inventoryInterface';
import { inventoryService } from '@/services/inventoryService';
import { FilterParams } from '@/interfaces/baseInterface';
import { useAuthStore } from '@/stores/authStore';

export const useInventoryStore = defineStore('inventory', () => {
  const items = ref<Inventory[]>([]);
  const selectedItem = ref<Inventory | null>(null);
  const totalItems = ref<number>(0);
  const totalPivotItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const lastFilterParams = ref<FilterParams>({});
  const inventoryPivot = ref<InventoryPivot | null>(null);
  const lastPivotFilterParams = ref<FilterParams>({});

  const inventories = computed(() => items.value);
  const selectedInventory = computed(() => selectedItem.value);
  const totalInventories = computed(() => totalItems.value);
  const totalRows = computed(() => totalPivotItems.value);

  async function fetchInventories(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;
    try {
      const result = await inventoryService.fetchAll(params);
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

  async function downloadInventoriesExcel(params?: FilterParams) {
    await inventoryService.downloadExcel(params ?? lastFilterParams.value);
  }

  async function downloadInventoriesPdf(params?: FilterParams) {
    await inventoryService.downloadPdf(params ?? lastFilterParams.value);
  }

  async function downloadInventoryPivotExcel(params?: FilterParams) {
    await inventoryService.downloadPivotExcel(params ?? lastPivotFilterParams.value);
  }

  async function downloadInventoryPivotPdf(params?: FilterParams) {
    await inventoryService.downloadPivotPdf(params ?? lastPivotFilterParams.value);
  }

  async function downloadInventorySheet(params?: FilterParams) {
    const authStore = useAuthStore();
    const storeName = authStore.currentUser?.storeName;
    await inventoryService.inventorySheet(params ?? lastFilterParams.value, storeName);
  }

  async function editInventoryPrice(inventory: Inventory) {
    const result = await inventoryService.updatePrice(inventory);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await fetchInventories(lastFilterParams.value);
    return result;
  }

  return {
    items,
    selectedItem,
    totalItems,
    totalPivotItems,
    totalRows,
    loading,
    lastFilterParams,
    inventories,
    inventoryPivot,
    lastPivotFilterParams,
    selectedInventory,
    totalInventories,

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
