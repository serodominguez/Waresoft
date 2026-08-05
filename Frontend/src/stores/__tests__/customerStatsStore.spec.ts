import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';

vi.mock('@/services/customerService', () => ({
  customerService: {
    getStats: vi.fn(),
    fetchAll:      vi.fn().mockResolvedValue({ isSuccess: true, data: [], totalRecords: 0 }),
    select:        vi.fn().mockResolvedValue({ isSuccess: true, data: [] }),
    fetchById:     vi.fn().mockResolvedValue({ isSuccess: true, data: null }),
    downloadExcel: vi.fn().mockResolvedValue(undefined),
    downloadPdf:   vi.fn().mockResolvedValue(undefined),
    create:        vi.fn().mockResolvedValue({ isSuccess: true }),
    update:        vi.fn().mockResolvedValue({ isSuccess: true }),
    enable:        vi.fn().mockResolvedValue({ isSuccess: true }),
    disable:       vi.fn().mockResolvedValue({ isSuccess: true }),
    remove:        vi.fn().mockResolvedValue({ isSuccess: true }),
  }
}));

import { customerService } from '@/services/customerService';
import { useCustomerStatsStore } from '../customerStore';

const mockGetStats = vi.mocked(customerService.getStats);

const mockStats = {
  totalActive:       42,
  percentageChange:  5.5,
  isPositive:        true,
};

beforeEach(() => {
  setActivePinia(createPinia());
  vi.clearAllMocks();
});

describe('useCustomerStatsStore', () => {

  it('inicializa con stats null y loading false', () => {
    const store = useCustomerStatsStore();

    expect(store.stats).toBeNull();
    expect(store.loading).toBe(false);
  });

  it('fetchStats carga las estadísticas correctamente', async () => {
    mockGetStats.mockResolvedValue(mockStats);
    const store = useCustomerStatsStore();

    await store.fetchStats();

    expect(store.stats).toEqual(mockStats);
  });

  it('fetchStats setea loading false al terminar', async () => {
    mockGetStats.mockResolvedValue(mockStats);
    const store = useCustomerStatsStore();

    await store.fetchStats();

    expect(store.loading).toBe(false);
  });

  it('fetchStats setea loading false aunque falle', async () => {
    mockGetStats.mockRejectedValue(new Error('Network Error'));
    const store = useCustomerStatsStore();

    await store.fetchStats().catch(() => {});

    expect(store.loading).toBe(false);
  });

  it('fetchStats llama a customerService.getStats', async () => {
    mockGetStats.mockResolvedValue(mockStats);
    const store = useCustomerStatsStore();

    await store.fetchStats();

    expect(mockGetStats).toHaveBeenCalledTimes(1);
  });
});