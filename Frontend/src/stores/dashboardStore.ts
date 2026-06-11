import { defineStore } from 'pinia';
import { ref } from 'vue';
import { DashboardGoodsIssueStats, 
  DashboardInventoryStats,
  DashboardMovementStats,
  DashboardProductReplenishment, 
  DashboardProductStats,
  DashboardTransferByStore, 
  DashboardTransferPending,
  DashboardTransferStatus } from '@/interfaces/dashboardInterface';
import { dashboardService } from '@/services/dashboardService';

export const useDashboardStore = defineStore('dashboard', () => {
  const goodsIssueStats = ref<DashboardGoodsIssueStats | null>(null);
  const inventoryStats = ref<DashboardInventoryStats | null>(null);
  const movementsStats = ref<DashboardMovementStats[]>([]);
  const productReplenishment = ref<DashboardProductReplenishment | null>(null);
  const productStats = ref<DashboardProductStats | null>(null);
  const transfersByStore = ref<DashboardTransferByStore[]>([]);
  const transferStatus = ref<DashboardTransferStatus[]>([]);
  const transferPending = ref<DashboardTransferPending | null>(null);

  const loading = ref<boolean>(false);

  async function fetchAll() {
    loading.value = true;
    try {
      const [ goodsIssueData, inventoryData, movementsData, replenishmentData, productsData, transfersByStoreData, transferStatusData, transferPendingData] = await Promise.all([
        dashboardService.getGoodsIssueStats(),
        dashboardService.getInventoryStats(),
        dashboardService.getMovementsStats(),
        dashboardService.getProductReplenishment(),
        dashboardService.getProductStats(),
        dashboardService.getTransfersByStore(),
        dashboardService.getTransferStatus(),
        dashboardService.getTransferPending(),
      ]);

      goodsIssueStats.value = goodsIssueData;
      inventoryStats.value = inventoryData;
      movementsStats.value = movementsData;
      productReplenishment.value = replenishmentData;
      productStats.value = productsData;
      transfersByStore.value = transfersByStoreData;
      transferStatus.value = transferStatusData;
      transferPending.value = transferPendingData;
    } finally {
      loading.value = false;
    }
  }

  return {
    goodsIssueStats,
    inventoryStats,
    movementsStats,
    productReplenishment,
    productStats,
    transfersByStore,
    transferStatus,
    transferPending,
    loading,
    fetchAll,
  };
});