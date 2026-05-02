<template>
  <div>
    <InventoryList :inventories="inventories" :loading="loading" :totalInventories="totalInventories"
      :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" :downloadingSheet="downloadingSheet"
      :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" :canDownload="canDownload"
      :items-per-page="itemsPerPage" v-model:drawer="drawer" v-model:selectedFilter="selectedFilter"
      v-model:state="state" v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm"
      @open-modal="openModal" @edit-inventory="openForm" @fetch-inventories="fetchInventories"
      @search-inventories="searchInventories" @update-items-per-page="updateItemsPerPage" @change-page="changePage"
      @download-excel="downloadExcel" @download-pdf="downloadPdf" @clear-filters="clearFilters"
      @download-inventory-sheet="downloadInventorySheet" />

    <PriceForm v-model="form" :inventory="selectedInventory" @saved="handleSaved" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useInventoryStore } from '@/stores/inventoryStore';
import { useAuthStore } from '@/stores/authStore';
import { Inventory } from '@/interfaces/inventoryInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import { usePagination } from '@/composables/usePagination';
import InventoryList from '@/components/Inventory/InventoryList.vue';
import PriceForm from '@/components/Inventory/PriceForm.vue';

const inventoryStore = useInventoryStore();
const authStore      = useAuthStore();
const toast          = useToast();

const filterMap: Record<string, number> = {
  "Código": 1, "Descripción": 2, "Material": 3, "Color": 4, "Precio": 5, "Marca": 6, "Categoría": 7
};
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Código', filterMap);

const search           = ref<string | null>(null);
const drawer           = ref(false);
const form             = ref(false);
const modal            = ref(false);
const selectedInventory = ref<Inventory | null>(null);
const action           = ref<0 | 1 | 2>(0);
const downloadingExcel = ref(false);
const downloadingPdf   = ref(false);
const downloadingSheet = ref(false);

const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    inventoryStore.fetchInventories({
      pageNumber:  params.pageNumber,
      pageSize:    params.pageSize,
      sort:        'IdProduct',
      order:       'desc',
      ...getFilterParams(search.value),
    });
  }
);

const inventories     = computed(() => inventoryStore.inventories);
const loading         = computed(() => inventoryStore.loading);
const totalInventories = computed(() => inventoryStore.totalInventories);

const canCreate   = computed(() => authStore.hasPermission('inventario', 'crear'));
const canRead     = computed(() => authStore.hasPermission('inventario', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('inventario', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('inventario', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('inventario', 'descargar'));

const openModal = (payload: { inventory: Inventory; action: 0 | 1 | 2 }) => {
  selectedInventory.value = payload.inventory;
  action.value            = payload.action;
  modal.value             = true;
};

const openForm = (inventory?: Inventory) => {
  selectedInventory.value = inventory ? { ...inventory } : {
    idStore:           null,
    idProduct:         null,
    code:              '',
    description:       '',
    material:          '',
    color:             '',
    unitMeasure:       '',
    stockAvailable:    null,
    calculatedStock:   null,
    stockDifference:   null,
    stockInTransit:    null,
    price:             null,
    replenishment:     '',
    brandName:         '',
    categoryName:      '',
    auditCreateDate:   ''
  };
  form.value = true;
};

const fetchInventories = async () => {
  try {
    await inventoryStore.fetchInventories({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      sort:        'IdProduct',
      order:       'desc',
      stateFilter: state.value === 'Activos' ? 1 : 0,
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchInventories = async (params: {
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
    await inventoryStore.fetchInventories({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      sort:       'IdProduct',
      order:      'desc',
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
  fetchInventories();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await inventoryStore.downloadInventoriesExcel({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      sort:       'IdProduct',
      order:      'desc',
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
    await inventoryStore.downloadInventoriesPdf({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      sort:       'IdProduct',
      order:      'desc',
      ...getFilterParams(params.search),
    });
    toast.success('Archivo PDF descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo PDF');
  } finally {
    downloadingPdf.value = false;
  }
};

const downloadInventorySheet = async (params: { search: string | null }) => {
  downloadingSheet.value = true;
  try {
    await inventoryStore.downloadInventorySheet({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      sort:       'IdProduct',
      order:      'desc',
      ...getFilterParams(params.search),
    });
    toast.success('Planilla descargada correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar la planilla de inventario');
  } finally {
    downloadingSheet.value = false;
  }
};

const handleSaved = () => {
  fetchInventories();
};

onMounted(() => {
  fetchInventories();
});
</script>