import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Product } from '@/interfaces/productInterface';
import { productService } from '@/services/productService';
import { FilterParams } from '@/interfaces/baseInterface';

export const useProductStore = defineStore('product', () => {
  const items = ref<Product[]>([]);
  const selectedItem = ref<Product | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const products = computed(() => items.value);
  const selectedProduct = computed(() => selectedItem.value);
  const totalProducts = computed(() => totalItems.value || 0);

  async function fetchProducts(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const resultado = await productService.fetchAll(params);
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

  async function downloadProductsExcel(params?: FilterParams) {
    try {
      const filtrosParams = params || lastFilterParams.value || {};
      await productService.downloadExcel(filtrosParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadProductsPdf(params?: FilterParams) {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await productService.downloadPdf(filterParams);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function fetchProductById(id: number) {
    loading.value = true;

    try {
      const resultado = await productService.fetchById(id);
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

  async function registerProduct(product: Product) {
    try {
      const resultado = await productService.create(product);
      if (resultado.isSuccess) {
        await fetchProducts(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function editProduct(id: number, product: Product) {
    try {
      const resultado = await productService.update(id, product);
      if (resultado.isSuccess) {
        await fetchProducts(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function enableProduct(id: number) {
    try {
      const resultado = await productService.enable(id);
      if (resultado.isSuccess) {
        await fetchProducts(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function disableProduct(id: number) {
    try {
      const resultado = await productService.disable(id);
      if (resultado.isSuccess) {
        await fetchProducts(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  async function removeProduct(id: number) {
    try {
      const resultado = await productService.remove(id);
      if (resultado.isSuccess) {
        await fetchProducts(lastFilterParams.value || {});
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

    products,
    selectedProduct,
    totalProducts,

    fetchProducts,
    downloadProductsExcel,
    downloadProductsPdf,
    fetchProductById,
    registerProduct,
    editProduct,
    enableProduct,
    disableProduct,
    removeProduct,
  };
});