import { describe, it, expect, vi, beforeEach } from 'vitest';
import axios from 'axios';
import { goodsIssueService } from '../goodsIssueService';
import type { GoodsIssueRegister } from '@/interfaces/goodsIssueInterface';
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

const mockRegisterData: GoodsIssueRegister = {
  type:         'Venta',
  totalAmount:  150.00,
  annotations:  'Salida de prueba',
  idUser:       1,
  idStore:      2,
  goodsIssueDetails: [
    {
      item:       1,
      idProduct:  10,
      quantity:   2,
      unitPrice:  50.00,
      totalPrice: 100.00,
    },
  ],
};

// ─── register ─────────────────────────────────────────────────────────────────

describe('GoodsIssueService.register', () => {

  it('llama a axios.post con el endpoint y datos correctos', async () => {
    mockedAxios.post.mockResolvedValueOnce({ data: makeBaseResponse(null) });

    await goodsIssueService.register(mockRegisterData);

    expect(mockedAxios.post).toHaveBeenCalledWith(
      'api/GoodsIssue/Register',
      mockRegisterData
    );
  });

  it('retorna el BaseResponse completo', async () => {
    const response = makeBaseResponse({ idIssue: 1 });
    mockedAxios.post.mockResolvedValueOnce({ data: response });

    const result = await goodsIssueService.register(mockRegisterData);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.post.mockRejectedValueOnce(new Error('Network Error'));

    await expect(goodsIssueService.register(mockRegisterData)).rejects.toThrow('Network Error');
  });
});

// ─── disable ──────────────────────────────────────────────────────────────────

describe('GoodsIssueService.disable', () => {

  it('llama a axios.put con el endpoint correcto', async () => {
    mockedAxios.put.mockResolvedValueOnce({ data: makeBaseResponse(undefined) });

    await goodsIssueService.disable(5);

    expect(mockedAxios.put).toHaveBeenCalledWith('api/GoodsIssue/Disable/5', {});
  });

  it('retorna el BaseResponse', async () => {
    const response = makeBaseResponse(undefined);
    mockedAxios.put.mockResolvedValueOnce({ data: response });

    const result = await goodsIssueService.disable(5);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.put.mockRejectedValueOnce(new Error('Network Error'));

    await expect(goodsIssueService.disable(5)).rejects.toThrow('Network Error');
  });
});

// ─── getIssueWithDetails ──────────────────────────────────────────────────────

describe('GoodsIssueService.getIssueWithDetails', () => {

  it('llama a axios.get con el endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: makeBaseResponse(null) });

    await goodsIssueService.getIssueWithDetails(3);

    expect(mockedAxios.get).toHaveBeenCalledWith('api/GoodsIssue/3');
  });

  it('retorna el BaseResponse completo', async () => {
    const response = makeBaseResponse({ idIssue: 3, code: 'SAL-003' });
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    const result = await goodsIssueService.getIssueWithDetails(3);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(goodsIssueService.getIssueWithDetails(3)).rejects.toThrow('Network Error');
  });
});

// ─── exportPdf ────────────────────────────────────────────────────────────────

describe('GoodsIssueService.exportPdf', () => {

  it('llama a axios.get con responseType blob', async () => {
    const blob = new Blob(['pdf'], { type: 'application/pdf' });
    mockedAxios.get.mockResolvedValueOnce({ data: blob, headers: {} });

    await goodsIssueService.exportPdf(7);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/GoodsIssue/ExportPdf/7',
      { responseType: 'blob' }
    );
  });

  it('usa el filename por defecto si no hay content-disposition', async () => {
    const blob = new Blob(['pdf']);
    mockedAxios.get.mockResolvedValueOnce({ data: blob, headers: {} });

    const result = await goodsIssueService.exportPdf(7);

    expect(result.filename).toBe('Salida_7.pdf');
    expect(result.blob).toBe(blob);
  });

  it('extrae el filename del header content-disposition estándar', async () => {
    const blob = new Blob(['pdf']);
    mockedAxios.get.mockResolvedValueOnce({
      data: blob,
      headers: { 'content-disposition': 'attachment; filename="Salida_007.pdf"' },
    });

    const result = await goodsIssueService.exportPdf(7);

    expect(result.filename).toBe('Salida_007.pdf');
  });

  it('extrae el filename del header con encoding UTF-8', async () => {
    const blob = new Blob(['pdf']);
    mockedAxios.get.mockResolvedValueOnce({
      data: blob,
      headers: { 'content-disposition': "attachment; filename*=UTF-8''Salida%20Especial.pdf" },
    });

    const result = await goodsIssueService.exportPdf(7);

    expect(result.filename).toBe('Salida Especial.pdf');
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(goodsIssueService.exportPdf(7)).rejects.toThrow('Network Error');
  });
});