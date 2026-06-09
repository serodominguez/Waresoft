import { defineStore, storeToRefs } from 'pinia';
import { ref } from 'vue';
import { createBaseStore } from '@/stores/baseStore';
import { Product, ProductStats } from '@/interfaces/productInterface';
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

  async function generateBarcodePdf(productId: number, quantity: number) {
    const blob = await productService.generateBarcodePdf(productId, quantity);
    const pdfBlob = new Blob([blob], { type: 'application/pdf' });
    const url = window.URL.createObjectURL(pdfBlob);
    window.open(url, '_blank');
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
    generateBarcodePdf
  };
});

export const useProductStatsStore = defineStore('productStats', () => {
  const stats = ref<ProductStats | null>(null);
  const loading = ref<boolean>(false);

  async function fetchStats() {
    loading.value = true;
    try {
      stats.value = await productService.getStats();
    } finally {
      loading.value = false;
    }
  }

  return {
    stats,
    loading,
    fetchStats,
  };
});