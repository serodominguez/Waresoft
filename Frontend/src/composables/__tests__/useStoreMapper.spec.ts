import { describe, it, expect, vi, beforeEach } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';

// Mock de todos los stores
vi.mock('@/stores/brandStore',        () => ({ useBrandStore:        vi.fn() }));
vi.mock('@/stores/categoryStore',     () => ({ useCategoryStore:     vi.fn() }));
vi.mock('@/stores/customerStore',     () => ({ useCustomerStore:     vi.fn() }));
vi.mock('@/stores/goodsIssueStore',   () => ({ useGoodsIssueStore:   vi.fn() }));
vi.mock('@/stores/goodsReceiptStore', () => ({ useGoodsReceiptStore: vi.fn() }));
vi.mock('@/stores/moduleStore',       () => ({ useModuleStore:       vi.fn() }));
vi.mock('@/stores/productStore',      () => ({ useProductStore:      vi.fn() }));
vi.mock('@/stores/roleStore',         () => ({ useRoleStore:         vi.fn() }));
vi.mock('@/stores/storeStore',        () => ({ useStoreStore:        vi.fn() }));
vi.mock('@/stores/supplierStore',     () => ({ useSupplierStore:     vi.fn() }));
vi.mock('@/stores/transferStore',     () => ({ useTransferStore:     vi.fn() }));
vi.mock('@/stores/userStore',         () => ({ useUserStore:         vi.fn() }));

import { useBrandStore }        from '@/stores/brandStore';
import { useCategoryStore }     from '@/stores/categoryStore';
import { useCustomerStore }     from '@/stores/customerStore';
import { useGoodsIssueStore }   from '@/stores/goodsIssueStore';
import { useGoodsReceiptStore } from '@/stores/goodsReceiptStore';
import { useModuleStore }       from '@/stores/moduleStore';
import { useProductStore }      from '@/stores/productStore';
import { useRoleStore }         from '@/stores/roleStore';
import { useStoreStore }        from '@/stores/storeStore';
import { useSupplierStore }     from '@/stores/supplierStore';
import { useTransferStore }     from '@/stores/transferStore';
import { useUserStore }         from '@/stores/userStore';
import { useStoreMapper }       from '../useStoreMapper';

const makeMockStore = (overrides = {}) => ({
  remove:  vi.fn().mockResolvedValue({ isSuccess: true }),
  enable:  vi.fn().mockResolvedValue({ isSuccess: true }),
  disable: vi.fn().mockResolvedValue({ isSuccess: true }),
  cancel:  vi.fn().mockResolvedValue({ isSuccess: true }),
  ...overrides,
});

beforeEach(() => {
  setActivePinia(createPinia());

  const mockStore = makeMockStore();

  vi.mocked(useBrandStore).mockReturnValue(mockStore as any);
  vi.mocked(useCategoryStore).mockReturnValue(mockStore as any);
  vi.mocked(useCustomerStore).mockReturnValue(mockStore as any);
  vi.mocked(useGoodsIssueStore).mockReturnValue(mockStore as any);
  vi.mocked(useGoodsReceiptStore).mockReturnValue(mockStore as any);
  vi.mocked(useModuleStore).mockReturnValue(mockStore as any);
  vi.mocked(useProductStore).mockReturnValue(mockStore as any);
  vi.mocked(useRoleStore).mockReturnValue(mockStore as any);
  vi.mocked(useStoreStore).mockReturnValue(mockStore as any);
  vi.mocked(useSupplierStore).mockReturnValue(mockStore as any);
  vi.mocked(useTransferStore).mockReturnValue(mockStore as any);
  vi.mocked(useUserStore).mockReturnValue(mockStore as any);
});

describe('useStoreMapper', () => {

  it('getStoreAction retorna una función para eliminar', () => {
    const { getStoreAction } = useStoreMapper();

    const fn = getStoreAction('brand', 'eliminar');

    expect(typeof fn).toBe('function');
  });

  it('getStoreAction llama remove del store correcto', async () => {
    const brandMock = makeMockStore();
    vi.mocked(useBrandStore).mockReturnValue(brandMock as any);

    const { getStoreAction } = useStoreMapper();
    const fn = getStoreAction('brand', 'eliminar');
    await fn(1);

    expect(brandMock.remove).toHaveBeenCalledWith(1);
  });

  it('getStoreAction llama enable para activar', async () => {
    const brandMock = makeMockStore();
    vi.mocked(useBrandStore).mockReturnValue(brandMock as any);

    const { getStoreAction } = useStoreMapper();
    const fn = getStoreAction('brand', 'activar');
    await fn(2);

    expect(brandMock.enable).toHaveBeenCalledWith(2);
  });

  it('getStoreAction llama disable para inactivar', async () => {
    const brandMock = makeMockStore();
    vi.mocked(useBrandStore).mockReturnValue(brandMock as any);

    const { getStoreAction } = useStoreMapper();
    const fn = getStoreAction('brand', 'inactivar');
    await fn(3);

    expect(brandMock.disable).toHaveBeenCalledWith(3);
  });

  it('getStoreAction llama cancel para cancelar', async () => {
    const transferMock = makeMockStore();
    vi.mocked(useTransferStore).mockReturnValue(transferMock as any);

    const { getStoreAction } = useStoreMapper();
    const fn = getStoreAction('transfer', 'cancelar');
    await fn(5);

    expect(transferMock.cancel).toHaveBeenCalledWith(5);
  });

  it('getStoreAction lanza error si el método no existe en el store', () => {
    vi.mocked(useBrandStore).mockReturnValue({} as any);

    const { getStoreAction } = useStoreMapper();

    expect(() => getStoreAction('brand', 'eliminar')).toThrowError(
      'Método "remove" no encontrado en el store "brand"'
    );
  });

  it('getStoreAction retorna isSuccess true al ejecutar la acción', async () => {
    const { getStoreAction } = useStoreMapper();
    const fn = getStoreAction('product', 'eliminar');

    const result = await fn(10);

    expect(result.isSuccess).toBe(true);
  });
});