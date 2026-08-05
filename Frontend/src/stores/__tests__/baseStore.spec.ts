import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import { createBaseStore } from '../baseStore';

const makeService = (overrides = {}) => ({
  fetchAll: vi.fn().mockResolvedValue({ isSuccess: true, data: [], totalRecords: 0 }),
  select: vi.fn().mockResolvedValue({ isSuccess: true, data: [] }),
  fetchById: vi.fn().mockResolvedValue({ isSuccess: true, data: { id: 1 } }),
  downloadExcel: vi.fn().mockResolvedValue(undefined),
  downloadPdf: vi.fn().mockResolvedValue(undefined),
  create: vi.fn().mockResolvedValue({ isSuccess: true }),
  update: vi.fn().mockResolvedValue({ isSuccess: true }),
  enable: vi.fn().mockResolvedValue({ isSuccess: true }),
  disable: vi.fn().mockResolvedValue({ isSuccess: true }),
  remove: vi.fn().mockResolvedValue({ isSuccess: true }),
  ...overrides,
});

beforeEach(() => {
  setActivePinia(createPinia());
});

describe('baseStore', () => {

  // ── Estado inicial ────────────────────────────────────────────────────────
  it('inicializa con estado vacío', () => {
    const service = makeService();
    const useStore = createBaseStore('test', service);
    const store = useStore();

    expect(store.items).toEqual([]);
    expect(store.selectedItem).toBeNull();
    expect(store.totalItems).toBe(0);
    expect(store.loading).toBe(false);
  });

  // ── fetchAll ──────────────────────────────────────────────────────────────
  it('fetchAll carga items y totalItems', async () => {
    const items = [{ id: 1 }, { id: 2 }];
    const service = makeService({
      fetchAll: vi.fn().mockResolvedValue({ isSuccess: true, data: items, totalRecords: 2 })
    });
    const store = createBaseStore('test-fetchAll', service)();

    await store.fetchAll({ pageNumber: 1, pageSize: 10 });

    expect(store.items).toEqual(items);
    expect(store.totalItems).toBe(2);
  });

  it('fetchAll guarda los params en lastFilterParams', async () => {
    const service = makeService();
    const store = createBaseStore('test-params', service)();

    await store.fetchAll({ pageNumber: 2, pageSize: 25 });

    expect(store.lastFilterParams).toEqual({ pageNumber: 2, pageSize: 25 });
  });

  it('fetchAll lanza error cuando isSuccess es false', async () => {
    const service = makeService({
      fetchAll: vi.fn().mockResolvedValue({ isSuccess: false, message: 'Error del servidor' })
    });
    const store = createBaseStore('test-error', service)();

    await expect(store.fetchAll()).rejects.toThrow('Error del servidor');
  });

  it('fetchAll setea loading en false aunque falle', async () => {
    const service = makeService({
      fetchAll: vi.fn().mockResolvedValue({ isSuccess: false, message: 'Error' })
    });
    const store = createBaseStore('test-loading', service)();

    await store.fetchAll().catch(() => { });

    expect(store.loading).toBe(false);
  });

  // ── fetchForSelect ────────────────────────────────────────────────────────
  it('fetchForSelect carga items para select', async () => {
    const items = [{ id: 1, name: 'A' }];
    const service = makeService({
      select: vi.fn().mockResolvedValue({ isSuccess: true, data: items })
    });
    const store = createBaseStore('test-select', service)();

    await store.fetchForSelect();

    expect(store.items).toEqual(items);
  });

  it('fetchForSelect lanza error cuando isSuccess es false', async () => {
    const service = makeService({
      select: vi.fn().mockResolvedValue({ isSuccess: false, message: 'Sin datos' })
    });
    const store = createBaseStore('test-select-error', service)();

    await expect(store.fetchForSelect()).rejects.toThrow('Sin datos');
  });

  // ── fetchById ─────────────────────────────────────────────────────────────
  it('fetchById setea selectedItem', async () => {
    const item = { id: 5, name: 'Producto' };
    const service = makeService({
      fetchById: vi.fn().mockResolvedValue({ isSuccess: true, data: item })
    });
    const store = createBaseStore('test-byId', service)();

    await store.fetchById(5);

    expect(store.selectedItem).toEqual(item);
  });

  it('fetchById lanza error cuando isSuccess es false', async () => {
    const service = makeService({
      fetchById: vi.fn().mockResolvedValue({ isSuccess: false, message: 'No encontrado' })
    });
    const store = createBaseStore('test-byId-error', service)();

    await expect(store.fetchById(99)).rejects.toThrow('No encontrado');
  });

  // ── register ──────────────────────────────────────────────────────────────
  it('register llama create y recarga la lista', async () => {
    const service = makeService();
    const store = createBaseStore('test-register', service)();

    await store.register({ name: 'Nuevo' } as any);

    expect(service.create).toHaveBeenCalledWith({ name: 'Nuevo' });
    expect(service.fetchAll).toHaveBeenCalledTimes(1);
  });

  it('register lanza error cuando isSuccess es false', async () => {
    const service = makeService({
      create: vi.fn().mockResolvedValue({ isSuccess: false, message: 'Ya existe' })
    });
    const store = createBaseStore('test-register-error', service)();

    await expect(store.register({} as any)).rejects.toThrow('Ya existe');
  });

  // ── edit ──────────────────────────────────────────────────────────────────
  it('edit llama update y recarga la lista', async () => {
    const service = makeService();
    const store = createBaseStore('test-edit', service)();

    await store.edit(1, { name: 'Editado' } as any);

    expect(service.update).toHaveBeenCalledWith(1, { name: 'Editado' });
    expect(service.fetchAll).toHaveBeenCalledTimes(1);
  });

  // ── enable / disable / remove ─────────────────────────────────────────────
  it('enable llama service.enable y recarga la lista', async () => {
    const service = makeService();
    const store = createBaseStore('test-enable', service)();

    await store.enable(1);

    expect(service.enable).toHaveBeenCalledWith(1);
    expect(service.fetchAll).toHaveBeenCalledTimes(1);
  });

  it('disable llama service.disable y recarga la lista', async () => {
    const service = makeService();
    const store = createBaseStore('test-disable', service)();

    await store.disable(1);

    expect(service.disable).toHaveBeenCalledWith(1);
    expect(service.fetchAll).toHaveBeenCalledTimes(1);
  });

  it('remove llama service.remove y recarga la lista', async () => {
    const service = makeService();
    const store = createBaseStore('test-remove', service)();

    await store.remove(1);

    expect(service.remove).toHaveBeenCalledWith(1);
    expect(service.fetchAll).toHaveBeenCalledTimes(1);
  });

  it('remove lanza error cuando isSuccess es false', async () => {
    const service = makeService({
      remove: vi.fn().mockResolvedValue({ isSuccess: false, message: 'No se puede eliminar' })
    });
    const store = createBaseStore('test-remove-error', service)();

    await expect(store.remove(1)).rejects.toThrow('No se puede eliminar');
  });

  // ── Descargas ─────────────────────────────────────────────────────────────
  it('downloadExcel llama service.downloadExcel con lastFilterParams', async () => {
    const service = makeService();
    const store = createBaseStore('test-excel', service)();
    await store.fetchAll({ pageNumber: 1, pageSize: 10 });

    await store.downloadExcel();

    expect(service.downloadExcel).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  it('downloadPdf llama service.downloadPdf con lastFilterParams', async () => {
    const service = makeService();
    const store = createBaseStore('test-pdf', service)();
    await store.fetchAll({ pageNumber: 1, pageSize: 10 });

    await store.downloadPdf();

    expect(service.downloadPdf).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
  });

  // ── Computed ──────────────────────────────────────────────────────────────
  it('list computed refleja items', async () => {
    const items = [{ id: 1 }];
    const service = makeService({
      fetchAll: vi.fn().mockResolvedValue({ isSuccess: true, data: items, totalRecords: 1 })
    });
    const store = createBaseStore('test-computed', service)();

    await store.fetchAll();

    expect(store.list).toEqual(items);
    expect(store.total).toBe(1);
  });
});