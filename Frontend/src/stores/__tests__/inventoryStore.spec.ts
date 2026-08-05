import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';

vi.mock('@/services/inventoryService', () => ({
  inventoryService: {
    fetchAll: vi.fn().mockResolvedValue({ isSuccess: true, data: [], totalRecords: 0, message: '', errors: null }),
    select: vi.fn().mockResolvedValue({ isSuccess: true, data: [], totalRecords: 0, message: '', errors: null }),
    fetchById: vi.fn().mockResolvedValue({ isSuccess: true, data: null, totalRecords: 0, message: '', errors: null }),
    downloadExcel: vi.fn().mockResolvedValue(undefined),
    downloadPdf: vi.fn().mockResolvedValue(undefined),
    create: vi.fn().mockResolvedValue({ isSuccess: true, data: null, totalRecords: 0, message: '', errors: null }),
    update: vi.fn().mockResolvedValue({ isSuccess: true, data: null, totalRecords: 0, message: '', errors: null }),
    enable: vi.fn().mockResolvedValue({ isSuccess: true, data: null, totalRecords: 0, message: '', errors: null }),
    disable: vi.fn().mockResolvedValue({ isSuccess: true, data: null, totalRecords: 0, message: '', errors: null }),
    remove: vi.fn().mockResolvedValue({ isSuccess: true, data: null, totalRecords: 0, message: '', errors: null }),
    fetchCalculated: vi.fn(),
    fetchPivot: vi.fn(),
    updatePrice: vi.fn(),
    inventorySheet: vi.fn(),
    downloadCalculatedExcel: vi.fn(),
    downloadCalculatedPdf: vi.fn(),
    downloadPivotExcel: vi.fn(),
    downloadPivotPdf: vi.fn(),
  } as any
}));

vi.mock('@/stores/authStore', () => ({
  useAuthStore: vi.fn(() => ({
    currentUser: { storeName: 'Tienda Central' }
  }))
}));

import { inventoryService } from '@/services/inventoryService';
import { useInventoryStore } from '../inventoryStore';

const baseResponse = { totalRecords: 0, message: '', errors: null };

const mockInventory = {
  idStore: 1, idProduct: 1, code: 'P-001', description: 'Producto A',
  material: 'Metal', color: 'Rojo', unitMeasure: 'Unidad',
  stockAvailable: 10, calculatedStock: 10, stockDifference: 0,
  stockInTransit: 0, minimumStock: 5, price: 100,
  replenishment: 'Disponible', brandName: 'Marca A',
  categoryName: 'Cat A', auditCreateDate: '2025-01-01'
};

const mockPivot = {
  stores: ['Tienda A', 'Tienda B'],
  rows: [{
    image: '', code: 'P-001', description: 'Producto A',
    material: 'Metal', color: 'Rojo', brandName: 'Marca A',
    categoryName: 'Cat A', auditCreateDate: '2025-01-01',
    stockByStore: { 'Tienda A': 5, 'Tienda B': 3 }
  }]
};

beforeEach(() => {
  setActivePinia(createPinia());
  vi.clearAllMocks();

  vi.mocked(inventoryService.fetchAll).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: [], totalRecords: 0
  } as any);
  vi.mocked(inventoryService.fetchCalculated).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: [mockInventory], totalRecords: 1
  });
  vi.mocked(inventoryService.fetchPivot).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: mockPivot, totalRecords: 1
  });
  vi.mocked(inventoryService.updatePrice).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: mockInventory
  });
  vi.mocked(inventoryService.inventorySheet).mockResolvedValue(undefined);
  vi.mocked(inventoryService.downloadCalculatedExcel).mockResolvedValue(undefined);
  vi.mocked(inventoryService.downloadCalculatedPdf).mockResolvedValue(undefined);
  vi.mocked(inventoryService.downloadPivotExcel).mockResolvedValue(undefined);
  vi.mocked(inventoryService.downloadPivotPdf).mockResolvedValue(undefined);
});

describe('useInventoryStore', () => {

  // ── Estado inicial ────────────────────────────────────────────────────────
  it('inicializa con estado vacío', () => {
    const store = useInventoryStore();

    expect(store.items).toEqual([]);
    expect(store.inventoryPivot).toBeNull();
    expect(store.totalPivotItems).toBe(0);
    expect(store.loading).toBe(false);
  });

  // ── fetchInventories ──────────────────────────────────────────────────────
  it('fetchInventories carga items y totalItems', async () => {
    const store = useInventoryStore();

    await store.fetchInventories({ pageNumber: 1, pageSize: 10 });

    expect(store.items).toEqual([mockInventory]);
    expect(store.totalItems).toBe(1);
  });

  it('fetchInventories guarda lastFilterParams', async () => {
    const store = useInventoryStore();

    await store.fetchInventories({ pageNumber: 2, pageSize: 25 });

    expect(store.lastFilterParams).toEqual({ pageNumber: 2, pageSize: 25 });
  });

  it('fetchInventories lanza error cuando isSuccess es false', async () => {
    vi.mocked(inventoryService.fetchCalculated).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'Error del servidor', data: []
    });
    const store = useInventoryStore();

    await expect(store.fetchInventories()).rejects.toThrow('Error del servidor');
  });

  it('fetchInventories setea loading false aunque falle', async () => {
    vi.mocked(inventoryService.fetchCalculated).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'Error', data: []
    });
    const store = useInventoryStore();

    await store.fetchInventories().catch(() => { });

    expect(store.loading).toBe(false);
  });

  // ── fetchInventoryPivot ───────────────────────────────────────────────────
  it('fetchInventoryPivot carga inventoryPivot y totalPivotItems', async () => {
    const store = useInventoryStore();

    await store.fetchInventoryPivot({ pageNumber: 1, pageSize: 10 });

    expect(store.inventoryPivot).toEqual(mockPivot);
    expect(store.totalPivotItems).toBe(1);
  });

  it('fetchInventoryPivot guarda lastPivotFilterParams', async () => {
    const store = useInventoryStore();

    await store.fetchInventoryPivot({ pageNumber: 3, pageSize: 50 });

    expect(store.lastPivotFilterParams).toEqual({ pageNumber: 3, pageSize: 50 });
  });

  it('fetchInventoryPivot lanza error cuando isSuccess es false', async () => {
    vi.mocked(inventoryService.fetchPivot).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'Sin datos', data: null as any
    });
    const store = useInventoryStore();

    await expect(store.fetchInventoryPivot()).rejects.toThrow('Sin datos');
  });

  // ── editInventoryPrice ────────────────────────────────────────────────────
  it('editInventoryPrice llama updatePrice y recarga inventories', async () => {
    const store = useInventoryStore();

    await store.editInventoryPrice(mockInventory);

    expect(inventoryService.updatePrice).toHaveBeenCalledWith(mockInventory);
    expect(inventoryService.fetchCalculated).toHaveBeenCalled();
  });

  it('editInventoryPrice lanza error cuando isSuccess es false', async () => {
    vi.mocked(inventoryService.updatePrice).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'No se pudo actualizar', data: null as any
    });
    const store = useInventoryStore();

    await expect(store.editInventoryPrice(mockInventory)).rejects.toThrow('No se pudo actualizar');
  });

  // ── Descargas ─────────────────────────────────────────────────────────────
  it('downloadInventorySheet llama al servicio con storeName del usuario', async () => {
    const store = useInventoryStore();
    await store.fetchInventories({ pageNumber: 1, pageSize: 10 });

    await store.downloadInventorySheet();

    expect(inventoryService.inventorySheet).toHaveBeenCalledWith(
      { pageNumber: 1, pageSize: 10 },
      'Tienda Central'
    );
  });

  it('downloadInventoriesExcel llama al servicio con lastFilterParams', async () => {
    const store = useInventoryStore();
    await store.fetchInventories({ pageNumber: 1, pageSize: 10 });

    await store.downloadInventoriesExcel();

    expect(inventoryService.downloadCalculatedExcel).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  it('downloadInventoriesPdf llama al servicio con lastFilterParams', async () => {
    const store = useInventoryStore();
    await store.fetchInventories({ pageNumber: 1, pageSize: 10 });

    await store.downloadInventoriesPdf();

    expect(inventoryService.downloadCalculatedPdf).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  it('downloadInventoryPivotExcel llama al servicio con lastPivotFilterParams', async () => {
    const store = useInventoryStore();
    await store.fetchInventoryPivot({ pageNumber: 1, pageSize: 10 });

    await store.downloadInventoryPivotExcel();

    expect(inventoryService.downloadPivotExcel).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  it('downloadInventoryPivotPdf llama al servicio con lastPivotFilterParams', async () => {
    const store = useInventoryStore();
    await store.fetchInventoryPivot({ pageNumber: 1, pageSize: 10 });

    await store.downloadInventoryPivotPdf();

    expect(inventoryService.downloadPivotPdf).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  // ── Computed ──────────────────────────────────────────────────────────────
  it('totalRows computed refleja totalPivotItems', async () => {
    const store = useInventoryStore();

    await store.fetchInventoryPivot();

    expect(store.totalRows).toBe(1);
  });
});