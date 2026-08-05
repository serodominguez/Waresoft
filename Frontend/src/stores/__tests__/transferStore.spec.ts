import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import type { BaseResponse } from '@/interfaces/baseInterface';

vi.mock('@/services/transferService', () => ({
  transferService: {
    fetchAll:                vi.fn(),
    getTransferWithDetails:  vi.fn(),
    downloadExcel:           vi.fn(),
    downloadPdf:             vi.fn(),
    exportPdf:               vi.fn(),
    send:                    vi.fn(),
    receive:                 vi.fn(),
    disable:                 vi.fn(),
    getStats:                vi.fn(),
  }
}));

import { transferService } from '@/services/transferService';
import { useTransferStore, useTransferStatsStore } from '../transferStore';

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
const mockTransfer = {
  idTransfer: 1, code: 'TR-001', storeOriginName: 'Tienda A',
  storeDestinationName: 'Tienda B', idUser: 1, userName: 'sergio',
  totalAmount: 600, annotations: '', auditCreateDate: '2025-01-01',
  statusTransfer: 'Pendiente'
};

const mockDetail = {
  idProduct: 1, code: 'P-001', description: 'Producto A',
  material: 'Metal', color: 'Rojo', categoryName: 'Cat A',
  brandName: 'Marca A', quantity: 2, unitPrice: 100, totalPrice: 200
};

const mockTransferWithDetails = { ...mockTransfer, transferDetails: [mockDetail] };

const mockStats = {
  totalTransfers: 10, pending: 3, completed: 7
};

// ── Setup ─────────────────────────────────────────────────────────────────────
beforeEach(() => {
  setActivePinia(createPinia());
  vi.clearAllMocks();

  vi.mocked(transferService.fetchAll).mockResolvedValue(
    mockResponse({ isSuccess: true, data: [mockTransfer], totalRecords: 1 }) as any
  );
  vi.mocked(transferService.getTransferWithDetails).mockResolvedValue(
    mockResponse({ isSuccess: true, data: mockTransferWithDetails })
  );
  vi.mocked(transferService.downloadExcel).mockResolvedValue(undefined as any);
  vi.mocked(transferService.downloadPdf).mockResolvedValue(undefined as any);
  vi.mocked(transferService.exportPdf).mockResolvedValue({
    blob: new Blob(['pdf'], { type: 'application/pdf' }),
    filename: 'transferencia-001.pdf'
  });
  vi.mocked(transferService.send).mockResolvedValue(
    mockResponse({ isSuccess: true })
  );
  vi.mocked(transferService.receive).mockResolvedValue(
    mockResponse({ isSuccess: true })
  );
  vi.mocked(transferService.disable).mockResolvedValue(
    mockResponse({ isSuccess: true })
  );
  vi.mocked(transferService.getStats).mockResolvedValue(mockStats as any);
});

// ══════════════════════════════════════════════════════════════════════════════
describe('useTransferStore', () => {

  // ── Estado inicial ────────────────────────────────────────────────────────
  it('inicializa con estado vacío', () => {
    const store = useTransferStore();

    expect(store.items).toEqual([]);
    expect(store.selectedItem).toBeNull();
    expect(store.selectedTransferDetails).toEqual([]);
    expect(store.totalItems).toBe(0);
    expect(store.loading).toBe(false);
  });

  // ── fetchTransfers ────────────────────────────────────────────────────────
  it('fetchTransfers carga items y totalItems', async () => {
    const store = useTransferStore();

    await store.fetchTransfers({ pageNumber: 1, pageSize: 10 });

    expect(store.items).toEqual([mockTransfer]);
    expect(store.totalItems).toBe(1);
  });

  it('fetchTransfers guarda lastFilterParams', async () => {
    const store = useTransferStore();

    await store.fetchTransfers({ pageNumber: 2, pageSize: 25 });

    expect(store.lastFilterParams).toEqual({ pageNumber: 2, pageSize: 25 });
  });

  it('fetchTransfers lanza error cuando isSuccess es false', async () => {
    vi.mocked(transferService.fetchAll).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'Error del servidor' }) as any
    );
    const store = useTransferStore();

    await expect(store.fetchTransfers()).rejects.toThrow('Error del servidor');
  });

  it('fetchTransfers setea loading false aunque falle', async () => {
    vi.mocked(transferService.fetchAll).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'Error' }) as any
    );
    const store = useTransferStore();

    await store.fetchTransfers().catch(() => {});

    expect(store.loading).toBe(false);
  });

  // ── fetchTransferById ─────────────────────────────────────────────────────
  it('fetchTransferById carga selectedItem y selectedTransferDetails', async () => {
    const store = useTransferStore();

    await store.fetchTransferById(1);

    expect(store.selectedItem).toEqual(mockTransferWithDetails);
    expect(store.selectedTransferDetails).toHaveLength(1);
    expect(store.selectedTransferDetails[0].idProduct).toBe(1);
    expect(store.selectedTransferDetails[0].quantity).toBe(2);
  });

  it('fetchTransferById mapea los detalles correctamente', async () => {
    const store = useTransferStore();

    await store.fetchTransferById(1);

    const detail = store.selectedTransferDetails[0];
    expect(detail.code).toBe('P-001');
    expect(detail.description).toBe('Producto A');
    expect(detail.totalPrice).toBe(200);
  });

  it('fetchTransferById retorna array vacío si no hay detalles', async () => {
    vi.mocked(transferService.getTransferWithDetails).mockResolvedValue(
      mockResponse({ isSuccess: true, data: { ...mockTransferWithDetails, transferDetails: null } })
    );
    const store = useTransferStore();

    await store.fetchTransferById(1);

    expect(store.selectedTransferDetails).toEqual([]);
  });

  it('fetchTransferById lanza error cuando isSuccess es false', async () => {
    vi.mocked(transferService.getTransferWithDetails).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'No encontrado' })
    );
    const store = useTransferStore();

    await expect(store.fetchTransferById(99)).rejects.toThrow('No encontrado');
  });

  // ── Descargas ─────────────────────────────────────────────────────────────
  it('downloadTransferExcel llama al servicio con lastFilterParams', async () => {
    const store = useTransferStore();
    await store.fetchTransfers({ pageNumber: 1, pageSize: 10 });

    await store.downloadTransferExcel();

    expect(transferService.downloadExcel).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  it('downloadTransferPdf llama al servicio con lastFilterParams', async () => {
    const store = useTransferStore();
    await store.fetchTransfers({ pageNumber: 1, pageSize: 10 });

    await store.downloadTransferPdf();

    expect(transferService.downloadPdf).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  it('getBlobTransferPdf retorna un Blob', async () => {
    const store = useTransferStore();

    const blob = await store.getBlobTransferPdf(1);

    expect(blob).toBeInstanceOf(Blob);
  });

  // ── sendTrasnfer ──────────────────────────────────────────────────────────
  it('sendTrasnfer llama al servicio y recarga la lista', async () => {
    const store = useTransferStore();
    const transferData = {
      idStoreOrigin: 1, idStoreDestination: 2,
      idUser: 1, annotations: '', transferDetails: []
    };

    await store.sendTrasnfer(transferData as any);

    expect(transferService.send).toHaveBeenCalledWith(transferData);
    expect(transferService.fetchAll).toHaveBeenCalled();
  });

  it('sendTrasnfer lanza error cuando isSuccess es false', async () => {
    vi.mocked(transferService.send).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'Stock insuficiente' })
    );
    const store = useTransferStore();

    await expect(store.sendTrasnfer({} as any)).rejects.toThrow('Stock insuficiente');
  });

  // ── receiveTransfer ───────────────────────────────────────────────────────
  it('receiveTransfer llama al servicio y recarga la lista', async () => {
    const store = useTransferStore();

    await store.receiveTransfer(1);

    expect(transferService.receive).toHaveBeenCalledWith(1);
    expect(transferService.fetchAll).toHaveBeenCalled();
  });

  it('receiveTransfer lanza error cuando isSuccess es false', async () => {
    vi.mocked(transferService.receive).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'No se puede recibir' })
    );
    const store = useTransferStore();

    await expect(store.receiveTransfer(1)).rejects.toThrow('No se puede recibir');
  });

  // ── cancel (disableTransfer) ──────────────────────────────────────────────
  it('cancel llama a disable y recarga la lista', async () => {
    const store = useTransferStore();

    await store.cancel(1);

    expect(transferService.disable).toHaveBeenCalledWith(1);
    expect(transferService.fetchAll).toHaveBeenCalled();
  });

  it('cancel lanza error cuando isSuccess es false', async () => {
    vi.mocked(transferService.disable).mockResolvedValue(
      mockResponse({ isSuccess: false, message: 'No se puede cancelar' })
    );
    const store = useTransferStore();

    await expect(store.cancel(1)).rejects.toThrow('No se puede cancelar');
  });

  // ── Computed ──────────────────────────────────────────────────────────────
  it('transfers computed refleja items', async () => {
    const store = useTransferStore();

    await store.fetchTransfers();

    expect(store.transfers).toEqual([mockTransfer]);
  });

  it('totalTransfers computed refleja totalItems', async () => {
    const store = useTransferStore();

    await store.fetchTransfers();

    expect(store.totalTransfers).toBe(1);
  });
});

// ══════════════════════════════════════════════════════════════════════════════
describe('useTransferStatsStore', () => {

  it('inicializa con stats null', () => {
    const store = useTransferStatsStore();

    expect(store.stats).toBeNull();
    expect(store.loading).toBe(false);
  });

  it('fetchStats carga las estadísticas', async () => {
    const store = useTransferStatsStore();

    await store.fetchStats();

    expect(store.stats).toEqual(mockStats);
    expect(transferService.getStats).toHaveBeenCalled();
  });

  it('fetchStats setea loading false aunque falle', async () => {
    vi.mocked(transferService.getStats).mockRejectedValue(new Error('Error de red'));
    const store = useTransferStatsStore();

    await store.fetchStats().catch(() => {});

    expect(store.loading).toBe(false);
  });
});