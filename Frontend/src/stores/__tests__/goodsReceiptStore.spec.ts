import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import type { BaseResponse } from '@/interfaces/baseInterface';

vi.mock('@/services/goodsReceiptService', () => ({
  goodsReceiptService: {
    fetchAll: vi.fn(),
    getReceiptWithDetails: vi.fn(),
    downloadExcel: vi.fn(),
    downloadPdf: vi.fn(),
    exportPdf: vi.fn(),
    register: vi.fn(),
    disable: vi.fn(),
  }
}));

import { goodsReceiptService } from '@/services/goodsReceiptService';
import { useGoodsReceiptStore } from '../goodsReceiptStore';

// ── Helper ────────────────────────────────────────────────────────────────────
function mockResponse<T>(overrides: Partial<BaseResponse<T>> & { isSuccess: boolean }): BaseResponse<T> {
  return {
    data: undefined as any,
    totalRecords: 0,
    message: '',
    errors: null,
    ...overrides
  };
}

// ── Datos de prueba ───────────────────────────────────────────────────────────
const mockReceipt = {
  idReceipt: 1, code: 'EN-001', type: 'Entrada', storeName: 'Tienda A',
  idUser: 1, userName: 'sergio', totalAmount: 800,
  annotations: '', auditCreateDate: '2025-01-01', statusReceipt: 'Completado'
};

const mockDetail = {
  idProduct: 1, code: 'P-001', description: 'Producto A',
  material: 'Metal', color: 'Rojo', categoryName: 'Cat A',
  brandName: 'Marca A', quantity: 3, unitCost: 100, totalCost: 300
};

const mockReceiptWithDetails = { ...mockReceipt, goodsReceiptDetails: [mockDetail] };

// ── Setup ─────────────────────────────────────────────────────────────────────
beforeEach(() => {
  setActivePinia(createPinia());
  vi.clearAllMocks();

  vi.mocked(goodsReceiptService.fetchAll).mockResolvedValue(
    mockResponse({ isSuccess: true, data: [mockReceipt], totalRecords: 1 }) as any
  );
  vi.mocked(goodsReceiptService.getReceiptWithDetails).mockResolvedValue(
    mockResponse({ isSuccess: true, data: mockReceiptWithDetails })
  );
  vi.mocked(goodsReceiptService.downloadExcel).mockResolvedValue(undefined as any);
  vi.mocked(goodsReceiptService.downloadPdf).mockResolvedValue(undefined as any);
  vi.mocked(goodsReceiptService.exportPdf).mockResolvedValue({
    blob: new Blob(['pdf'], { type: 'application/pdf' }),
    filename: 'entrada-001.pdf'
  });
  vi.mocked(goodsReceiptService.register).mockResolvedValue(
    mockResponse({ isSuccess: true })
  );
  vi.mocked(goodsReceiptService.disable).mockResolvedValue(
    mockResponse({ isSuccess: true })
  );
});

describe('useGoodsReceiptStore', () => {

  // ── Estado inicial ────────────────────────────────────────────────────────
  it('inicializa con estado vacío', () => {
    const store = useGoodsReceiptStore();

    expect(store.items).toEqual([]);
    expect(store.selectedItem).toBeNull();
    expect(store.selectedReceiptDetails).toEqual([]);
    expect(store.totalItems).toBe(0);
    expect(store.loading).toBe(false);
  });

  // ── fetchGoodsReceipt ─────────────────────────────────────────────────────
  it('fetchGoodsReceipt carga items y totalItems', async () => {
    const store = useGoodsReceiptStore();

    await store.fetchGoodsReceipt({ pageNumber: 1, pageSize: 10 });

    expect(store.items).toEqual([mockReceipt]);
    expect(store.totalItems).toBe(1);
  });

  it('fetchGoodsReceipt guarda lastFilterParams', async () => {
    const store = useGoodsReceiptStore();

    await store.fetchGoodsReceipt({ pageNumber: 2, pageSize: 25 });

    expect(store.lastFilterParams).toEqual({ pageNumber: 2, pageSize: 25 });
  });

  it('fetchGoodsReceipt lanza error cuando isSuccess es false', async () => {
    vi.mocked(goodsReceiptService.fetchAll).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'Error del servidor' }) as any
    );
    const store = useGoodsReceiptStore();

    await expect(store.fetchGoodsReceipt()).rejects.toThrow('Error del servidor');
  });

  it('fetchGoodsReceipt setea loading false aunque falle', async () => {
    vi.mocked(goodsReceiptService.fetchAll).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'Error' }) as any
    );
    const store = useGoodsReceiptStore();

    await store.fetchGoodsReceipt().catch(() => { });

    expect(store.loading).toBe(false);
  });

  // ── fetchGoodsReceiptById ─────────────────────────────────────────────────
  it('fetchGoodsReceiptById carga selectedItem y selectedReceiptDetails', async () => {
    const store = useGoodsReceiptStore();

    await store.fetchGoodsReceiptById(1);

    expect(store.selectedItem).toEqual(mockReceiptWithDetails);
    expect(store.selectedReceiptDetails).toHaveLength(1);
    expect(store.selectedReceiptDetails[0].idProduct).toBe(1);
    expect(store.selectedReceiptDetails[0].quantity).toBe(3);
  });

  it('fetchGoodsReceiptById mapea los detalles correctamente', async () => {
    const store = useGoodsReceiptStore();

    await store.fetchGoodsReceiptById(1);

    const detail = store.selectedReceiptDetails[0];
    expect(detail.code).toBe('P-001');
    expect(detail.description).toBe('Producto A');
    expect(detail.totalCost).toBe(300);
  });

  it('fetchGoodsReceiptById retorna array vacío si no hay detalles', async () => {
    vi.mocked(goodsReceiptService.getReceiptWithDetails).mockResolvedValue(
      mockResponse({ isSuccess: true, data: { ...mockReceiptWithDetails, goodsReceiptDetails: null } })
    );
    const store = useGoodsReceiptStore();

    await store.fetchGoodsReceiptById(1);

    expect(store.selectedReceiptDetails).toEqual([]);
  });

  it('fetchGoodsReceiptById lanza error cuando isSuccess es false', async () => {
    vi.mocked(goodsReceiptService.getReceiptWithDetails).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'No encontrado' })
    );
    const store = useGoodsReceiptStore();

    await expect(store.fetchGoodsReceiptById(99)).rejects.toThrow('No encontrado');
  });

  // ── Descargas ─────────────────────────────────────────────────────────────
  it('downloadGoodsReceiptExcel llama al servicio con lastFilterParams', async () => {
    const store = useGoodsReceiptStore();
    await store.fetchGoodsReceipt({ pageNumber: 1, pageSize: 10 });

    await store.downloadGoodsReceiptExcel();

    expect(goodsReceiptService.downloadExcel).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  it('downloadGoodsReceiptPdf llama al servicio con lastFilterParams', async () => {
    const store = useGoodsReceiptStore();
    await store.fetchGoodsReceipt({ pageNumber: 1, pageSize: 10 });

    await store.downloadGoodsReceiptPdf();

    expect(goodsReceiptService.downloadPdf).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  it('getBlobGoodsReceiptPdf retorna un Blob', async () => {
    const store = useGoodsReceiptStore();

    const blob = await store.getBlobGoodsReceiptPdf(1);

    expect(blob).toBeInstanceOf(Blob);
  });

  // ── registerGoodsReceipt ──────────────────────────────────────────────────
  it('registerGoodsReceipt llama al servicio y recarga la lista', async () => {
    const store = useGoodsReceiptStore();
    const receiptData = {
      type: 'Entrada', totalAmount: 300, annotations: '',
      idUser: 1, idStore: 1, goodsReceiptDetails: []
    };

    await store.registerGoodsReceipt(receiptData as any);

    expect(goodsReceiptService.register).toHaveBeenCalledWith(receiptData);
    expect(goodsReceiptService.fetchAll).toHaveBeenCalled();
  });

  it('registerGoodsReceipt lanza error cuando isSuccess es false', async () => {
    vi.mocked(goodsReceiptService.register).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'Stock insuficiente' })
    );
    const store = useGoodsReceiptStore();

    await expect(store.registerGoodsReceipt({} as any)).rejects.toThrow('Stock insuficiente');
  });

  // ── cancel (disableGoodsReceipt) ──────────────────────────────────────────
  it('cancel llama a disable y recarga la lista', async () => {
    const store = useGoodsReceiptStore();

    await store.cancel(1);

    expect(goodsReceiptService.disable).toHaveBeenCalledWith(1);
    expect(goodsReceiptService.fetchAll).toHaveBeenCalled();
  });

  it('cancel lanza error cuando isSuccess es false', async () => {
    vi.mocked(goodsReceiptService.disable).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'No se puede cancelar' })
    );
    const store = useGoodsReceiptStore();

    await expect(store.cancel(1)).rejects.toThrow('No se puede cancelar');
  });

  // ── Computed ──────────────────────────────────────────────────────────────
  it('goodsreceipt computed refleja items', async () => {
    const store = useGoodsReceiptStore();

    await store.fetchGoodsReceipt();

    expect(store.goodsreceipt).toEqual([mockReceipt]);
  });

  it('totalGoodsReceipt computed refleja totalItems', async () => {
    const store = useGoodsReceiptStore();

    await store.fetchGoodsReceipt();

    expect(store.totalGoodsReceipt).toBe(1);
  });
});