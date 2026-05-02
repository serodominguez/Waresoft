<template>
  <div>
    <StockList :stores="stores" :rows="rows" :totalRows="totalRows" :loading="loading" :canRead="canRead"
      :canDownload="canDownload" :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf"
      :items-per-page="itemsPerPage" v-model:drawer="drawer" v-model:selectedFilter="selectedFilter"
      v-model:state="state" v-model:startDate="startDate" v-model:endDate="endDate" @fetch-stock="fetchStock"
      @search-stock="searchStock" @update-items-per-page="updateItemsPerPage" @change-page="changePage"
      @download-excel="downloadExcel" @download-pdf="downloadPdf" @clear-filters="clearFilters" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useInventoryStore } from '@/stores/inventoryStore';
import { useAuthStore } from '@/stores/authStore';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import { usePagination } from '@/composables/usePagination';
import StockList from '@/components/Inventory/StockList.vue';

const inventoryStore = useInventoryStore();
const authStore      = useAuthStore();
const toast          = useToast();

const filterMap: Record<string, number> = {
  'Código': 1, 'Descripción': 2, 'Material': 3, 'Color': 4, 'Marca': 5, 'Categoría': 6
};
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Código', filterMap);

const search           = ref<string | null>(null);
const drawer           = ref(false);
const downloadingExcel = ref(false);
const downloadingPdf   = ref(false);

const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    inventoryStore.fetchInventoryPivot({
      pageNumber:  params.pageNumber,
      pageSize:    params.pageSize,
      sort:        'IdProduct',
      order:       'asc',
      ...getFilterParams(search.value),
    });
  }
);

const loading    = computed(() => inventoryStore.loading);
const totalRows  = computed(() => inventoryStore.totalRows);
const stores     = computed(() => inventoryStore.inventoryPivot?.stores ?? []);
const rows       = computed(() => inventoryStore.inventoryPivot?.rows ?? []);

const canRead     = computed(() => authStore.hasPermission('inventario', 'leer'));
const canDownload = computed(() => authStore.hasPermission('inventario', 'descargar'));

const fetchStock = async () => {
  try {
    await inventoryStore.fetchInventoryPivot({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      sort:        'IdProduct',
      order:       'asc',
      stateFilter: state.value === 'Activos' ? 1 : 0,
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchStock = async (params: {
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
    await inventoryStore.fetchInventoryPivot({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      sort:       'IdProduct',
      order:      'asc',
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar stock');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Código';
  state.value          = 'Activos';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchStock();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await inventoryStore.downloadInventoryPivotExcel({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      sort:       'IdProduct',
      order:      'asc',
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
    await inventoryStore.downloadInventoryPivotPdf({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      sort:       'IdProduct',
      order:      'asc',
      ...getFilterParams(params.search),
    });
    toast.success('Archivo PDF descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo PDF');
  } finally {
    downloadingPdf.value = false;
  }
};

onMounted(() => {
  fetchStock();
});
</script>