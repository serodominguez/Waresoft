<template>
  <div>
    <SupplierList :suppliers="suppliers" :loading="loading" :totalSuppliers="totalSuppliers"
      :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead"
      :canEdit="canEdit" :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @edit-supplier="openForm" @fetch-supplier="fetchSuppliers" @search-suppliers="searchSuppliers"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel"
      @download-pdf="downloadPdf" />

    <SupplierForm v-model="form" :supplier="selectedSupplier" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedSupplier?.idSupplier || 0" :item="selectedSupplier?.companyName || ''"
      :action="action" moduleName="supplier" entityName="Supplier" name="Proveedor" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useSupplierStore } from '@/stores/supplierStore';
import { useAuthStore } from '@/stores/auth';
import { Supplier } from '@/interfaces/supplierInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import SupplierList from '@/components/Supplier/SupplierList.vue';
import SupplierForm from '@/components/Supplier/SupplierForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const supplierStore = useSupplierStore();
const authStore = useAuthStore();

const toast = useToast();

const filterMap: Record<string, number> = {
  "Empresa": 1,
  "Contacto": 2
};
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Empresa', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);
const search = ref<string | null>(null);
const drawer = ref(false);
const form = ref(false);
const modal = ref(false);
const selectedSupplier = ref<Supplier | null>(null);
const action = ref<0 | 1 | 2>(0);

const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

const suppliers = computed(() => supplierStore.suppliers);
const loading = computed(() => supplierStore.loading);
const totalSuppliers = computed(() => supplierStore.totalSuppliers);

const canCreate = computed((): boolean => authStore.hasPermission('proveedores', 'crear'));
const canRead = computed((): boolean => authStore.hasPermission('proveedores', 'leer'));
const canEdit = computed((): boolean => authStore.hasPermission('proveedores', 'editar'));
const canDelete = computed((): boolean => authStore.hasPermission('proveedores', 'eliminar'));
const canDownload = computed((): boolean => authStore.hasPermission('proveedores', 'descargar'));

const openModal = (payload: { supplier: Supplier, action: 0 | 1 | 2 }) => {
  selectedSupplier.value = payload.supplier;
  action.value = payload.action;
  modal.value = true;
};

const openForm = (supplier?: Supplier) => {
  selectedSupplier.value = supplier ? { ...supplier } : {
    idSupplier: null,
    companyName: '',
    contact: '',
    email: '',
    phoneNumber: null,
    auditCreateDate: '',
    statusSupplier: ''
  };
  form.value = true;
};

const fetchSuppliers = async (params?: any) => {
  try {
    await supplierStore.fetchSuppliers(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchSuppliers = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await supplierStore.fetchSuppliers({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar proveedores');
  }
};

const refreshSuppliers = () => {
  if (search.value?.trim()) {
    searchSuppliers({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchSuppliers();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshSuppliers();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshSuppliers();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await supplierStore.downloadSuppliersExcel({
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
    await supplierStore.downloadSuppliersPdf({
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
  fetchSuppliers();
};

const handleActionCompleted = () => {
  fetchSuppliers();
};

onMounted(() => {
  fetchSuppliers();
});
</script>