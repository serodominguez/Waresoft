<template>
  <div>
    <ProductList :products="products" :loading="loading" :totalProducts="totalProducts"
      :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead"
      :canEdit="canEdit" :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @edit-product="openForm" @fetch-products="fetchProducts" @search-products="searchProducts"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel"
      @download-pdf="downloadPdf" @clear-filters="clearFilters" />

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
import { useAuthStore } from '@/stores/authStore';
import { Product } from '@/interfaces/productInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import { usePagination } from '@/composables/usePagination';
import ProductList from '@/components/Product/ProductList.vue';
import ProductForm from '@/components/Product/ProductForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const productStore = useProductStore();
const authStore    = useAuthStore();
const toast        = useToast();

const filterMap: Record<string, number> = { "Código": 1, "Descripción": 2, "Material": 3, "Color": 4, "Marca": 5, "Categoría": 6 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Código', filterMap);

const search          = ref<string | null>(null);
const drawer          = ref(false);
const form            = ref(false);
const modal           = ref(false);
const selectedProduct = ref<Product | null>(null);
const action          = ref<0 | 1 | 2>(0);
const downloadingExcel = ref(false);
const downloadingPdf   = ref(false);

const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    productStore.fetchAll({
      pageNumber:  params.pageNumber,
      pageSize:    params.pageSize,
      ...getFilterParams(search.value),
    });
  }
);

const products      = computed(() => productStore.list);
const loading       = computed(() => productStore.loading);
const totalProducts = computed(() => productStore.total);

const canCreate   = computed(() => authStore.hasPermission('productos', 'crear'));
const canRead     = computed(() => authStore.hasPermission('productos', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('productos', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('productos', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('productos', 'descargar'));

const openModal = (payload: { product: Product; action: 0 | 1 | 2 }) => {
  selectedProduct.value = payload.product;
  action.value          = payload.action;
  modal.value           = true;
};

const openForm = (product?: Product) => {
  selectedProduct.value = product ? { ...product } : {
    idProduct:    null,
    code:         '',
    description:  '',
    material:     '',
    color:        '',
    unitMeasure:  '',
    image:        '',
    idBrand:      null,
    brandName:    '',
    idCategory:   null,
    categoryName: '',
    auditCreateDate: '',
    statusProduct: ''
  };
  form.value = true;
};

const fetchProducts = async () => {
  try {
    await productStore.fetchAll({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0,
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchProducts = async (params: {
  search: string | null;
  selectedFilter: string;
  state: string;
  startDate: Date | null;
  endDate: Date | null;
}) => {
  search.value          = params.search;
  selectedFilter.value  = params.selectedFilter;
  state.value           = params.state;
  startDate.value       = params.startDate;
  endDate.value         = params.endDate;
  currentPage.value     = 1;

  try {
    await productStore.fetchAll({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar productos');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Código';
  state.value          = 'Activos';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchProducts();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await productStore.downloadExcel({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
    });
    toast.success('Archivo descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo Excel');
  } finally {
    downloadingExcel.value = false;
  }
};

const downloadPdf = async (params: { search: string | null }) => {
  downloadingPdf.value = true;
  try {
    await productStore.downloadPdf({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
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