import { describe, it, expect, vi, beforeEach } from 'vitest';
import axios from 'axios';
import {
  permissionService,
  fetchPermissionsByRole,
  updatePermissions,
} from '../permissionService';
import type { PermissionResponse } from '@/interfaces/permissionInterface';

// ─── Mock de axios ────────────────────────────────────────────────────────────

vi.mock('axios');
const mockedAxios = vi.mocked(axios);

beforeEach(() => {
  vi.clearAllMocks();
});

// ─── Fixtures ─────────────────────────────────────────────────────────────────

const mockPermissionResponse: PermissionResponse = {
  isSuccess:    true,
  message:      'OK',
  totalRecords: 2,
  errors:       null,
  data: [
    {
      idPermission: 1,
      idRole:       1,
      idModule:     1,
      moduleName:   'Categorías',
      idAction:     1,
      actionName:   'Crear',
      status:       true,
    },
    {
      idPermission: 2,
      idRole:       1,
      idModule:     1,
      moduleName:   'Categorías',
      idAction:     2,
      actionName:   'Editar',
      status:       false,
    },
  ],
};

const mockBatchPayload = [
  { idPermission: 1, status: true  },
  { idPermission: 2, status: false },
];

// ─── fetchByRole ──────────────────────────────────────────────────────────────

describe('PermissionService.fetchByRole', () => {

  it('llama al endpoint correcto con el roleId', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: mockPermissionResponse });

    await permissionService.fetchByRole(3);

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Permission/Role/3');
  });

  it('retorna el PermissionResponse completo', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: mockPermissionResponse });

    const result = await permissionService.fetchByRole(3);

    expect(result).toEqual(mockPermissionResponse);
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

    await expect(permissionService.fetchByRole(3)).rejects.toThrow('Network Error');
  });
});

// ─── updateBatch ──────────────────────────────────────────────────────────────

describe('PermissionService.updateBatch', () => {

  it('llama a axios.put con el endpoint y payload correctos', async () => {
    mockedAxios.put.mockResolvedValueOnce({ data: { isSuccess: true } });

    await permissionService.updateBatch(mockBatchPayload);

    expect(mockedAxios.put).toHaveBeenCalledWith(
      'api/Permission/Update',
      mockBatchPayload
    );
  });

  it('retorna el data de la respuesta', async () => {
    const responseData = { isSuccess: true, message: 'Actualizado' };
    mockedAxios.put.mockResolvedValueOnce({ data: responseData });

    const result = await permissionService.updateBatch(mockBatchPayload);

    expect(result).toEqual(responseData);
  });

  it('envía el array completo de permisos', async () => {
    mockedAxios.put.mockResolvedValueOnce({ data: { isSuccess: true } });

    await permissionService.updateBatch(mockBatchPayload);

    const payload = mockedAxios.put.mock.calls[0][1] as typeof mockBatchPayload;
    expect(payload).toHaveLength(2);
    expect(payload[0]).toEqual({ idPermission: 1, status: true  });
    expect(payload[1]).toEqual({ idPermission: 2, status: false });
  });

  it('propaga el error si axios falla', async () => {
    mockedAxios.put.mockRejectedValueOnce(new Error('Network Error'));

    await expect(permissionService.updateBatch(mockBatchPayload)).rejects.toThrow('Network Error');
  });
});

// ─── exports funcionales ──────────────────────────────────────────────────────

describe('exports funcionales', () => {

  it('fetchPermissionsByRole delega a permissionService.fetchByRole', async () => {
    mockedAxios.get.mockResolvedValueOnce({ data: mockPermissionResponse });

    const result = await fetchPermissionsByRole(3);

    expect(mockedAxios.get).toHaveBeenCalledWith('api/Permission/Role/3');
    expect(result).toEqual(mockPermissionResponse);
  });

  it('updatePermissions delega a permissionService.updateBatch', async () => {
    const responseData = { isSuccess: true };
    mockedAxios.put.mockResolvedValueOnce({ data: responseData });

    const result = await updatePermissions(mockBatchPayload);

    expect(mockedAxios.put).toHaveBeenCalledWith('api/Permission/Update', mockBatchPayload);
    expect(result).toEqual(responseData);
  });
});