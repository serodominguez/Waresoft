import { describe, it, expect, vi, beforeEach } from 'vitest';
import axios from 'axios';
import { goodsReceiptService } from '../goodsReceiptService';
import type { GoodsReceiptRegister } from '@/interfaces/goodsReceiptInterface';
import type { BaseResponse } from '@/interfaces/baseInterface';

// ─── Mock de axios ────────────────────────────────────────────────────────────

vi.mock('axios');
const mockedAxios = vi.mocked(axios);

beforeEach(() => {
  vi.clearAllMocks();
});

// ─── Fixtures ─────────────────────────────────────────────────────────────────

const makeBaseResponse = <T>(data: T): BaseResponse<T> => ({
  isSuccess:    true,
  message:      'OK',
  data,
  totalRecords: 0,
  errors:       null,
});

const mockRegisterData: GoodsReceiptRegister = {
  type:           'Compra',
  documentDate:   '2024-01-15',
  documentType:   'Factura',
  documentNumber: 'FAC-001',
  totalAmount:    500.00,
  annotations:    'Entrada de prueba',
  idSupplier:     1,
  idStore:        2,
  goodsReceiptDetails: [
    {
      item:       1,
      idProduct:  10,
      quantity:   5,
      unitCost:   80.00,
      totalCost:  400.00,
    },
  ],
};

// ─── register ─────────────────────────────────────────────────────────────────

describe('GoodsReceiptService.register', () => {

  it('llama a axios.post con el endpoint y datos correctos', async () => {
    mockedAxios.post.mockResolvedValueOnce({ data: makeBaseResponse(null) });

    await goodsReceiptService.register(mockRegisterData);

    expect(mockedAxios.post).toHaveBeenCalledWith(
      'api/GoodsReceipt/Register',
      mockRegisterData
    );
  });

  it('retorna el BaseResponse completo', async () => {
    const response = makeBaseResponse({ idReceipt: 1 });
    mockedAxios.post.mockResolvedValueOnce({ data: response });

    const result = await goodsReceiptService.register(mockRegisterData);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.post.mockRejectedValueOnce(new Error('Network Error'));

    await expect(goodsReceiptService.register(mockRegisterData)).rejects.toThrow('Network Error');
  });
});

// ─── disable ──────────────────────────────────────────────────────────────────

describe('GoodsReceiptService.disable', () => {

  it('llama a axios.put con el endpoint correcto', async () => {
    mockedAxios.put.mockResolvedValueOnce({ data: makeBaseResponse(undefined) });

    await goodsReceiptService.disable(5);

    expect(mockedAxios.put).toHaveBeenCalledWith('api/GoodsReceipt/Disable/5', {});
  });

  it('retorna el BaseResponse', async () => {
    const response = makeBaseResponse(undefined);
    mockedAxios.put.mockResolvedValueOnce({ data: response });

    const result = await goodsReceiptService.disable(5);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.put.mockRejectedValueOnce(new Error('Network Error'));

    await expect(goodsReceiptService.disable(5)).rejects.toThrow('Network Error');
  });
});

// ─── getReceiptWithDetails ────────────────────────────────────────────────────

describe('GoodsReceiptService.getReceiptWithDetails', () => {

  it('llama a axios.get con el endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: makeBaseResponse(null) });

    await goodsReceiptService.getReceiptWithDetails(3);

    expect(mockedAxios.get).toHaveBeenCalledWith('api/GoodsReceipt/3');
  });

  it('retorna el BaseResponse completo', async () => {
    const response = makeBaseResponse({ idReceipt: 3, code: 'ENT-003' });
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    const result = await goodsReceiptService.getReceiptWithDetails(3);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(goodsReceiptService.getReceiptWithDetails(3)).rejects.toThrow('Network Error');
  });
});

// ─── exportPdf ────────────────────────────────────────────────────────────────

describe('GoodsReceiptService.exportPdf', () => {

  it('llama a axios.get con responseType blob', async () => {
    const blob = new Blob(['pdf'], { type: 'application/pdf' });
    mockedAxios.get.mockResolvedValueOnce({ data: blob, headers: {} });

    await goodsReceiptService.exportPdf(7);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/GoodsReceipt/ExportPdf/7',
      { responseType: 'blob' }
    );
  });

  it('usa el filename por defecto si no hay content-disposition', async () => {
    const blob = new Blob(['pdf']);
    mockedAxios.get.mockResolvedValueOnce({ data: blob, headers: {} });

    const result = await goodsReceiptService.exportPdf(7);

    expect(result.filename).toBe('Entrada_7.pdf');
    expect(result.blob).toBe(blob);
  });

  it('extrae el filename del header content-disposition estándar', async () => {
    const blob = new Blob(['pdf']);
    mockedAxios.get.mockResolvedValueOnce({
      data: blob,
      headers: { 'content-disposition': 'attachment; filename="Entrada_007.pdf"' },
    });

    const result = await goodsReceiptService.exportPdf(7);

    expect(result.filename).toBe('Entrada_007.pdf');
  });

  it('extrae el filename del header con encoding UTF-8', async () => {
    const blob = new Blob(['pdf']);
    mockedAxios.get.mockResolvedValueOnce({
      data: blob,
      headers: { 'content-disposition': "attachment; filename*=UTF-8''Entrada%20Especial.pdf" },
    });

    const result = await goodsReceiptService.exportPdf(7);

    expect(result.filename).toBe('Entrada Especial.pdf');
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(goodsReceiptService.exportPdf(7)).rejects.toThrow('Network Error');
  });
});