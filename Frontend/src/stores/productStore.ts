import { defineStore, storeToRefs } from 'pinia';
import { createBaseStore } from '@/stores/baseStore';
import { Product } from '@/interfaces/productInterface';
import { productService } from '@/services/productService';

const useBaseProductStore = createBaseStore<Product>('product-base', productService);

export const useProductStore = defineStore('product', () => {
  const base = useBaseProductStore();

  // Estado reactivo del base
  const {
    items, selectedItem, totalItems, loading, lastFilterParams,
    list, selected, total
  } = storeToRefs(base);

  async function registerProduct(product: FormData) {
    const result = await productService.registerProduct(product);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await base.fetchAll(base.lastFilterParams ?? {});
    return result;
  }

  async function editProduct(id: number, product: FormData) {
    const result = await productService.editProduct(id, product);
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    await base.fetchAll(base.lastFilterParams ?? {});
    return result;
  }

  async function generateProductCode() {
    const result = await productService.generateProductCode();
    if (!result.isSuccess) throw new Error(result.message ?? result.errors);
    return result;
  }

  return {
    // Estado (reactivo via storeToRefs)
    items,
    selectedItem,
    totalItems,
    loading,
    lastFilterParams,
    list,
    selected,
    total,
    // Métodos del base
    fetchAll: base.fetchAll,
    fetchForSelect: base.fetchForSelect,
    fetchById: base.fetchById,
    downloadExcel: base.downloadExcel,
    downloadPdf: base.downloadPdf,
    enable: base.enable,
    disable: base.disable,
    remove: base.remove,
    // Métodos propios
    registerProduct,
    editProduct,
    generateProductCode,
  };
});