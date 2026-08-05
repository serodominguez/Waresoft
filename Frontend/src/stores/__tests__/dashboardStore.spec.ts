import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';

vi.mock('@/services/dashboardService', () => ({
  dashboardService: {
    getGoodsIssueStats: vi.fn(),
    getInventoryStats: vi.fn(),
    getMovementsStats: vi.fn(),
    getProductReplenishment: vi.fn(),
    getProductStats: vi.fn(),
    getTransfersByStore: vi.fn(),
    getTransferStatus: vi.fn(),
    getTransferPending: vi.fn(),
  }
}));

import { dashboardService } from '@/services/dashboardService';
import { useDashboardStore } from '../dashboardStore';

const mockGoodsIssueStats = { totalIssues: 10, differenceVsLast7Days: 2, isPositive: true };
const mockInventoryStats = { belowMinimum: 5, differenceVsLastMonth: -1, isPositive: false };
const mockMovementsStats = [{ month: 'Ene', receipts: 10, issues: 5 }];
const mockProductReplenishment = { available: 100, notAvailable: 20, discontinued: 5 };
const mockProductStats = { totalActive: 200, newThisMonth: 15 };
const mockTransfersByStore = [{ storeName: 'Tienda A', totalTransfers: 8 }];
const mockTransferStatus = [{ month: 'Ene', sent: 3, pending: 1, received: 4 }];
const mockTransferPending = { totalPending: 3, differenceVsYesterday: 1, isPendingPositive: false };

beforeEach(() => {
  setActivePinia(createPinia());
  vi.clearAllMocks();

  vi.mocked(dashboardService.getGoodsIssueStats).mockResolvedValue(mockGoodsIssueStats);
  vi.mocked(dashboardService.getInventoryStats).mockResolvedValue(mockInventoryStats);
  vi.mocked(dashboardService.getMovementsStats).mockResolvedValue(mockMovementsStats);
  vi.mocked(dashboardService.getProductReplenishment).mockResolvedValue(mockProductReplenishment);
  vi.mocked(dashboardService.getProductStats).mockResolvedValue(mockProductStats);
  vi.mocked(dashboardService.getTransfersByStore).mockResolvedValue(mockTransfersByStore);
  vi.mocked(dashboardService.getTransferStatus).mockResolvedValue(mockTransferStatus);
  vi.mocked(dashboardService.getTransferPending).mockResolvedValue(mockTransferPending);
});

describe('useDashboardStore', () => {

  it('inicializa con todos los valores en null o array vacío', () => {
    const store = useDashboardStore();

    expect(store.goodsIssueStats).toBeNull();
    expect(store.inventoryStats).toBeNull();
    expect(store.movementsStats).toEqual([]);
    expect(store.productReplenishment).toBeNull();
    expect(store.productStats).toBeNull();
    expect(store.transfersByStore).toEqual([]);
    expect(store.transferStatus).toEqual([]);
    expect(store.transferPending).toBeNull();
    expect(store.loading).toBe(false);
  });

  it('fetchAll carga todos los datos correctamente', async () => {
    const store = useDashboardStore();

    await store.fetchAll();

    expect(store.goodsIssueStats).toEqual(mockGoodsIssueStats);
    expect(store.inventoryStats).toEqual(mockInventoryStats);
    expect(store.movementsStats).toEqual(mockMovementsStats);
    expect(store.productReplenishment).toEqual(mockProductReplenishment);
    expect(store.productStats).toEqual(mockProductStats);
    expect(store.transfersByStore).toEqual(mockTransfersByStore);
    expect(store.transferStatus).toEqual(mockTransferStatus);
    expect(store.transferPending).toEqual(mockTransferPending);
  });

  it('fetchAll llama a todos los servicios en paralelo', async () => {
    const store = useDashboardStore();

    await store.fetchAll();

    expect(dashboardService.getGoodsIssueStats).toHaveBeenCalledTimes(1);
    expect(dashboardService.getInventoryStats).toHaveBeenCalledTimes(1);
    expect(dashboardService.getMovementsStats).toHaveBeenCalledTimes(1);
    expect(dashboardService.getProductReplenishment).toHaveBeenCalledTimes(1);
    expect(dashboardService.getProductStats).toHaveBeenCalledTimes(1);
    expect(dashboardService.getTransfersByStore).toHaveBeenCalledTimes(1);
    expect(dashboardService.getTransferStatus).toHaveBeenCalledTimes(1);
    expect(dashboardService.getTransferPending).toHaveBeenCalledTimes(1);
  });

  it('fetchAll setea loading false al terminar', async () => {
    const store = useDashboardStore();

    await store.fetchAll();

    expect(store.loading).toBe(false);
  });

  it('fetchAll setea loading false aunque algún servicio falle', async () => {
    vi.mocked(dashboardService.getGoodsIssueStats).mockRejectedValue(new Error('Network Error'));
    const store = useDashboardStore();

    await store.fetchAll().catch(() => { });

    expect(store.loading).toBe(false);
  });
});