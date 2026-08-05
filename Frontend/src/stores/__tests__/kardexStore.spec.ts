import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';

vi.mock('@/services/kardexService', () => ({
  kardexService: {
    fetchKardex: vi.fn(),
    downloadKardexExcel: vi.fn(),
    downloadKardexPdf: vi.fn(),
  }
}));

import { kardexService } from '@/services/kardexService';
import { useKardexStore } from '../kardexStore';

const baseResponse = { totalRecords: 0, message: '', errors: null };

const mockMovement = {
  idProduct: 1, quantity: 5, idMovement: 1, code: 'SA-001',
  date: '2025-01-01', movementType: 'Entrada' as const,
  type: 'Compra', state: 'Completado', accumulatedStock: 15
};

const mockKardex = {
  idProduct: 1, code: 'P-001', description: 'Producto A',
  material: 'Metal', color: 'Rojo', unitMeasure: 'Unidad',
  currentStock: 10, calculatedStock: 10, stockDifference: 0,
  movements: [mockMovement]
};

beforeEach(() => {
  setActivePinia(createPinia());
  vi.clearAllMocks();

  vi.mocked(kardexService.fetchKardex).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: mockKardex, totalRecords: 1
  });
  vi.mocked(kardexService.downloadKardexExcel).mockResolvedValue(undefined);
  vi.mocked(kardexService.downloadKardexPdf).mockResolvedValue(undefined);
});

describe('useKardexStore', () => {

  // ── Estado inicial ────────────────────────────────────────────────────────
  it('inicializa con estado vacío', () => {
    const store = useKardexStore();

    expect(store.kardexDetail).toBeNull();
    expect(store.totalItems).toBe(0);
    expect(store.loading).toBe(false);
    expect(store.lastProductId).toBeNull();
  });

  // ── fetchKardex ───────────────────────────────────────────────────────────
  it('fetchKardex carga kardexDetail y totalItems', async () => {
    const store = useKardexStore();

    await store.fetchKardex(1, { pageNumber: 1, pageSize: 10 });

    expect(store.kardexDetail).toEqual(mockKardex);
    expect(store.totalItems).toBe(1);
  });

  it('fetchKardex guarda lastProductId y lastFilterParams', async () => {
    const store = useKardexStore();

    await store.fetchKardex(5, { pageNumber: 2, pageSize: 25 });

    expect(store.lastProductId).toBe(5);
    expect(store.lastFilterParams).toEqual({ pageNumber: 2, pageSize: 25 });
  });

  it('fetchKardex llama al servicio con productId y params', async () => {
    const store = useKardexStore();

    await store.fetchKardex(1, { pageNumber: 1, pageSize: 10 });

    expect(kardexService.fetchKardex).toHaveBeenCalledWith(1, { pageNumber: 1, pageSize: 10 }, undefined);
  });

  it('fetchKardex lanza error cuando isSuccess es false', async () => {
    vi.mocked(kardexService.fetchKardex).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'No encontrado', data: null as any
    });
    const store = useKardexStore();

    await expect(store.fetchKardex(99)).rejects.toThrow('No encontrado');
  });

  it('fetchKardex setea loading false aunque falle', async () => {
    vi.mocked(kardexService.fetchKardex).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'Error', data: null as any
    });
    const store = useKardexStore();

    await store.fetchKardex(1).catch(() => { });

    expect(store.loading).toBe(false);
  });

  it('fetchKardex ignora AbortError sin lanzar excepción', async () => {
    const abortError = new Error('Aborted');
    abortError.name = 'AbortError';
    vi.mocked(kardexService.fetchKardex).mockRejectedValue(abortError);
    const store = useKardexStore();

    await expect(store.fetchKardex(1)).resolves.toBeUndefined();
  });

  it('fetchKardex ignora ERR_CANCELED sin lanzar excepción', async () => {
    const cancelError = new Error('Canceled') as any;
    cancelError.code = 'ERR_CANCELED';
    vi.mocked(kardexService.fetchKardex).mockRejectedValue(cancelError);
    const store = useKardexStore();

    await expect(store.fetchKardex(1)).resolves.toBeUndefined();
  });

  // ── Descargas ─────────────────────────────────────────────────────────────
  it('downloadKardexExcel llama al servicio con lastProductId y lastFilterParams', async () => {
    const store = useKardexStore();
    await store.fetchKardex(1, { pageNumber: 1, pageSize: 10 });

    await store.downloadKardexExcel();

    expect(kardexService.downloadKardexExcel).toHaveBeenCalledWith(1, { pageNumber: 1, pageSize: 10 });
  });

  it('downloadKardexExcel no llama al servicio si lastProductId es null', async () => {
    const store = useKardexStore();

    await store.downloadKardexExcel();

    expect(kardexService.downloadKardexExcel).not.toHaveBeenCalled();
  });

  it('downloadKardexPdf llama al servicio con lastProductId y lastFilterParams', async () => {
    const store = useKardexStore();
    await store.fetchKardex(1, { pageNumber: 1, pageSize: 10 });

    await store.downloadKardexPdf();

    expect(kardexService.downloadKardexPdf).toHaveBeenCalledWith(1, { pageNumber: 1, pageSize: 10 });
  });

  it('downloadKardexPdf no llama al servicio si lastProductId es null', async () => {
    const store = useKardexStore();

    await store.downloadKardexPdf();

    expect(kardexService.downloadKardexPdf).not.toHaveBeenCalled();
  });

  // ── clearKardex ───────────────────────────────────────────────────────────
  it('clearKardex resetea todo el estado', async () => {
    const store = useKardexStore();
    await store.fetchKardex(1);

    store.clearKardex();

    expect(store.kardexDetail).toBeNull();
    expect(store.totalItems).toBe(0);
    expect(store.lastProductId).toBeNull();
    expect(store.lastFilterParams).toEqual({});
  });

  // ── Computed ──────────────────────────────────────────────────────────────
  it('kardex computed refleja kardexDetail', async () => {
    const store = useKardexStore();

    await store.fetchKardex(1);

    expect(store.kardex).toEqual(mockKardex);
  });

  it('totalKardex computed refleja totalItems', async () => {
    const store = useKardexStore();

    await store.fetchKardex(1);

    expect(store.totalKardex).toBe(1);
  });
});