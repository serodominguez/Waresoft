import { ref, computed } from 'vue';
import { defineStore } from 'pinia';
import { FilterParams, BaseState } from '@/interfaces/baseInterface';
import { BaseService } from '@/interfaces/baseServiceInterface';

export function createBaseStore<T>(storeName: string, service: BaseService<T>) {
  return defineStore(storeName, () => {

    const items = ref<BaseState<T>['items']>([]);
    const selectedItem = ref<BaseState<T>['selectedItem']>(null);
    const totalItems = ref<BaseState<T>['totalItems']>(0);
    const loading = ref<BaseState<T>['loading']>(false);
    const lastFilterParams = ref<BaseState<T>['lastFilterParams']>({});

    const list = computed(() => items.value);
    const selected = computed(() => selectedItem.value);
    const total = computed(() => totalItems.value);

    // ── Lecturas ──────────────────────────────────────────────
    async function fetchAll(params: FilterParams = {}) {
      loading.value = true;
      items.value = [];
      lastFilterParams.value = params;
      try {
        const result = await service.fetchAll(params);
        if (!result.isSuccess) throw new Error(result.message ?? result.errors);
        items.value = result.data;           // BaseResponse<T[]>.data
        totalItems.value = result.totalRecords; // BaseResponse<T[]>.totalRecords
      } finally {
        loading.value = false;
      }
    }

    async function fetchForSelect() {
      loading.value = true;
      items.value = [];
      try {
        const result = await service.select();
        if (!result.isSuccess) throw new Error(result.message ?? result.errors);
        items.value = result.data;
      } finally {
        loading.value = false;
      }
    }

    async function fetchById(id: number) {
      loading.value = true;
      try {
        const result = await service.fetchById(id);
        if (!result.isSuccess) throw new Error(result.message ?? result.errors);
        selectedItem.value = result.data;
      } finally {
        loading.value = false;
      }
    }

    // ── Descargas ─────────────────────────────────────────────
    async function downloadExcel(params?: FilterParams) {
      await service.downloadExcel(params ?? lastFilterParams.value);
    }

    async function downloadPdf(params?: FilterParams) {
      await service.downloadPdf(params ?? lastFilterParams.value);
    }

    // ── Mutaciones ────────────────────────────────────────────
    async function register(item: T) {
      const result = await service.create(item);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      await fetchAll(lastFilterParams.value);
      return result;
    }

    async function edit(id: number, item: T) {
      const result = await service.update(id, item);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      await fetchAll(lastFilterParams.value);
      return result;
    }

    async function enable(id: number) {
      const result = await service.enable(id);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      await fetchAll(lastFilterParams.value);
      return result;
    }

    async function disable(id: number) {
      const result = await service.disable(id);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      await fetchAll(lastFilterParams.value);
      return result;
    }

    async function remove(id: number) {
      const result = await service.remove(id);
      if (!result.isSuccess) throw new Error(result.message ?? result.errors);
      await fetchAll(lastFilterParams.value);
      return result;
    }

    return {
      items, 
      selectedItem, 
      totalItems, 
      loading, 
      lastFilterParams,
      list, 
      selected, 
      total,
      fetchAll, 
      fetchForSelect, 
      fetchById,
      downloadExcel, 
      downloadPdf,
      register, 
      edit, 
      enable, 
      disable, 
      remove,
    };
  });
}