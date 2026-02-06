<template>
  <div>
    <StoreList :stores="stores" :loading="loading" :totalStores="totalStores" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-store="openForm"
      @fetch-stores="fetchStores" @search-stores="searchStores" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf" />

    <StoreForm v-model="form" :store="selectedStore" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedStore?.idStore || 0" :item="selectedStore?.storeName || ''"
      :action="action" moduleName="store" entityName="Store" name="Tienda" gender="female"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useStoreStore } from '@/stores/storeStore';
import { useAuthStore } from '@/stores/auth';
import { Store } from '@/interfaces/storeInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import StoreList from '@/components/Store/StoreList.vue';
import StoreForm from '@/components/Store/StoreForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const storeStore = useStoreStore();
const authStore = useAuthStore();

const toast = useToast();

const filterMap: Record<string, number> = {
  "Tienda": 1,
  "Encargado": 2,
  "Dirección": 3,
  "Ciudad": 4
};
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Tienda', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);
const search = ref<string | null>(null);
const drawer = ref(false);
const form = ref(false);
const modal = ref(false);
const selectedStore = ref<Store | null>(null);
const action = ref<0 | 1 | 2>(0);

const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

const stores = computed(() => storeStore.stores);
const loading = computed(() => storeStore.loading);
const totalStores = computed(() => storeStore.totalStores);

const canCreate = computed((): boolean => authStore.hasPermission('establecimientos', 'crear'));
const canRead = computed((): boolean => authStore.hasPermission('establecimientos', 'leer'));
const canEdit = computed((): boolean => authStore.hasPermission('establecimientos', 'editar'));
const canDelete = computed((): boolean => authStore.hasPermission('establecimientos', 'eliminar'));
const canDownload = computed((): boolean => authStore.hasPermission('establecimientos', 'descargar'));

const openModal = (payload: { store: Store, action: 0 | 1 | 2 }) => {
  selectedStore.value = payload.store;
  action.value = payload.action;
  modal.value = true;
};

const openForm = (storeData?: Store) => {
  selectedStore.value = storeData ? { ...storeData } : {
    idStore: null,
    storeName: '',
    manager: '',
    address: '',
    phoneNumber: null,
    city: '',
    email: '',
    type: '',
    auditCreateDate: '',
    statusStore: ''
  };
  form.value = true;
};

const fetchStores = async (params?: any) => {
  try {
    await storeStore.fetchStores(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchStores = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await storeStore.fetchStores({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar tiendas');
  }
};

const refreshStores = () => {
  if (search.value?.trim()) {
    searchStores({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchStores();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshStores();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshStores();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await storeStore.downloadStoresExcel({
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
    await storeStore.downloadStoresPdf({
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
  fetchStores();
};

const handleActionCompleted = () => {
  fetchStores();
};

onMounted(() => {
  fetchStores();
});
</script>