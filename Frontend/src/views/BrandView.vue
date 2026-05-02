<template>
  <div>
    <BrandList :brands="brands" :loading="loading" :totalBrands="totalBrands" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-brand="openForm"
      @fetch-brands="fetchBrands" @search-brands="searchBrands" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf"
      @clear-filters="clearFilters" />

    <BrandForm v-model="form" :brand="selectedBrand" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedBrand?.idBrand || 0" :item="selectedBrand?.brandName || ''"
      :action="action" moduleName="brand" entityName="Brand" name="Marca" gender="female"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useBrandStore } from '@/stores/brandStore';
import { useAuthStore } from '@/stores/authStore';
import { Brand } from '@/interfaces/brandInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import { usePagination } from '@/composables/usePagination';
import BrandList from '@/components/Brand/BrandList.vue';
import BrandForm from '@/components/Brand/BrandForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

//Stores
const brandStore = useBrandStore();
const authStore  = useAuthStore();
const toast      = useToast();

//Filtros
const filterMap: Record<string, number> = { "Marca": 1 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Marca', filterMap);

//Estado local
const search          = ref<string | null>(null);
const drawer          = ref(false);
const form            = ref(false);
const modal           = ref(false);
const selectedBrand   = ref<Brand | null>(null);
const action          = ref<0 | 1 | 2>(0);
const downloadingExcel = ref(false);
const downloadingPdf   = ref(false);

//Paginación
// usePagination recibe el callback que se ejecuta al cambiar página
// o items por página. Aquí decide si usar los filtros activos o no.
const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    brandStore.fetchAll({
      pageNumber:  params.pageNumber,
      pageSize:    params.pageSize,
      ...getFilterParams(search.value),
    });
  }
);

//Computed
const brands      = computed(() => brandStore.list);
const loading     = computed(() => brandStore.loading);
const totalBrands = computed(() => brandStore.total);

const canCreate   = computed(() => authStore.hasPermission('marcas', 'crear'));
const canRead     = computed(() => authStore.hasPermission('marcas', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('marcas', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('marcas', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('marcas', 'descargar'));

//Métodos
const openModal = (payload: { brand: Brand; action: 0 | 1 | 2 }) => {
  selectedBrand.value = payload.brand;
  action.value        = payload.action;
  modal.value         = true;
};

const openForm = (brand?: Brand) => {
  selectedBrand.value = brand ? { ...brand } : {
    idBrand:         null,
    brandName:       '',
    auditCreateDate: '',
    statusBrand:     ''
  };
  form.value = true;
};

//Carga inicial y recarga tras acciones (guardar, activar, eliminar)
const fetchBrands = async () => {
  try {
    await brandStore.fetchAll({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0,
    });
  } catch (error) {
    handleSilentError(error);
  }
};

//Búsqueda con filtros activos
const searchBrands = async (params: {
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
    await brandStore.fetchAll({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar marcas');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Marca';
  state.value          = 'Activos';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchBrands();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await brandStore.downloadExcel({
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
    await brandStore.downloadPdf({
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
  fetchBrands();
};

const handleActionCompleted = () => {
  fetchBrands();
};

//Lifecycle
onMounted(() => {
  fetchBrands();
});
</script>