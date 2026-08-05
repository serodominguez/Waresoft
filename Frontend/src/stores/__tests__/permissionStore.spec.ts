import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';

vi.mock('@/services/permissionService', () => ({
  fetchPermissionsByRole: vi.fn(),
  updatePermissions:      vi.fn(),
}));

import { fetchPermissionsByRole, updatePermissions } from '@/services/permissionService';
import { usePermissionStore } from '../permissionStore';

const baseResponse = { totalRecords: null, message: '', errors: null };

const mockPermissions = [
  { idPermission: 1, idRole: 1, idModule: 1, moduleName: 'Productos', idAction: 1, actionName: 'crear',    status: true  },
  { idPermission: 2, idRole: 1, idModule: 1, moduleName: 'Productos', idAction: 2, actionName: 'leer',     status: true  },
  { idPermission: 3, idRole: 1, idModule: 1, moduleName: 'Productos', idAction: 3, actionName: 'editar',   status: false },
  { idPermission: 4, idRole: 1, idModule: 1, moduleName: 'Productos', idAction: 4, actionName: 'eliminar', status: false },
  { idPermission: 5, idRole: 1, idModule: 1, moduleName: 'Productos', idAction: 5, actionName: 'descargar',status: true  },
  { idPermission: 6, idRole: 1, idModule: 2, moduleName: 'Clientes',  idAction: 1, actionName: 'crear',    status: false },
  { idPermission: 7, idRole: 1, idModule: 2, moduleName: 'Clientes',  idAction: 2, actionName: 'leer',     status: true  },
];

beforeEach(() => {
  setActivePinia(createPinia());
  vi.clearAllMocks();

  vi.mocked(fetchPermissionsByRole).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: mockPermissions
  });
  vi.mocked(updatePermissions).mockResolvedValue({
    ...baseResponse, isSuccess: true, data: null
  });
});

describe('usePermissionStore', () => {

  // ── Estado inicial ────────────────────────────────────────────────────────
  it('inicializa con estado vacío', () => {
    const store = usePermissionStore();

    expect(store.items).toEqual([]);
    expect(store.loading).toBe(false);
  });

  // ── fetchPermissionsByRole ────────────────────────────────────────────────
  it('fetchPermissionsByRole carga los permisos', async () => {
    const store = usePermissionStore();

    await store.fetchPermissionsByRole(1);

    expect(store.items).toEqual(mockPermissions);
  });

  it('fetchPermissionsByRole llama al servicio con el roleId correcto', async () => {
    const store = usePermissionStore();

    await store.fetchPermissionsByRole(5);

    expect(fetchPermissionsByRole).toHaveBeenCalledWith(5);
  });

  it('fetchPermissionsByRole lanza error cuando isSuccess es false', async () => {
    vi.mocked(fetchPermissionsByRole).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'Rol no encontrado', data: []
    });
    const store = usePermissionStore();

    await expect(store.fetchPermissionsByRole(99)).rejects.toThrow('Rol no encontrado');
  });

  it('fetchPermissionsByRole setea loading false aunque falle', async () => {
    vi.mocked(fetchPermissionsByRole).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'Error', data: []
    });
    const store = usePermissionStore();

    await store.fetchPermissionsByRole(1).catch(() => {});

    expect(store.loading).toBe(false);
  });

  // ── updatePermissions ─────────────────────────────────────────────────────
  it('updatePermissions llama al servicio con los permisos correctos', async () => {
    const store = usePermissionStore();
    const updates = [{ idPermission: 1, status: false }];

    await store.updatePermissions(updates);

    expect(updatePermissions).toHaveBeenCalledWith(updates);
  });

  it('updatePermissions lanza error cuando isSuccess es false', async () => {
    vi.mocked(updatePermissions).mockResolvedValue({
      ...baseResponse, isSuccess: false, message: 'Error al actualizar', data: null
    });
    const store = usePermissionStore();

    await expect(store.updatePermissions([{ idPermission: 1, status: false }]))
      .rejects.toThrow('Error al actualizar');
  });

  // ── clearPermissions ──────────────────────────────────────────────────────
  it('clearPermissions limpia los items', async () => {
    const store = usePermissionStore();
    await store.fetchPermissionsByRole(1);

    store.clearPermissions();

    expect(store.items).toEqual([]);
  });

  // ── permissionsByModule computed ──────────────────────────────────────────
  it('permissionsByModule agrupa permisos por módulo', async () => {
    const store = usePermissionStore();
    await store.fetchPermissionsByRole(1);

    const grouped = store.permissionsByModule;

    expect(grouped).toHaveLength(2);
    expect(grouped.map(g => g.module)).toContain('Productos');
    expect(grouped.map(g => g.module)).toContain('Clientes');
  });

  it('permissionsByModule mapea los status correctamente', async () => {
    const store = usePermissionStore();
    await store.fetchPermissionsByRole(1);

    const productos = store.permissionsByModule.find(g => g.module === 'Productos');

    expect(productos?.permissions.crear).toBe(true);
    expect(productos?.permissions.leer).toBe(true);
    expect(productos?.permissions.editar).toBe(false);
    expect(productos?.permissions.eliminar).toBe(false);
    expect(productos?.permissions.descargar).toBe(true);
  });

  it('permissionsByModule retorna array vacío cuando no hay permisos', () => {
    const store = usePermissionStore();

    expect(store.permissionsByModule).toEqual([]);
  });

  it('permissions computed refleja items', async () => {
    const store = usePermissionStore();
    await store.fetchPermissionsByRole(1);

    expect(store.permissions).toEqual(mockPermissions);
  });
});