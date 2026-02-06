import axios from 'axios';
import { PermissionResponse } from '@/interfaces/permissionInterface';

class PermissionService {
  private readonly endpoint = 'Permission';


  async fetchByRole(roleId: number): Promise<PermissionResponse> {
    const response = await axios.get<PermissionResponse>(
      `api/${this.endpoint}/Role/${roleId}`
    );
    return response.data;
  }

  async updateBatch(
    permissions: Array<{ idPermission: number; status: boolean }>
  ): Promise<any> {
    const response = await axios.put(
      `api/${this.endpoint}/Update`,
      permissions
    );
    return response.data;
  }
}

export const permissionService = new PermissionService();

export const fetchPermissionsByRole = (roleId: number) => permissionService.fetchByRole(roleId);
export const updatePermissions = (permissions: Array<{ idPermission: number; status: boolean }>) => permissionService.updateBatch(permissions);

