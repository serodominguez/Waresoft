import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';

vi.mock('@/services/productService', () => ({
  productService: {
    fetchAll: vi.fn().mockResolvedValue({ isSuccess: true, data: [], totalRecords: 0, message: '', errors: null }),
    select: vi.fn().mockResolvedValue({ isSuccess: true, data: [], totalRecords: 0, message: '', errors: null }),
    fetchById: vi.fn().mockResolvedValue({ isSuccess: true, data: null, totalRecords: 0, message: '', errors: null }),
    downloadExcel: vi.fn().mockResolvedValue(undefined),
    downloadPdf: vi.fn().mockResolvedValue(undefined),
    enable: vi.fn().mockResolvedValue({ isSuccess: true, data: null, totalRecords: 0, message: '', errors: null }),
    disable: vi.fn().mockResolvedValue({ isSuccess: true, data: null, totalRecords: 0, message: '', errors: null }),
    remove: vi.fn().mockResolvedValue({ isSuccess: true, data: null, totalRecords: 0, message: '', errors: null }),
    registerProduct: vi.fn(),
    editProduct: vi.fn(),
    generateProductCode: vi.fn(),
    generateBarcodePdf: vi.fn(),
    getStats: vi.fn(),
  } as any
}));

import { productService } from '@/services/productService';
import { useProductStore, useProductStatsStore } from '../productStore';

const baseResponse = { totalRecords: 0, message: '', errors: null };

const mockProduct = {
  idProduct: 1, code: 'P-001', description: 'Producto A',
  material: 'Metal', color: 'Rojo', unitMeasure: 'Unidad',
  image: '', idBrand: 1, brandName: 'Marca A',
  idCategory: 1, categoryName: 'Cat A',
  auditCreateDate: '2025-01-01', statusProduct: 'Activo'
};

const mockStats = { totalActive: 100, newThisMonth: 5 };

beforeEach(() => {
  setActivePinia(createPinia());
  vi.clearAllMocks();

  vi.mocked(productService.fetchAll).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: [mockProduct], totalRecords: 1
  } as any);
  vi.mocked(productService.registerProduct).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: mockProduct
  });
  vi.mocked(productService.editProduct).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: mockProduct
  });
  vi.mocked(productService.generateProductCode).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: 'P-002'
  });
  vi.mocked(productService.generateBarcodePdf).mockResolvedValue(
    new Blob(['pdf'], { type: 'application/pdf' })
  );
  vi.mocked(productService.getStats).mockResolvedValue(mockStats);
});

describe('useProductStore', () => {

  // ── Estado inicial ────────────────────────────────────────────────────────
  it('inicializa con estado vacío', () => {
    const store = useProductStore();

    expect(store.items).toEqual([]);
    expect(store.selectedItem).toBeNull();
    expect(store.totalItems).toBe(0);
    expect(store.loading).toBe(false);
  });

  // ── registerProduct ───────────────────────────────────────────────────────
  it('registerProduct llama al servicio y recarga la lista', async () => {
    const store = useProductStore();
    const formData = new FormData();

    await store.registerProduct(formData);

    expect(productService.registerProduct).toHaveBeenCalledWith(formData);
    expect(productService.fetchAll).toHaveBeenCalled();
  });

  it('registerProduct retorna el resultado', async () => {
    const store = useProductStore();

    const result = await store.registerProduct(new FormData());

    expect(result.isSuccess).toBe(true);
  });

  it('registerProduct lanza error cuando isSuccess es false', async () => {
    vi.mocked(productService.registerProduct).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'Error al registrar', data: null as any
    });
    const store = useProductStore();

    await expect(store.registerProduct(new FormData())).rejects.toThrow('Error al registrar');
  });

  // ── editProduct ───────────────────────────────────────────────────────────
  it('editProduct llama al servicio con id y formData', async () => {
    const store = useProductStore();
    const formData = new FormData();

    await store.editProduct(1, formData);

    expect(productService.editProduct).toHaveBeenCalledWith(1, formData);
    expect(productService.fetchAll).toHaveBeenCalled();
  });

  it('editProduct lanza error cuando isSuccess es false', async () => {
    vi.mocked(productService.editProduct).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'Error al editar', data: null as any
    });
    const store = useProductStore();

    await expect(store.editProduct(1, new FormData())).rejects.toThrow('Error al editar');
  });

  // ── generateProductCode ───────────────────────────────────────────────────
  it('generateProductCode retorna el código generado', async () => {
    const store = useProductStore();

    const result = await store.generateProductCode();

    expect(result.data).toBe('P-002');
  });

  it('generateProductCode lanza error cuando isSuccess es false', async () => {
    vi.mocked(productService.generateProductCode).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'No se pudo generar', data: null as any
    });
    const store = useProductStore();

    await expect(store.generateProductCode()).rejects.toThrow('No se pudo generar');
  });

  // ── generateBarcodePdf ────────────────────────────────────────────────────
  it('generateBarcodePdf llama al servicio con productId y quantity', async () => {
    const store = useProductStore();
    const mockOpen = vi.spyOn(window, 'open').mockImplementation(() => null);

    await store.generateBarcodePdf(1, 5);

    expect(productService.generateBarcodePdf).toHaveBeenCalledWith(1, 5);
    mockOpen.mockRestore();
  });
});

describe('useProductStatsStore', () => {

  it('inicializa con stats null y loading false', () => {
    const store = useProductStatsStore();

    expect(store.stats).toBeNull();
    expect(store.loading).toBe(false);
  });

  it('fetchStats carga las estadísticas', async () => {
    const store = useProductStatsStore();

    await store.fetchStats();

    expect(store.stats).toEqual(mockStats);
  });

  it('fetchStats setea loading false al terminar', async () => {
    const store = useProductStatsStore();

    await store.fetchStats();

    expect(store.loading).toBe(false);
  });

  it('fetchStats setea loading false aunque falle', async () => {
    vi.mocked(productService.getStats).mockRejectedValue(new Error('Network Error'));
    const store = useProductStatsStore();

    await store.fetchStats().catch(() => { });

    expect(store.loading).toBe(false);
  });

  it('fetchStats llama a productService.getStats', async () => {
    const store = useProductStatsStore();

    await store.fetchStats();

    expect(productService.getStats).toHaveBeenCalledTimes(1);
  });
});