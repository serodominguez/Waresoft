import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Role } from '@/interfaces/roleInterface';
import { roleService } from '@/services/roleService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useRoleStore = defineStore('role', () => {
  const items = ref<Role[]>([]);
  const selectedItem = ref<Role | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const roles = computed(() => items.value);
  const selectedRole = computed(() => selectedItem.value);
  const totalRoles = computed(() => totalItems.value || 0);

  async function fetchRoles(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const result = await roleService.fetchAll(params);
      if (result.isSuccess) {
        items.value = result.data;
        totalItems.value = result.totalRecords;
      } else {
        error.value = result.message || result.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function downloadRolesExcel(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await roleService.downloadExcel(filterParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadRolesPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await roleService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function selectRole() {
    loading.value = true;
    items.value = [];

    try {
      const result = await roleService.select();
      if (result.isSuccess) {
        items.value = result.data;
      } else {
        error.value = result.message || result.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function fetchRoleById(id: number) {
    loading.value = true;

    try {
      const result = await roleService.fetchById(id);
      if (result.isSuccess) {
        selectedItem.value = result.data;
      } else {
        error.value = result.message || result.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function registerRole(role: Role) {
    try {
      const result = await roleService.create(role);
      if (result.isSuccess) {
        await fetchRoles(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function editRole(id: number, role: Role) {
    try {
      const result = await roleService.update(id, role);
      if (result.isSuccess) {
        await fetchRoles(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function enableRole(id: number) {
    try {
      const result = await roleService.enable(id);
      if (result.isSuccess) {
        await fetchRoles(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableRole(id: number) {
    try {
      const result = await roleService.disable(id);
      if (result.isSuccess) {
        await fetchRoles(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function removeRole(id: number) {
    try {
      const result = await roleService.remove(id);
      if (result.isSuccess) {
        await fetchRoles(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  return {
    items,
    selectedItem,
    totalItems,
    loading,
    error,
    lastFilterParams,

    roles,
    selectedRole,
    totalRoles,

    fetchRoles,
    downloadRolesExcel,
    downloadRolesPdf,
    selectRole,
    fetchRoleById,
    registerRole,
    editRole,
    enableRole,
    disableRole,
    removeRole,
  };
});