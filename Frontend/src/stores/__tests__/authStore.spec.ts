import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';

// Mocks
vi.mock('@/router/index', () => ({
  default: { push: vi.fn() }
}));

vi.mock('axios', () => ({
  default: {
    get: vi.fn(),
    defaults: { headers: { common: {} } }
  }
}));

vi.mock('jwt-decode', () => ({
  jwtDecode: vi.fn()
}));

vi.mock('@/utils/string', () => ({
  normalize: (text: string) => text.toLowerCase()
}));

import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import router from '@/router/index';
import { useAuthStore } from '../authStore';

const mockJwtDecode = vi.mocked(jwtDecode);
const mockAxiosGet  = vi.mocked(axios.get);

const mockDecoded = {
  userId:    '1',
  userName:  'sergio',
  role:      'Admin',
  storeName: 'Tienda Central',
  storeType: 'Principal',
  storeId:   '1',
  nbf:       0,
  exp:       Math.floor(Date.now() / 1000) + 3600, // válido por 1 hora
  iss:       'waresoft',
  aud:       'waresoft',
};

const mockPermissions = [
  { module: 'productos', action: 'crear' },
  { module: 'productos', action: 'editar' },
];

beforeEach(() => {
  setActivePinia(createPinia());
  localStorage.clear();
  vi.clearAllMocks();
  axios.defaults.headers.common = {};
  mockJwtDecode.mockReturnValue(mockDecoded as any);
});

describe('useAuthStore - getters', () => {

  it('isAuthenticated retorna false cuando no hay token', () => {
    const store = useAuthStore();
    expect(store.isAuthenticated).toBe(false);
  });

  it('isAuthenticated retorna true cuando hay token', () => {
    const store = useAuthStore();
    store.token = 'fake-token';
    expect(store.isAuthenticated).toBe(true);
  });

  it('getCurrentUser retorna null cuando no hay usuario', () => {
    const store = useAuthStore();
    expect(store.getCurrentUser).toBeNull();
  });

  it('hasPermission retorna false cuando no hay usuario', () => {
    const store = useAuthStore();
    expect(store.hasPermission('productos', 'crear')).toBe(false);
  });

  it('hasPermission retorna true cuando el usuario tiene el permiso', () => {
    const store = useAuthStore();
    store.currentUser = {
      userId: 1, userName: 'sergio', role: 'Admin',
      storeId: 1, storeName: 'Tienda', storeType: 'Principal',
      permissions: mockPermissions,
    };
    expect(store.hasPermission('productos', 'crear')).toBe(true);
  });

  it('hasPermission retorna false cuando el usuario no tiene el permiso', () => {
    const store = useAuthStore();
    store.currentUser = {
      userId: 1, userName: 'sergio', role: 'Admin',
      storeId: 1, storeName: 'Tienda', storeType: 'Principal',
      permissions: mockPermissions,
    };
    expect(store.hasPermission('productos', 'eliminar')).toBe(false);
  });
});

describe('useAuthStore - initializeAuth', () => {

  it('setea authInitialized true cuando no hay token', async () => {
    const store = useAuthStore();
    await store.initializeAuth();
    expect(store.authInitialized).toBe(true);
    expect(store.token).toBeNull();
  });

  it('limpia sesión cuando el token está expirado', async () => {
    mockJwtDecode.mockReturnValue({ ...mockDecoded, exp: 0 } as any);
    localStorage.setItem('token', 'expired-token');

    const store = useAuthStore();
    await store.initializeAuth();

    expect(store.token).toBeNull();
    expect(store.currentUser).toBeNull();
    expect(localStorage.getItem('token')).toBeNull();
    expect(store.authInitialized).toBe(true);
  });

  it('restaura sesión desde localStorage cuando el token es válido', async () => {
    localStorage.setItem('token', 'valid-token');
    localStorage.setItem('user', JSON.stringify({
      userId: 1, userName: 'sergio', role: 'Admin',
      storeId: 1, storeName: 'Tienda', storeType: 'Principal',
      permissions: mockPermissions,
    }));

    const store = useAuthStore();
    await store.initializeAuth();

    expect(store.token).toBe('valid-token');
    expect(store.currentUser?.userName).toBe('sergio');
    expect(store.authInitialized).toBe(true);
  });

  it('restaura usuario sin permisos si no hay caché de usuario', async () => {
    localStorage.setItem('token', 'valid-token');

    const store = useAuthStore();
    await store.initializeAuth();

    expect(store.currentUser?.permissions).toEqual([]);
    expect(store.authInitialized).toBe(true);
  });
});

describe('useAuthStore - saveToken', () => {

  it('guarda el token y carga permisos del servidor', async () => {
    mockAxiosGet.mockResolvedValue({
      data: { isSuccess: true, data: mockPermissions }
    });

    const store = useAuthStore();
    await store.saveToken('new-token');

    expect(store.token).toBe('new-token');
    expect(localStorage.getItem('token')).toBe('new-token');
    expect(store.currentUser?.permissions).toEqual(mockPermissions);
  });

  it('guarda usuario sin permisos si el servidor falla', async () => {
    mockAxiosGet.mockRejectedValue(new Error('Network Error'));

    const store = useAuthStore();
    await store.saveToken('new-token');

    expect(store.currentUser?.permissions).toEqual([]);
  });

  it('limpia sesión si jwtDecode falla', async () => {
    mockJwtDecode.mockImplementation(() => { throw new Error('invalid token'); });

    const store = useAuthStore();
    await expect(store.saveToken('bad-token')).rejects.toThrow();

    expect(store.token).toBeNull();
    expect(localStorage.getItem('token')).toBeNull();
  });
});

describe('useAuthStore - logout', () => {

  it('limpia token y usuario', () => {
    const store = useAuthStore();
    store.token = 'fake-token';
    store.currentUser = {
      userId: 1, userName: 'sergio', role: 'Admin',
      storeId: 1, storeName: 'Tienda', storeType: 'Principal',
      permissions: [],
    };

    store.logout();

    expect(store.token).toBeNull();
    expect(store.currentUser).toBeNull();
  });

  it('limpia localStorage al hacer logout', () => {
    localStorage.setItem('token', 'fake-token');
    localStorage.setItem('user', '{}');

    const store = useAuthStore();
    store.logout();

    expect(localStorage.getItem('token')).toBeNull();
    expect(localStorage.getItem('user')).toBeNull();
  });

  it('redirige al login al hacer logout', () => {
    const store = useAuthStore();
    store.logout();

    expect(router.push).toHaveBeenCalledWith({ name: 'login' });
  });
});