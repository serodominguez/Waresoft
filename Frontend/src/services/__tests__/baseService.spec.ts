import { describe, it, expect, vi, beforeEach } from 'vitest';
import axios from 'axios';
import { BaseService } from '../baseService';
import type { BaseResponse, FilterParams } from '@/interfaces/baseInterface';

// ─── Mock de axios ────────────────────────────────────────────────────────────

vi.mock('axios');
const mockedAxios = vi.mocked(axios);

// ─── Mock de DOM (triggerDownload usa createElement y createObjectURL) ────────

const mockClick = vi.fn();
const mockAppendChild = vi.fn();
const mockRemoveChild = vi.fn();
const mockCreateObjectURL = vi.fn(() => 'blob:mock-url');
const mockRevokeObjectURL = vi.fn();

beforeEach(() => {
  vi.clearAllMocks();

  // DOM
  vi.spyOn(document, 'createElement').mockReturnValue({
    href: '',
    setAttribute: vi.fn(),
    click: mockClick,
    parentNode: { removeChild: mockRemoveChild },
  } as unknown as HTMLElement);

  vi.spyOn(document.body, 'appendChild').mockImplementation(mockAppendChild);

  window.URL.createObjectURL = mockCreateObjectURL;
  window.URL.revokeObjectURL = mockRevokeObjectURL;
});

// ─── Helper ───────────────────────────────────────────────────────────────────

type Entity = { id: number; name: string };

const makeService = (overrides = {}) =>
  new BaseService<Entity>({
    endpoint: 'Category',
    downloadFileName: 'categorias',
    ...overrides,
  });

const makeResponse = <T>(data: T): BaseResponse<T> => ({
  isSuccess: true,
  message: 'OK',
  data,
  totalRecords: 0,
  errors: null,
});

// ─── Tests ────────────────────────────────────────────────────────────────────

describe('BaseService.buildParams', () => {

  it('usa valores por defecto cuando no se pasan parámetros', () => {
    const service = makeService();
    // @ts-expect-error método protegido
    const params = service.buildParams();

    expect(params).toEqual({
      NumberPage: 1,
      NumberRecordsPage: 10,
      Order: 'desc',
      Sort: 'Id',
      StateFilter: 1,
    });
  });

  it('mapea correctamente los parámetros recibidos', () => {
    const service = makeService();
    const input: FilterParams = {
      pageNumber: 2,
      pageSize: 25,
      order: 'asc',
      sort: 'Name',
      stateFilter: 0,
      textFilter: 'test',
      numberFilter: 5,
      startDate: '2024-01-01',
      endDate: '2024-12-31',
    };

    // @ts-expect-error método protegido
    const params = service.buildParams(input);

    expect(params.NumberPage).toBe(2);
    expect(params.NumberRecordsPage).toBe(25);
    expect(params.Order).toBe('asc');
    expect(params.Sort).toBe('Name');
    expect(params.StateFilter).toBe(0);
    expect(params.TextFilter).toBe('test');
    expect(params.NumberFilter).toBe(5);
    expect(params.StartDate).toBe('2024-01-01');
    expect(params.EndDate).toBe('2024-12-31');
  });

  it('no incluye TextFilter ni NumberFilter si solo viene uno de los dos', () => {
    const service = makeService();
    // @ts-expect-error método protegido
    const params = service.buildParams({ textFilter: 'solo texto' });

    expect(params.TextFilter).toBeUndefined();
    expect(params.NumberFilter).toBeUndefined();
  });
});

describe('BaseService.fetchAll', () => {

  it('llama a axios.get con el endpoint y params correctos', async () => {
    const service = makeService();
    const response = makeResponse([{ id: 1, name: 'Cat A' }]);
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    await service.fetchAll();

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/Category',
      expect.objectContaining({
        params: expect.objectContaining({ Download: false }),
      })
    );
  });

  it('retorna el data de la respuesta', async () => {
    const service = makeService();
    const response = makeResponse([{ id: 1, name: 'Cat A' }]);
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    const result = await service.fetchAll();

    expect(result).toEqual(response);
  });

  it('en modo download llama con responseType blob y retorna Blob', async () => {
    const service = makeService();
    const blob = new Blob(['data']);
    mockedAxios.get.mockResolvedValueOnce({ data: blob });

    const result = await service.fetchAll({}, true);

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/Category',
      expect.objectContaining({ responseType: 'blob' })
    );
    expect(result).toBe(blob);
  });
});

describe('BaseService.fetchById', () => {

  it('llama a axios.get con el id en la URL', async () => {
    const service = makeService();
    const response = makeResponse({ id: 1, name: 'Cat A' });
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    await service.fetchById(1);

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Category/1');
  });

  it('retorna el data de la respuesta', async () => {
    const service = makeService();
    const response = makeResponse({ id: 1, name: 'Cat A' });
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    const result = await service.fetchById(1);

    expect(result).toEqual(response);
  });
});

describe('BaseService.select', () => {

  it('llama al selectEndpoint por defecto', async () => {
    const service = makeService();
    const response = makeResponse([{ id: 1, name: 'Cat A' }]);
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    await service.select();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Category/Select');
  });

  it('usa selectEndpoint personalizado si se configura', async () => {
    const service = makeService({ selectEndpoint: 'Category/CustomSelect' });
    const response = makeResponse([]);
    mockedAxios.get.mockResolvedValueOnce({ data: response });

    await service.select();

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Category/CustomSelect');
  });
});

describe('BaseService.create', () => {

  it('llama a axios.post con el endpoint Register por defecto', async () => {
    const service = makeService();
    const entity = { id: 0, name: 'Nueva' };
    const response = makeResponse(entity);
    mockedAxios.post.mockResolvedValueOnce({ data: response });

    await service.create(entity);

    expect(mockedAxios.post).toHaveBeenCalledWith('api/Category/Register', entity);
  });

  it('usa customEndpoint create si se configura', async () => {
    const service = makeService({ customEndpoints: { create: 'Category/CustomCreate' } });
    const entity = { id: 0, name: 'Nueva' };
    const response = makeResponse(entity);
    mockedAxios.post.mockResolvedValueOnce({ data: response });

    await service.create(entity);

    expect(mockedAxios.post).toHaveBeenCalledWith('api/Category/CustomCreate', entity);
  });
});

describe('BaseService.update', () => {

  it('llama a axios.put con el endpoint Edit/{id} por defecto', async () => {
    const service = makeService();
    const entity = { id: 1, name: 'Editada' };
    const response = makeResponse(entity);
    mockedAxios.put.mockResolvedValueOnce({ data: response });

    await service.update(1, entity);

    expect(mockedAxios.put).toHaveBeenCalledWith('api/Category/Edit/1', entity);
  });
});

describe('BaseService.enable / disable / remove', () => {

  it('enable llama a axios.put con Enable/{id}', async () => {
    const service = makeService();
    mockedAxios.put.mockResolvedValueOnce({ data: makeResponse(undefined) });

    await service.enable(3);

    expect(mockedAxios.put).toHaveBeenCalledWith('api/Category/Enable/3', {});
  });

  it('disable llama a axios.put con Disable/{id}', async () => {
    const service = makeService();
    mockedAxios.put.mockResolvedValueOnce({ data: makeResponse(undefined) });

    await service.disable(3);

    expect(mockedAxios.put).toHaveBeenCalledWith('api/Category/Disable/3', {});
  });

  it('remove llama a axios.put con Remove/{id}', async () => {
    const service = makeService();
    mockedAxios.put.mockResolvedValueOnce({ data: makeResponse(undefined) });

    await service.remove(3);

    expect(mockedAxios.put).toHaveBeenCalledWith('api/Category/Remove/3', {});
  });

  it('usa customEndpoints para enable/disable/remove si se configuran', async () => {
    const service = makeService({
      customEndpoints: {
        enable: 'Category/Activate',
        disable: 'Category/Deactivate',
        remove: 'Category/Delete',
      },
    });
    mockedAxios.put.mockResolvedValue({ data: makeResponse(undefined) });

    await service.enable(1);
    expect(mockedAxios.put).toHaveBeenCalledWith('api/Category/Activate', {});

    await service.disable(1);
    expect(mockedAxios.put).toHaveBeenCalledWith('api/Category/Deactivate', {});

    await service.remove(1);
    expect(mockedAxios.put).toHaveBeenCalledWith('api/Category/Delete', {});
  });
});

describe('BaseService.downloadExcel', () => {

  it('llama a fetchAll en modo download y dispara la descarga', async () => {
    const service = makeService();
    const blob = new Blob(['excel']);
    mockedAxios.get.mockResolvedValueOnce({ data: blob });

    await service.downloadExcel();

    expect(mockClick).toHaveBeenCalled();
    expect(mockCreateObjectURL).toHaveBeenCalledWith(blob);
    expect(mockRevokeObjectURL).toHaveBeenCalledWith('blob:mock-url');
  });

  it('el nombre del archivo incluye el downloadFileName y la fecha', async () => {
    const service = makeService();
    const blob = new Blob(['excel']);
    mockedAxios.get.mockResolvedValueOnce({ data: blob });

    const mockSetAttribute = vi.fn();
    vi.spyOn(document, 'createElement').mockReturnValue({
      href: '',
      setAttribute: mockSetAttribute,
      click: mockClick,
      parentNode: { removeChild: mockRemoveChild },
    } as unknown as HTMLElement);

    await service.downloadExcel();

    const date = new Date().toISOString().split('T')[0];
    expect(mockSetAttribute).toHaveBeenCalledWith('download', `categorias_${date}.xlsx`);
  });
});

describe('BaseService.downloadPdf', () => {

  it('llama a axios.get con DownloadType pdf y dispara la descarga', async () => {
    const service = makeService();
    mockedAxios.get.mockResolvedValueOnce({ data: new Uint8Array([1, 2, 3]) });

    await service.downloadPdf();

    expect(mockedAxios.get).toHaveBeenCalledWith(
      'api/Category',
      expect.objectContaining({
        responseType: 'blob',
        params: expect.objectContaining({ DownloadType: 'pdf', Download: true }),
      })
    );
    expect(mockClick).toHaveBeenCalled();
  });
});