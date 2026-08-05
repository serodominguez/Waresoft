import { describe, it, expect, vi, beforeEach } from 'vitest';
import axios from 'axios';
import { inventoryService } from '../inventoryService';
import type { Inventory, InventoryPivot } from '@/interfaces/inventoryInterface';
import type { BaseResponse, FilterParams } from '@/interfaces/baseInterface';

// ─── Mock de axios ────────────────────────────────────────────────────────────

vi.mock('axios');
const mockedAxios = vi.mocked(axios);

// ─── Mock de DOM ──────────────────────────────────────────────────────────────

const mockClick            = vi.fn();
const mockAppendChild      = vi.fn();
const mockRemoveChild      = vi.fn();
const mockSetAttribute     = vi.fn();
const mockCreateObjectURL  = vi.fn(() => 'blob:mock-url');
const mockRevokeObjectURL  = vi.fn();

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

const mockInventory: Inventory = {
  idStore:          1,
  idProduct:        10,
  code:             'PRD-001',
  description:      'Producto Test',
  material:         'Algodón',
  color:            'Rojo',
  unitMeasure:      'UND',
  stockAvailable:   50,
  calculatedStock:  48,
  stockDifference:  2,
  stockInTransit:   5,
  minimumStock:     10,
  price:            99.99,
  replenishment:    'Si',
  brandName:        'Marca A',
  categoryName:     'Categoría A',
  auditCreateDate:  '2024-01-01',
};

const mockPivot: InventoryPivot = {
  stores: ['Sucursal A', 'Sucursal B'],
  rows: [
    {
      image:        '',
      code:         'PRD-001',
      description:  'Producto Test',
      material:     'Algodón',
      color:        'Rojo',
      brandName:    'Marca A',
      categoryName: 'Categoría A',
      auditCreateDate: '2024-01-01',
      stockByStore: { 'Sucursal A': 10, 'Sucursal B': 5 },
    },
  ],
};

const mockParams: FilterParams = {
  pageNumber:  1,
  pageSize:    10,
  stateFilter: 1,
};

// ─── updatePrice ──────────────────────────────────────────────────────────────

describe('InventoryService.updatePrice', () => {

  it('llama a axios.put con el endpoint y datos correctos', async () => {
    mockedAxios.put.mockResolvedValueOnce({ data: makeBaseResponse(mockInventory) });

    await inventoryService.updatePrice(mockInventory);

    expect(mockedAxios.put).toHaveBeenCalledWith(
      'api/StoreInventory/Edit',
      mockInventory
    );
  });

  it('retorna el BaseResponse completo', async () => {
    const response = makeBaseResponse(mockInventory);
    mockedAxios.put.mockResolvedValueOnce({ data: response });

    const result = await inventoryService.updatePrice(mockInventory);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.put.mockRejectedValueOnce(new Error('Network Error'));

    await expect(inventoryService.updatePrice(mockInventory)).rejects.toThrow('Network Error');
  });
});

// ─── fetchCalculated ──────────────────────────────────────────────────────────

describe('InventoryService.fetchCalculated', () => {

  it('llama al endpoint Calculated con los params correctos', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: makeBaseResponse([mockInventory]) });

    await inventoryService.fetchCalculated(mockParams);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Calculated',
      expect.objectContaining({
        params: expect.objectContaining({ NumberPage: 1, NumberRecordsPage: 10 }),
      })
    );
  });

  it('retorna el BaseResponse con el array de inventario', async () => {
    const response = makeBaseResponse([mockInventory]);
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    const result = await inventoryService.fetchCalculated();

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(inventoryService.fetchCalculated()).rejects.toThrow('Network Error');
  });
});

// ─── fetchPivot ───────────────────────────────────────────────────────────────

describe('InventoryService.fetchPivot', () => {

  it('llama al endpoint Pivot con los params correctos', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: makeBaseResponse(mockPivot) });

    await inventoryService.fetchPivot(mockParams);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Pivot',
      expect.objectContaining({
        params: expect.objectContaining({ NumberPage: 1 }),
      })
    );
  });

  it('retorna el BaseResponse con el pivot', async () => {
    const response = makeBaseResponse(mockPivot);
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    const result = await inventoryService.fetchPivot();

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(inventoryService.fetchPivot()).rejects.toThrow('Network Error');
  });
});

// ─── downloadCalculatedExcel ──────────────────────────────────────────────────

describe('InventoryService.downloadCalculatedExcel', () => {

  it('llama a axios.get con Download true y responseType blob', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['excel']) });

    await inventoryService.downloadCalculatedExcel();

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Calculated',
      expect.objectContaining({
        responseType: 'blob',
        params: expect.objectContaining({ Download: true }),
      })
    );
  });

  it('dispara la descarga con nombre correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['excel']) });

    await inventoryService.downloadCalculatedExcel();

    const date = new Date().toISOString().split('T')[0];
    expect(mockSetAttribute).toHaveBeenCalledWith('download', `Inventario_${date}.xlsx`);
    expect(mockClick).toHaveBeenCalled();
    expect(mockRevokeObjectURL).toHaveBeenCalledWith('blob:mock-url');
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(inventoryService.downloadCalculatedExcel()).rejects.toThrow('Network Error');
  });
});

// ─── downloadCalculatedPdf ────────────────────────────────────────────────────

describe('InventoryService.downloadCalculatedPdf', () => {

  it('llama a axios.get con DownloadType pdf', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['pdf']) });

    await inventoryService.downloadCalculatedPdf();

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Calculated',
      expect.objectContaining({
        responseType: 'blob',
        params: expect.objectContaining({ Download: true, DownloadType: 'pdf' }),
      })
    );
  });

  it('dispara la descarga con nombre correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['pdf']) });

    await inventoryService.downloadCalculatedPdf();

    const date = new Date().toISOString().split('T')[0];
    expect(mockSetAttribute).toHaveBeenCalledWith('download', `Inventario_${date}.pdf`);
    expect(mockClick).toHaveBeenCalled();
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(inventoryService.downloadCalculatedPdf()).rejects.toThrow('Network Error');
  });
});

// ─── downloadPivotExcel ───────────────────────────────────────────────────────

describe('InventoryService.downloadPivotExcel', () => {

  it('llama al endpoint Pivot con Download true', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['excel']) });

    await inventoryService.downloadPivotExcel();

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Pivot',
      expect.objectContaining({
        responseType: 'blob',
        params: expect.objectContaining({ Download: true }),
      })
    );
  });

  it('dispara la descarga con nombre Consolidado_Existencias', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['excel']) });

    await inventoryService.downloadPivotExcel();

    const date = new Date().toISOString().split('T')[0];
    expect(mockSetAttribute).toHaveBeenCalledWith('download', `Consolidado_Existencias_${date}.xlsx`);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(inventoryService.downloadPivotExcel()).rejects.toThrow('Network Error');
  });
});

// ─── downloadPivotPdf ─────────────────────────────────────────────────────────

describe('InventoryService.downloadPivotPdf', () => {

  it('llama al endpoint Pivot con DownloadType pdf', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['pdf']) });

    await inventoryService.downloadPivotPdf();

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Pivot',
      expect.objectContaining({
        responseType: 'blob',
        params: expect.objectContaining({ Download: true, DownloadType: 'pdf' }),
      })
    );
  });

  it('dispara la descarga con nombre Consolidado_Existencias', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['pdf']) });

    await inventoryService.downloadPivotPdf();

    const date = new Date().toISOString().split('T')[0];
    expect(mockSetAttribute).toHaveBeenCalledWith('download', `Consolidado_Existencias_${date}.pdf`);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(inventoryService.downloadPivotPdf()).rejects.toThrow('Network Error');
  });
});

// ─── inventorySheet ───────────────────────────────────────────────────────────

describe('InventoryService.inventorySheet', () => {

  it('llama al endpoint Sheet con DownloadType pdf', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['pdf']) });

    await inventoryService.inventorySheet();

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/StoreInventory/Sheet',
      expect.objectContaining({
        responseType: 'blob',
        params: expect.objectContaining({ Download: true, DownloadType: 'pdf' }),
      })
    );
  });

  it('usa solo downloadFileName si no se pasa storeName', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['pdf']) });

    await inventoryService.inventorySheet();

    const date = new Date().toISOString().split('T')[0];
    expect(mockSetAttribute).toHaveBeenCalledWith('download', `Inventario_${date}.pdf`);
  });

  it('incluye el storeName como prefijo en el nombre del archivo', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: new Blob(['pdf']) });

    await inventoryService.inventorySheet({}, 'Sucursal A');

    const date = new Date().toISOString().split('T')[0];
    expect(mockSetAttribute).toHaveBeenCalledWith('download', `Sucursal A_Inventario_${date}.pdf`);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(inventoryService.inventorySheet()).rejects.toThrow('Network Error');
  });
});