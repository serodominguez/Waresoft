import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Module } from '@/interfaces/moduleInterface';
import { moduleService } from '@/services/moduleService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useModuleStore = defineStore('module', () => {
  const items = ref<Module[]>([]);
  const selectedItem = ref<Module | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const modules = computed(() => items.value);
  const selectedModule = computed(() => selectedItem.value);
  const totalModules = computed(() => totalItems.value || 0);

  async function fetchModules(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const resultado = await moduleService.fetchAll(params);
      if (resultado.isSuccess) {
        items.value = resultado.data;
        totalItems.value = resultado.totalRecords;
      } else {
        error.value = resultado.message || resultado.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function downloadModulesExcel(params?: FilterParams) {
    try {
      const filtrosParams = params || lastFilterParams.value || {};
      await moduleService.downloadExcel(filtrosParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadModulesPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await moduleService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function fetchModuleById(id: number) {
    loading.value = true;

    try {
      const resultado = await moduleService.fetchById(id);
      if (resultado.isSuccess) {
        selectedItem.value = resultado.data;
      } else {
        error.value = resultado.message || resultado.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function registerModule(module: Module) {
    try {
      const resultado = await moduleService.create(module);
      if (resultado.isSuccess) {
        await fetchModules(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function editModule(id: number, module: Module) {
    try {
      const resultado = await moduleService.update(id, module);
      if (resultado.isSuccess) {
        await fetchModules(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function enableModule(id: number) {
    try {
      const resultado = await moduleService.enable(id);
      if (resultado.isSuccess) {
        await fetchModules(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableModule(id: number) {
    try {
      const resultado = await moduleService.disable(id);
      if (resultado.isSuccess) {
        await fetchModules(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function removeModule(id: number) {
    try {
      const resultado = await moduleService.remove(id);
      if (resultado.isSuccess) {
        await fetchModules(lastFilterParams.value || {});
      }
      return resultado;
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

    modules,
    selectedModule,
    totalModules,

    fetchModules,
    downloadModulesExcel,
    downloadModulesPdf,
    fetchModuleById,
    registerModule,
    editModule,
    enableModule,
    disableModule,
    removeModule,
  };
})