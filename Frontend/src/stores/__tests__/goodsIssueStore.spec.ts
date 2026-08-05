import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import type { BaseResponse, FilterParams } from '@/interfaces/baseInterface';

vi.mock('@/services/goodsIssueService', () => ({
  goodsIssueService: {
    fetchAll: vi.fn(),
    getIssueWithDetails: vi.fn(),
    downloadExcel: vi.fn(),
    downloadPdf: vi.fn(),
    exportPdf: vi.fn(),
    register: vi.fn(),
    disable: vi.fn()
  }
}));

import { goodsIssueService } from '@/services/goodsIssueService';
import { useGoodsIssueStore } from '../goodsIssueStore';

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
const mockIssue = {
  idIssue: 1, code: 'SA-001', type: 'Salida', storeName: 'Tienda A',
  idUser: 1, userName: 'sergio', totalAmount: 500,
  annotations: '', auditCreateDate: '2025-01-01', statusIssue: 'Completado'
};

const mockDetail = {
  idProduct: 1, code: 'P-001', description: 'Producto A',
  material: 'Metal', color: 'Rojo', categoryName: 'Cat A',
  brandName: 'Marca A', quantity: 2, unitPrice: 100,
  totalPrice: 200, stockAvailable: 10
};

const mockIssueWithDetails = { ...mockIssue, goodsIssueDetails: [mockDetail] };

// ── Setup ─────────────────────────────────────────────────────────────────────
beforeEach(() => {
  setActivePinia(createPinia());
  vi.clearAllMocks();

  vi.mocked(goodsIssueService.fetchAll).mockResolvedValue(
    mockResponse({ isSuccess: true, data: [mockIssue], totalRecords: 1 }) as any
  );
  vi.mocked(goodsIssueService.getIssueWithDetails).mockResolvedValue(
    mockResponse({ isSuccess: true, data: mockIssueWithDetails })
  );
  vi.mocked(goodsIssueService.downloadExcel).mockResolvedValue(undefined);
  vi.mocked(goodsIssueService.downloadPdf).mockResolvedValue(undefined);
  vi.mocked(goodsIssueService.exportPdf).mockResolvedValue({
    blob: new Blob(['pdf'], { type: 'application/pdf' }),
    filename: 'salida-001.pdf'
  });
  vi.mocked(goodsIssueService.register).mockResolvedValue(
    mockResponse({ isSuccess: true })
  );
  vi.mocked(goodsIssueService.disable).mockResolvedValue(
    mockResponse({ isSuccess: true })
  );
});

describe('useGoodsIssueStore', () => {

  // ── Estado inicial ────────────────────────────────────────────────────────
  it('inicializa con estado vacío', () => {
    const store = useGoodsIssueStore();

    expect(store.items).toEqual([]);
    expect(store.selectedItem).toBeNull();
    expect(store.selectedIssueDetails).toEqual([]);
    expect(store.totalItems).toBe(0);
    expect(store.loading).toBe(false);
  });

  // ── fetchGoodsIssue ───────────────────────────────────────────────────────
  it('fetchGoodsIssue carga items y totalItems', async () => {
    const store = useGoodsIssueStore();

    await store.fetchGoodsIssue({ pageNumber: 1, pageSize: 10 });

    expect(store.items).toEqual([mockIssue]);
    expect(store.totalItems).toBe(1);
  });

  it('fetchGoodsIssue guarda lastFilterParams', async () => {
    const store = useGoodsIssueStore();

    await store.fetchGoodsIssue({ pageNumber: 2, pageSize: 25 });

    expect(store.lastFilterParams).toEqual({ pageNumber: 2, pageSize: 25 });
  });

  it('fetchGoodsIssue lanza error cuando isSuccess es false', async () => {
    vi.mocked(goodsIssueService.fetchAll).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'Error del servidor' }) as any
    );
    const store = useGoodsIssueStore();

    await expect(store.fetchGoodsIssue()).rejects.toThrow('Error del servidor');
  });

  it('fetchGoodsIssue setea loading false aunque falle', async () => {
    vi.mocked(goodsIssueService.fetchAll).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'Error' }) as any
    );
    const store = useGoodsIssueStore();

    await store.fetchGoodsIssue().catch(() => { });

    expect(store.loading).toBe(false);
  });

  // ── fetchGoodsIssueById ───────────────────────────────────────────────────
  it('fetchGoodsIssueById carga selectedItem y selectedIssueDetails', async () => {
    const store = useGoodsIssueStore();

    await store.fetchGoodsIssueById(1);

    expect(store.selectedItem).toEqual(mockIssueWithDetails);
    expect(store.selectedIssueDetails).toHaveLength(1);
    expect(store.selectedIssueDetails[0].idProduct).toBe(1);
    expect(store.selectedIssueDetails[0].quantity).toBe(2);
  });

  it('fetchGoodsIssueById mapea los detalles correctamente', async () => {
    const store = useGoodsIssueStore();

    await store.fetchGoodsIssueById(1);

    const detail = store.selectedIssueDetails[0];
    expect(detail.code).toBe('P-001');
    expect(detail.description).toBe('Producto A');
    expect(detail.totalPrice).toBe(200);
  });

  it('fetchGoodsIssueById retorna array vacío si no hay detalles', async () => {
    vi.mocked(goodsIssueService.getIssueWithDetails).mockResolvedValue(
      mockResponse({ isSuccess: true, data: { ...mockIssueWithDetails, goodsIssueDetails: null } })
    );
    const store = useGoodsIssueStore();

    await store.fetchGoodsIssueById(1);

    expect(store.selectedIssueDetails).toEqual([]);
  });

  it('fetchGoodsIssueById lanza error cuando isSuccess es false', async () => {
    vi.mocked(goodsIssueService.getIssueWithDetails).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'No encontrado' })
    );
    const store = useGoodsIssueStore();

    await expect(store.fetchGoodsIssueById(99)).rejects.toThrow('No encontrado');
  });

  // ── Descargas ─────────────────────────────────────────────────────────────
  it('downloadGoodsIssueExcel llama al servicio con lastFilterParams', async () => {
    const store = useGoodsIssueStore();
    await store.fetchGoodsIssue({ pageNumber: 1, pageSize: 10 });

    await store.downloadGoodsIssueExcel();

    expect(goodsIssueService.downloadExcel).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  it('downloadGoodsIssuePdf llama al servicio con lastFilterParams', async () => {
    const store = useGoodsIssueStore();
    await store.fetchGoodsIssue({ pageNumber: 1, pageSize: 10 });

    await store.downloadGoodsIssuePdf();

    expect(goodsIssueService.downloadPdf).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  it('getBlobGoodsIssuePdf retorna un Blob', async () => {
    const store = useGoodsIssueStore();

    const blob = await store.getBlobGoodsIssuePdf(1);

    expect(blob).toBeInstanceOf(Blob);
  });

  // ── registerGoodsIssue ────────────────────────────────────────────────────
  it('registerGoodsIssue llama al servicio y recarga la lista', async () => {
    const store = useGoodsIssueStore();
    const issueData = {
      type: 'Salida', totalAmount: 200, annotations: '',
      idUser: 1, idStore: 1, goodsIssueDetails: []
    };

    await store.registerGoodsIssue(issueData);

    expect(goodsIssueService.register).toHaveBeenCalledWith(issueData);
    expect(goodsIssueService.fetchAll).toHaveBeenCalled();
  });

  it('registerGoodsIssue lanza error cuando isSuccess es false', async () => {
    vi.mocked(goodsIssueService.register).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'Stock insuficiente' })
    );
    const store = useGoodsIssueStore();

    await expect(store.registerGoodsIssue({} as any)).rejects.toThrow('Stock insuficiente');
  });

  // ── cancel (disableGoodsIssue) ────────────────────────────────────────────
  it('cancel llama a disable y recarga la lista', async () => {
    const store = useGoodsIssueStore();

    await store.cancel(1);

    expect(goodsIssueService.disable).toHaveBeenCalledWith(1);
    expect(goodsIssueService.fetchAll).toHaveBeenCalled();
  });

  it('cancel lanza error cuando isSuccess es false', async () => {
    vi.mocked(goodsIssueService.disable).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'No se puede cancelar' })
    );
    const store = useGoodsIssueStore();

    await expect(store.cancel(1)).rejects.toThrow('No se puede cancelar');
  });

  // ── Computed ──────────────────────────────────────────────────────────────
  it('goodsissue computed refleja items', async () => {
    const store = useGoodsIssueStore();

    await store.fetchGoodsIssue();

    expect(store.goodsissue).toEqual([mockIssue]);
  });

  it('totalGoodsIssue computed refleja totalItems', async () => {
    const store = useGoodsIssueStore();

    await store.fetchGoodsIssue();

    expect(store.totalGoodsIssue).toBe(1);
  });
});