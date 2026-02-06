<template>
  <div>
    <BrandList :brands="brands" :loading="loading" :totalBrands="totalBrands" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-brand="openForm"
      @fetch-brands="fetchBrands" @search-brands="searchBrands" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf" />

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
import { useAuthStore } from '@/stores/auth';
import { Brand } from '@/interfaces/brandInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import BrandList from '@/components/Brand/BrandList.vue';
import BrandForm from '@/components/Brand/BrandForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

// Pinia store
const brandStore = useBrandStore();
const authStore = useAuthStore();

const toast = useToast();

// Composable de filtros
const filterMap: Record<string, number> = { "Marca": 1 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Marca', filterMap);

// Control de paginación
const currentPage = ref(1);
const itemsPerPage = ref(10);

// Control de búsqueda
const search = ref<string | null>(null);
const drawer = ref(false);

// Control de modales y formularios
const form = ref(false);
const modal = ref(false);

// Marca seleccionada para edición o eliminación
const selectedBrand = ref<Brand | null>(null);

// Tipo de acción a realizar en el modal (0: eliminar, 1: activar, 2: desactivar)
const action = ref<0 | 1 | 2>(0);

// Estado de descarga de Excel y Pdf
const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

// Computed properties - Ahora usan Pinia
const brands = computed(() => brandStore.brands);
const loading = computed(() => brandStore.loading);
const totalBrands = computed(() => brandStore.totalBrands);

// Permisos del usuario (ahora usan Pinia - authStore)
const canCreate = computed((): boolean => authStore.hasPermission('marcas', 'crear'));
const canRead = computed((): boolean => authStore.hasPermission('marcas', 'leer'));
const canEdit = computed((): boolean => authStore.hasPermission('marcas', 'editar'));
const canDelete = computed((): boolean => authStore.hasPermission('marcas', 'eliminar'));
const canDownload = computed((): boolean => authStore.hasPermission('marcas', 'descargar'));

// Métodos
const openModal = (payload: { brand: Brand, action: 0 | 1 | 2 }) => {
  selectedBrand.value = payload.brand;
  action.value = payload.action;
  modal.value = true;
};

const openForm = (brand?: Brand) => {
  selectedBrand.value = brand ? { ...brand } : {
    idBrand: null,
    brandName: '',
    auditCreateDate: '',
    statusBrand: ''
  };
  form.value = true;
};

const fetchBrands = async (params?: any) => {
  try {
    await brandStore.fetchBrands(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchBrands = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await brandStore.fetchBrands({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar marcas');
  }
};

const refreshBrands = () => {
  if (search.value?.trim()) {
    searchBrands({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchBrands();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshBrands();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshBrands();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await brandStore.downloadBrandsExcel({
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
    await brandStore.downloadBrandsPdf({
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
  fetchBrands();
};

const handleActionCompleted = () => {
  fetchBrands();
};

onMounted(() => {
  fetchBrands();
});
</script>