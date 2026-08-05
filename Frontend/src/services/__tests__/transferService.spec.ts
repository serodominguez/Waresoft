import { describe, it, expect, vi, beforeEach } from 'vitest';
import axios from 'axios';
import { transferService } from '../transferService';
import type { TransferRegister, TransferStats } from '@/interfaces/transferInterface';
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

const mockStats: TransferStats = {
  totalPending:          3,
  differenceVsYesterday: 1,
  isPendingPositive:     false,
  totalSent:             10,
  sentPercentageChange:  20,
  isSentPositive:        true,
};

const mockRegisterData: TransferRegister = {
  totalAmount:        200.00,
  annotations:        'Traspaso de prueba',
  idStoreOrigin:      1,
  idStoreDestination: 2,
  transferDetails: [
    {
      item:       1,
      idProduct:  5,
      quantity:   2,
      unitPrice:  50.00,
      totalPrice: 100.00,
    },
  ],
};

// ─── getStats ─────────────────────────────────────────────────────────────────

describe('TransferService.getStats', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: { data: mockStats } });

    await transferService.getStats();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Transfer/Stats');
  });

  it('retorna los datos correctamente', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: { data: mockStats } });

    const result = await transferService.getStats();

    expect(result).toEqual(mockStats);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(transferService.getStats()).rejects.toThrow('Network Error');
  });
});

// ─── send ─────────────────────────────────────────────────────────────────────

describe('TransferService.send', () => {

  it('llama a axios.post con el endpoint y datos correctos', async () => {
    mockedAxios.post.mockResolvedValueOnce({ data: makeBaseResponse(null) });

    await transferService.send(mockRegisterData);

    expect(mockedAxios.post).toHaveBeenCalledWith(
      'api/Transfer/Send',
      mockRegisterData
    );
  });

  it('retorna el BaseResponse completo', async () => {
    const response = makeBaseResponse({ idTransfer: 1 });
    mockedAxios.post.mockResolvedValueOnce({ data: response });

    const result = await transferService.send(mockRegisterData);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.post.mockRejectedValueOnce(new Error('Network Error'));

    await expect(transferService.send(mockRegisterData)).rejects.toThrow('Network Error');
  });
});

// ─── receive ──────────────────────────────────────────────────────────────────

describe('TransferService.receive', () => {

  it('llama a axios.put con el endpoint correcto', async () => {
    mockedAxios.put.mockResolvedValueOnce({ data: makeBaseResponse(undefined) });

    await transferService.receive(4);

    expect(mockedAxios.put).toHaveBeenCalledWith('api/Transfer/Receive/4', {});
  });

  it('retorna el BaseResponse', async () => {
    const response = makeBaseResponse(undefined);
    mockedAxios.put.mockResolvedValueOnce({ data: response });

    const result = await transferService.receive(4);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.put.mockRejectedValueOnce(new Error('Network Error'));

    await expect(transferService.receive(4)).rejects.toThrow('Network Error');
  });
});

// ─── disable ──────────────────────────────────────────────────────────────────

describe('TransferService.disable', () => {

  it('llama a axios.put con el endpoint correcto', async () => {
    mockedAxios.put.mockResolvedValueOnce({ data: makeBaseResponse(undefined) });

    await transferService.disable(5);

    expect(mockedAxios.put).toHaveBeenCalledWith('api/Transfer/Disable/5', {});
  });

  it('retorna el BaseResponse', async () => {
    const response = makeBaseResponse(undefined);
    mockedAxios.put.mockResolvedValueOnce({ data: response });

    const result = await transferService.disable(5);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.put.mockRejectedValueOnce(new Error('Network Error'));

    await expect(transferService.disable(5)).rejects.toThrow('Network Error');
  });
});

// ─── getTransferWithDetails ───────────────────────────────────────────────────

describe('TransferService.getTransferWithDetails', () => {

  it('llama a axios.get con el endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: makeBaseResponse(null) });

    await transferService.getTransferWithDetails(3);

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Transfer/3');
  });

  it('retorna el BaseResponse completo', async () => {
    const response = makeBaseResponse({ idTransfer: 3, code: 'TRA-003' });
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    const result = await transferService.getTransferWithDetails(3);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(transferService.getTransferWithDetails(3)).rejects.toThrow('Network Error');
  });
});

// ─── exportPdf ────────────────────────────────────────────────────────────────

describe('TransferService.exportPdf', () => {

  it('llama a axios.get con responseType blob', async () => {
    const blob = new Blob(['pdf'], { type: 'application/pdf' });
    mockedAxios.get.mockResolvedValueOnce({ data: blob, headers: {} });

    await transferService.exportPdf(7);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/Transfer/ExportPdf/7',
      { responseType: 'blob' }
    );
  });

  it('usa el filename por defecto si no hay content-disposition', async () => {
    const blob = new Blob(['pdf']);
    mockedAxios.get.mockResolvedValueOnce({ data: blob, headers: {} });

    const result = await transferService.exportPdf(7);

    expect(result.filename).toBe('Traspaso_7.pdf');
    expect(result.blob).toBe(blob);
  });

  it('extrae el filename del header content-disposition estándar', async () => {
    const blob = new Blob(['pdf']);
    mockedAxios.get.mockResolvedValueOnce({
      data: blob,
      headers: { 'content-disposition': 'attachment; filename="Traspaso_007.pdf"' },
    });

    const result = await transferService.exportPdf(7);

    expect(result.filename).toBe('Traspaso_007.pdf');
  });

  it('extrae el filename del header con encoding UTF-8', async () => {
    const blob = new Blob(['pdf']);
    mockedAxios.get.mockResolvedValueOnce({
      data: blob,
      headers: { 'content-disposition': "attachment; filename*=UTF-8''Traspaso%20Especial.pdf" },
    });

    const result = await transferService.exportPdf(7);

    expect(result.filename).toBe('Traspaso Especial.pdf');
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(transferService.exportPdf(7)).rejects.toThrow('Network Error');
  });
});