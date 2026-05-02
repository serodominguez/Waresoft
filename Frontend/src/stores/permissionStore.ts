import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import {
  fetchPermissionsByRole as fetchPermissionsByRoleService,
  updatePermissions as updatePermissionsService
} from '@/services/permissionService';
import { Permission, PermissionsByModule } from '@/interfaces/permissionInterface';

export const usePermissionStore = defineStore('permission', () => {
  const items = ref<Permission[]>([]);
  const loading = ref<boolean>(false);

  const permissions = computed(() => items.value);

  const permissionsByModule = computed((): PermissionsByModule[] => {
    const grouped = items.value.reduce((acc, perm) => {
      if (!acc[perm.moduleName]) {
        acc[perm.moduleName] = {
          module: perm.moduleName,
          permissions: { crear: false, leer: false, editar: false, eliminar: false, descargar: false },
        };
      }
      const actionKey = perm.actionName.toLowerCase() as keyof PermissionsByModule['permissions'];

      if (acc[perm.moduleName].permissions[actionKey] !== undefined) {
        acc[perm.moduleName].permissions[actionKey] = perm.status;
      }

      return acc;
    }, {} as Record<string, PermissionsByModule>);

    return Object.values(grouped);
  });

  async function fetchPermissionsByRole(roleId: number) {
    loading.value = true;
    items.value = []; // Limpieza antes de cargar
    try {
      const result = await fetchPermissionsByRoleService(roleId);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      items.value = result.data;
    } finally {
      loading.value = false;
    }
  }

  async function updatePermissions(updatedPermissions: Array<{ idPermission: number; status: boolean }>) {
    const result = await updatePermissionsService(updatedPermissions);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    return result;
  }

  function clearPermissions() {
    items.value = [];
  }

  return {
    items,
    loading,
    permissions,
    permissionsByModule,

    fetchPermissionsByRole,
    updatePermissions,
    clearPermissions,
  };
});
