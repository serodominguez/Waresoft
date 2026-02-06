import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { User } from '@/interfaces/userInterface';
import { userService } from '@/services/userService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useUserStore = defineStore('user', () => {
  const items = ref<User[]>([]);
  const selectedItem = ref<User | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const users = computed(() => items.value);
  const selectedUser = computed(() => selectedItem.value);
  const totalUsers = computed(() => totalItems.value || 0);

  async function fetchUsers(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const result = await userService.fetchAll(params);
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

  async function downloadUsersExcel(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await userService.downloadExcel(filterParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadUsersPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await userService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function selectUser() {
    loading.value = true;
    items.value = [];

    try {
      const result = await userService.select();
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

  async function fetchUserById(id: number) {
    loading.value = true;

    try {
      const result = await userService.fetchById(id);
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

  async function registerUser(user: User) {
    try {
      const result = await userService.create(user);
      if (result.isSuccess) {
        await fetchUsers(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function editUser(id: number, user: User) {
    try {
      const result = await userService.update(id, user);
      if (result.isSuccess) {
        await fetchUsers(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function enableUser(id: number) {
    try {
      const result = await userService.enable(id);
      if (result.isSuccess) {
        await fetchUsers(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableUser(id: number) {
    try {
      const result = await userService.disable(id);
      if (result.isSuccess) {
        await fetchUsers(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function removeUser(id: number) {
    try {
      const result = await userService.remove(id);
      if (result.isSuccess) {
        await fetchUsers(lastFilterParams.value || {});
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

    users,
    selectedUser,
    totalUsers,

    fetchUsers,
    downloadUsersExcel,
    downloadUsersPdf,
    selectUser,
    fetchUserById,
    registerUser,
    editUser,
    enableUser,
    disableUser,
    removeUser,
  };
});