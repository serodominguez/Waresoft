import { describe, it, expect, vi, beforeEach } from 'vitest';
import axios from 'axios';
import { productService } from '../productService';
import type { ProductStats, Product } from '@/interfaces/productInterface';
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

const mockStats: ProductStats = {
  totalActive:  80,
  newThisMonth: 5,
};

const mockProduct: Product = {
  idProduct:      1,
  code:           'PRD-001',
  description:    'Producto Test',
  material:       'Algodón',
  color:          'Azul',
  unitMeasure:    'UND',
  image:          'imagen.jpg',
  idBrand:        1,
  brandName:      'Marca A',
  idCategory:     2,
  categoryName:   'Categoría A',
  auditCreateDate: '2024-01-01',
  statusProduct:  'Activo',
};

// ─── getStats ─────────────────────────────────────────────────────────────────

describe('ProductService.getStats', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: { data: mockStats } });

    await productService.getStats();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Product/Stats');
  });

  it('retorna los datos correctamente', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: { data: mockStats } });

    const result = await productService.getStats();

    expect(result).toEqual(mockStats);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(productService.getStats()).rejects.toThrow('Network Error');
  });
});

// ─── registerProduct ──────────────────────────────────────────────────────────

describe('ProductService.registerProduct', () => {

  it('llama a axios.post con el endpoint y FormData correctos', async () => {
    const formData = new FormData();
    formData.append('description', 'Producto Test');
    mockedAxios.post.mockResolvedValueOnce({ data: makeBaseResponse(mockProduct) });

    await productService.registerProduct(formData);

    expect(mockedAxios.post).toHaveBeenCalledWith('api/Product/Register', formData);
  });

  it('retorna el BaseResponse completo', async () => {
    const formData   = new FormData();
    const response   = makeBaseResponse(mockProduct);
    mockedAxios.post.mockResolvedValueOnce({ data: response });

    const result = await productService.registerProduct(formData);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.post.mockRejectedValueOnce(new Error('Network Error'));

    await expect(productService.registerProduct(new FormData())).rejects.toThrow('Network Error');
  });
});

// ─── editProduct ──────────────────────────────────────────────────────────────

describe('ProductService.editProduct', () => {

  it('llama a axios.put con el endpoint y FormData correctos', async () => {
    const formData = new FormData();
    formData.append('description', 'Producto Editado');
    mockedAxios.put.mockResolvedValueOnce({ data: makeBaseResponse(mockProduct) });

    await productService.editProduct(1, formData);

    expect(mockedAxios.put).toHaveBeenCalledWith('api/Product/Edit/1', formData);
  });

  it('retorna el BaseResponse completo', async () => {
    const formData = new FormData();
    const response = makeBaseResponse(mockProduct);
    mockedAxios.put.mockResolvedValueOnce({ data: response });

    const result = await productService.editProduct(1, formData);

    expect(result).toEqual(response);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.put.mockRejectedValueOnce(new Error('Network Error'));

    await expect(productService.editProduct(1, new FormData())).rejects.toThrow('Network Error');
  });
});

// ─── generateProductCode ──────────────────────────────────────────────────────

describe('ProductService.generateProductCode', () => {

  it('llama al endpoint correcto', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: makeBaseResponse('PRD-999') });

    await productService.generateProductCode();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Sequence/Product-Code');
  });

  it('retorna el BaseResponse con el código generado', async () => {
    const response = makeBaseResponse('PRD-999');
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    const result = await productService.generateProductCode();

    expect(result).toEqual(response);
    expect(result.data).toBe('PRD-999');
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(productService.generateProductCode()).rejects.toThrow('Network Error');
  });
});

// ─── generateBarcodePdf ───────────────────────────────────────────────────────

describe('ProductService.generateBarcodePdf', () => {

  it('llama a axios.post con el endpoint, payload y responseType blob', async () => {
    const blob = new Blob(['pdf'], { type: 'application/pdf' });
    mockedAxios.post.mockResolvedValueOnce({ data: blob });

    await productService.generateBarcodePdf(1, 5);

    expect(mockedAxios.post).toHaveBeenCalledWith(
      'api/Product/BarcodePdf',
      { idProduct: 1, quantity: 5 },
      { responseType: 'blob' }
    );
  });

  it('retorna el Blob directamente', async () => {
    const blob = new Blob(['pdf'], { type: 'application/pdf' });
    mockedAxios.post.mockResolvedValueOnce({ data: blob });

    const result = await productService.generateBarcodePdf(1, 5);

    expect(result).toBe(blob);
  });

  it('envía el idProduct y quantity correctamente', async () => {
    const blob = new Blob(['pdf']);
    mockedAxios.post.mockResolvedValueOnce({ data: blob });

    await productService.generateBarcodePdf(42, 10);

    const payload = mockedAxios.post.mock.calls[0][1] as { idProduct: number; quantity: number };
    expect(payload.idProduct).toBe(42);
    expect(payload.quantity).toBe(10);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.post.mockRejectedValueOnce(new Error('Network Error'));

    await expect(productService.generateBarcodePdf(1, 5)).rejects.toThrow('Network Error');
  });
});