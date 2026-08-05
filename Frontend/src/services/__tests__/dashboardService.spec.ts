import { describe, it, expect, vi, beforeEach } from 'vitest';
import axios from 'axios';
import { dashboardService } from '../dashboardService';
import type {
  DashboardGoodsIssueStats,
  DashboardInventoryStats,
  DashboardMovementStats,
  DashboardProductReplenishment,
  DashboardProductStats,
  DashboardTransferByStore,
  DashboardTransferPending,
  DashboardTransferStatus,
} from '@/interfaces/dashboardInterface';

// ─── Mock de axios ────────────────────────────────────────────────────────────

vi.mock('axios');
const mockedAxios = vi.mocked(axios);

beforeEach(() => {
  vi.clearAllMocks();
});

// ─── Helper ───────────────────────────────────────────────────────────────────

const wrapData = <T>(data: T) => ({ data: { data } });

// ─── Fixtures ─────────────────────────────────────────────────────────────────

const goodsIssueStats: DashboardGoodsIssueStats = {
  totalIssues:           20,
  differenceVsLast7Days: 5,
  isPositive:            true,
};

const inventoryStats: DashboardInventoryStats = {
  belowMinimum:           3,
  differenceVsLastMonth:  1,
  isPositive:             false,
};

const movementStats: DashboardMovementStats[] = [
  { month: 'Enero', receipts: 10, issues: 5 },
  { month: 'Febrero', receipts: 8,  issues: 3 },
];

const productReplenishment: DashboardProductReplenishment = {
  available:    50,
  notAvailable: 10,
  discontinued: 2,
};

const productStats: DashboardProductStats = {
  totalActive:  100,
  newThisMonth: 5,
};

const transfersByStore: DashboardTransferByStore[] = [
  { storeName: 'Sucursal A', totalTransfers: 15 },
  { storeName: 'Sucursal B', totalTransfers: 8  },
];

const transferPending: DashboardTransferPending = {
  totalPending:          4,
  differenceVsYesterday: 2,
  isPendingPositive:     false,
};

const transferStatus: DashboardTransferStatus[] = [
  { month: 'Enero', sent: 10, pending: 2, received: 8 },
];

// ─── Tests ────────────────────────────────────────────────────────────────────

describe('DashboardService.getGoodsIssueStats', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(goodsIssueStats));

    await dashboardService.getGoodsIssueStats();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Dashboard/GoodsIssueStats');
  });

  it('retorna los datos correctamente', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(goodsIssueStats));

    const result = await dashboardService.getGoodsIssueStats();

    expect(result).toEqual(goodsIssueStats);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(dashboardService.getGoodsIssueStats()).rejects.toThrow('Network Error');
  });
});

describe('DashboardService.getInventoryStats', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(inventoryStats));

    await dashboardService.getInventoryStats();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Dashboard/InventoryStats');
  });

  it('retorna los datos correctamente', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(inventoryStats));

    const result = await dashboardService.getInventoryStats();

    expect(result).toEqual(inventoryStats);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(dashboardService.getInventoryStats()).rejects.toThrow('Network Error');
  });
});

describe('DashboardService.getMovementsStats', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(movementStats));

    await dashboardService.getMovementsStats();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Dashboard/MovementsStats');
  });

  it('retorna el array correctamente', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(movementStats));

    const result = await dashboardService.getMovementsStats();

    expect(result).toHaveLength(2);
    expect(result[0].month).toBe('Enero');
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(dashboardService.getMovementsStats()).rejects.toThrow('Network Error');
  });
});

describe('DashboardService.getProductReplenishment', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(productReplenishment));

    await dashboardService.getProductReplenishment();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Dashboard/ProductReplenishment');
  });

  it('retorna los datos correctamente', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(productReplenishment));

    const result = await dashboardService.getProductReplenishment();

    expect(result).toEqual(productReplenishment);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(dashboardService.getProductReplenishment()).rejects.toThrow('Network Error');
  });
});

describe('DashboardService.getProductStats', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(productStats));

    await dashboardService.getProductStats();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Dashboard/ProductStats');
  });

  it('retorna los datos correctamente', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(productStats));

    const result = await dashboardService.getProductStats();

    expect(result).toEqual(productStats);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(dashboardService.getProductStats()).rejects.toThrow('Network Error');
  });
});

describe('DashboardService.getTransfersByStore', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(transfersByStore));

    await dashboardService.getTransfersByStore();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Dashboard/TransfersByStore');
  });

  it('retorna el array correctamente', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(transfersByStore));

    const result = await dashboardService.getTransfersByStore();

    expect(result).toHaveLength(2);
    expect(result[0].storeName).toBe('Sucursal A');
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(dashboardService.getTransfersByStore()).rejects.toThrow('Network Error');
  });
});

describe('DashboardService.getTransferPending', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(transferPending));

    await dashboardService.getTransferPending();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Dashboard/TransferPending');
  });

  it('retorna los datos correctamente', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(transferPending));

    const result = await dashboardService.getTransferPending();

    expect(result).toEqual(transferPending);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(dashboardService.getTransferPending()).rejects.toThrow('Network Error');
  });
});

describe('DashboardService.getTransferStatus', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(transferStatus));

    await dashboardService.getTransferStatus();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Dashboard/TransferStatus');
  });

  it('retorna el array correctamente', async () => {
    mockedAxios.get.mockResolvedValueOnce(wrapData(transferStatus));

    const result = await dashboardService.getTransferStatus();

    expect(result).toHaveLength(1);
    expect(result[0].month).toBe('Enero');
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(dashboardService.getTransferStatus()).rejects.toThrow('Network Error');
  });
});