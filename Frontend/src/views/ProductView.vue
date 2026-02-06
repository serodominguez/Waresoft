<template>
  <div>
    <ProductList :products="products" :loading="loading" :totalProducts="totalProducts"
      :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead"
      :canEdit="canEdit" :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @edit-product="openForm" @fetch-products="fetchProducts" @search-products="searchProducts"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel"
      @download-pdf="downloadPdf" />

    <ProductForm v-model="form" :product="selectedProduct" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedProduct?.idProduct || 0" :item="selectedProduct?.description || ''"
      :action="action" moduleName="product" entityName="Product" name="Producto" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useProductStore } from '@/stores/productStore';
import { useAuthStore } from '@/stores/auth';
import { Product } from '@/interfaces/productInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import ProductList from '@/components/Product/ProductList.vue';
import ProductForm from '@/components/Product/ProductForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const productStore = useProductStore();
const authStore = useAuthStore();

const toast = useToast();

const filterMap: Record<string, number> = {
  "Código": 1,
  "Descripción": 2,
  "Material": 3,
  "Color": 4,
  "Marca": 5,
  "Categoría": 6
};
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Código', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);

const search = ref<string | null>(null);
const drawer = ref(false);

const form = ref(false);
const modal = ref(false);

const selectedProduct = ref<Product | null>(null);

const action = ref<0 | 1 | 2>(0);

const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

const products = computed(() => productStore.products);
const loading = computed(() => productStore.loading);
const totalProducts = computed(() => productStore.totalProducts);

const canCreate = computed((): boolean => authStore.hasPermission('productos', 'crear'));
const canRead = computed((): boolean => authStore.hasPermission('productos', 'leer'));
const canEdit = computed((): boolean => authStore.hasPermission('productos', 'editar'));
const canDelete = computed((): boolean => authStore.hasPermission('productos', 'eliminar'));
const canDownload = computed((): boolean => authStore.hasPermission('productos', 'descargar'));

const openModal = (payload: { product: Product, action: 0 | 1 | 2 }) => {
  selectedProduct.value = payload.product;
  action.value = payload.action;
  modal.value = true;
};

const openForm = (product?: Product) => {
  selectedProduct.value = product ? { ...product } : {
    idProduct: null,
    code: '',
    description: '',
    material: '',
    color: '',
    unitMeasure: '',
    idBrand: null,
    brandName: '',
    idCategory: null,
    categoryName: '',
    auditCreateDate: '',
    statusProduct: ''
  };
  form.value = true;
};

const fetchProducts = async (params?: any) => {
  try {
    await productStore.fetchProducts(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchProducts = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await productStore.fetchProducts({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar productos');
  }
};

const refreshProducts = () => {
  if (search.value?.trim()) {
    searchProducts({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchProducts();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshProducts();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshProducts();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await productStore.downloadProductsExcel({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    toast.success('Archivo descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo Excel');
  } finally {
    downloadingExcel.value = false;
  }
};

const downloadPdf = async (params: any) => {
  downloadingPdf.value = true;
  try {
    await productStore.downloadProductsPdf({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    toast.success('Archivo PDF descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo PDF');
  } finally {
    downloadingPdf.value = false;
  }
};

const handleSaved = () => {
  fetchProducts();
};

const handleActionCompleted = () => {
  fetchProducts();
};

onMounted(() => {
  fetchProducts();
});
</script>