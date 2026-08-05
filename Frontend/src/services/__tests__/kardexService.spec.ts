import { describe, it, expect, vi, beforeEach } from 'vitest';
import axios from 'axios';
import { kardexService } from '../kardexService';
import type { KardexDetail } from '@/interfaces/kardexInterface';
import type { BaseResponse, FilterParams } from '@/interfaces/baseInterface';

// ─── Mock de axios ────────────────────────────────────────────────────────────

vi.mock('axios');
const mockedAxios = vi.mocked(axios);

// ─── Mock de DOM ──────────────────────────────────────────────────────────────

const mockClick           = vi.fn();
const mockAppendChild     = vi.fn();
const mockRemoveChild     = vi.fn();
const mockSetAttribute    = vi.fn();
const mockCreateObjectURL = vi.fn(() => 'blob:mock-url');
const mockRevokeObjectURL = vi.fn();

beforeEach(() => {
  vi.clearAllMocks();

  vi.spyOn(document, 'createElement').mockReturnValue({
    href:         '',
    setAttribute: mockSetAttribute,
    click:        mockClick,
    parentNode:   { removeChild: mockRemoveChild },
  } as unknown as HTMLElement);

  vi.spyOn(document.body, 'appendChild').mockImplementation(mockAppendChild);

  window.URL.createObjectURL = mockCreateObjectURL;
  window.URL.revokeObjectURL = mockRevokeObjectURL;
});

// ─── Fixtures ─────────────────────────────────────────────────────────────────

const makeBaseResponse = <T>(data: T): BaseResponse<T> => ({
  isSuccess:    true,
  message:      'OK',
  data,
  totalRecords: 0,
  errors:       null,
});

const mockKardex: KardexDetail = {
  idProduct:       1,
  code:            'PRD-001',
  description:     'Producto Test',
  material:        'Algodón',
  color:           'Rojo',
  unitMeasure:     'UND',
  currentStock:    50,
  calculatedStock: 48,
  stockDifference: 2,
  movements: [
    {
      idProduct:       1,
      quantity:        10,
      idMovement:      100,
      code:            'ENT-001',
      date:            '2024-01-10',
      movementType:    'Entrada',
      type:            'Compra',
      state:           'Activo',
      accumulatedStock: 50,
    },
  ],
};

const mockParams: FilterParams = {
  pageNumber:  1,
  pageSize:    10,
  stateFilter: 1,
};

// ─── fetchKardex ──────────────────────────────────────────────────────────────

describe('KardexService.fetchKardex', () => {

  it('llama al endpoint correcto con productId y params', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: makeBaseResponse(mockKardex) });

    await kardexService.fetchKardex(1, mockParams);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Kardex',
      expect.objectContaining({
        params: expect.objectContaining({
          productId:         1,
          NumberPage:        1,
          NumberRecordsPage: 10,
        }),
      })
    );
  });

  it('pasa el signal de AbortController si se proporciona', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: makeBaseResponse(mockKardex) });
    const controller = new AbortController();

    await kardexService.fetchKardex(1, {}, controller.signal);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Kardex',
      expect.objectContaining({ signal: controller.signal })
    );
  });

  it('no incluye signal si no se proporciona', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: makeBaseResponse(mockKardex) });

    await kardexService.fetchKardex(1);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Kardex',
      expect.objectContaining({ signal: undefined })
    );
  });

  it('retorna el BaseResponse completo', async () => {
    const response = makeBaseResponse(mockKardex);
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    const result = await kardexService.fetchKardex(1);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(kardexService.fetchKardex(1)).rejects.toThrow('Network Error');
  });
});

// ─── downloadKardexExcel ──────────────────────────────────────────────────────

describe('KardexService.downloadKardexExcel', () => {

  it('llama al endpoint con productId y Download true', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['excel']) });

    await kardexService.downloadKardexExcel(1, mockParams);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Kardex',
      expect.objectContaining({
        responseType: 'blob',
        params: expect.objectContaining({
          productId: 1,
          Download:  true,
        }),
      })
    );
  });

  it('dispara la descarga con nombre Kardex y extension xlsx', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['excel']) });

    await kardexService.downloadKardexExcel(1);

    const date = new Date().toISOString().split('T')[0];
    expect(mockSetAttribute).toHaveBeenCalledWith('download', `Kardex_${date}.xlsx`);
    expect(mockClick).toHaveBeenCalled();
    expect(mockRevokeObjectURL).toHaveBeenCalledWith('blob:mock-url');
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(kardexService.downloadKardexExcel(1)).rejects.toThrow('Network Error');
  });
});

// ─── downloadKardexPdf ────────────────────────────────────────────────────────

describe('KardexService.downloadKardexPdf', () => {

  it('llama al endpoint con productId, Download true y DownloadType pdf', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['pdf']) });

    await kardexService.downloadKardexPdf(1, mockParams);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Kardex',
      expect.objectContaining({
        responseType: 'blob',
        params: expect.objectContaining({
          productId:    1,
          Download:     true,
          DownloadType: 'pdf',
        }),
      })
    );
  });

  it('dispara la descarga con nombre Kardex y extension pdf', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['pdf']) });

    await kardexService.downloadKardexPdf(1);

    const date = new Date().toISOString().split('T')[0];
    expect(mockSetAttribute).toHaveBeenCalledWith('download', `Kardex_${date}.pdf`);
    expect(mockClick).toHaveBeenCalled();
    expect(mockRevokeObjectURL).toHaveBeenCalledWith('blob:mock-url');
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(kardexService.downloadKardexPdf(1)).rejects.toThrow('Network Error');
  });
});