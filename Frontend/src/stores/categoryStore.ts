import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Category } from '@/interfaces/categoryInterface';
import { categoryService } from '@/services/categoryService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useCategoryStore = defineStore('category', () => {
  const items = ref<Category[]>([]);
  const selectedItem = ref<Category | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const categories = computed(() => items.value);
  const selectedCategory = computed(() => selectedItem.value);
  const totalCategories = computed(() => totalItems.value || 0);

  async function fetchCategories(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const result = await categoryService.fetchAll(params);
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

  async function downloadCategoriesExcel(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await categoryService.downloadExcel(filterParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadCategoriesPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await categoryService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function selectCategory() {
    loading.value = true;
    items.value = [];

    try {
      const result = await categoryService.select();
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

  async function fetchCategoryById(id: number) {
    loading.value = true;

    try {
      const result = await categoryService.fetchById(id);
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

  async function registerCategory(category: Category) {
    try {
      const result = await categoryService.create(category);
      if (result.isSuccess) {
        await fetchCategories(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function editCategory(id: number, category: Category) {
    try {
      const result = await categoryService.update(id, category);
      if (result.isSuccess) {
        await fetchCategories(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function enableCategory(id: number) {
    try {
      const result = await categoryService.enable(id);
      if (result.isSuccess) {
        await fetchCategories(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableCategory(id: number) {
    try {
      const result = await categoryService.disable(id);
      if (result.isSuccess) {
        await fetchCategories(lastFilterParams.value || {});
      }
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function removeCategory(id: number) {
    try {
      const result = await categoryService.remove(id);
      if (result.isSuccess) {
        await fetchCategories(lastFilterParams.value || {});
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

    categories,
    selectedCategory,
    totalCategories,

    fetchCategories,
    downloadCategoriesExcel,
    downloadCategoriesPdf,
    selectCategory,
    fetchCategoryById,
    registerCategory,
    editCategory,
    enableCategory,
    disableCategory,
    removeCategory,
  };
});