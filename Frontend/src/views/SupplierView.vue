<template>
  <div>
    <SupplierList :suppliers="suppliers" :loading="loading" :totalSuppliers="totalSuppliers"
      :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead"
      :canEdit="canEdit" :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @edit-supplier="openForm" @fetch-supplier="fetchSuppliers" @search-suppliers="searchSuppliers"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel"
      @download-pdf="downloadPdf" @clear-filters="clearFilters" />

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
import { useAuthStore } from '@/stores/authStore';
import { Supplier } from '@/interfaces/supplierInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import { usePagination } from '@/composables/usePagination';
import SupplierList from '@/components/Supplier/SupplierList.vue';
import SupplierForm from '@/components/Supplier/SupplierForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const supplierStore = useSupplierStore();
const authStore     = useAuthStore();
const toast         = useToast();

const filterMap: Record<string, number> = { "Empresa": 1, "Contacto": 2 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Empresa', filterMap);

const search           = ref<string | null>(null);
const drawer           = ref(false);
const form             = ref(false);
const modal            = ref(false);
const selectedSupplier = ref<Supplier | null>(null);
const action           = ref<0 | 1 | 2>(0);
const downloadingExcel = ref(false);
const downloadingPdf   = ref(false);

const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    supplierStore.fetchAll({
      pageNumber:  params.pageNumber,
      pageSize:    params.pageSize,
      ...getFilterParams(search.value),
    });
  }
);

const suppliers     = computed(() => supplierStore.list);
const loading       = computed(() => supplierStore.loading);
const totalSuppliers = computed(() => supplierStore.total);

const canCreate   = computed(() => authStore.hasPermission('proveedores', 'crear'));
const canRead     = computed(() => authStore.hasPermission('proveedores', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('proveedores', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('proveedores', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('proveedores', 'descargar'));

const openModal = (payload: { supplier: Supplier; action: 0 | 1 | 2 }) => {
  selectedSupplier.value = payload.supplier;
  action.value           = payload.action;
  modal.value            = true;
};

const openForm = (supplier?: Supplier) => {
  selectedSupplier.value = supplier ? { ...supplier } : {
    idSupplier:    null,
    companyName:   '',
    contact:       '',
    email:         '',
    phoneNumber:   null,
    auditCreateDate: '',
    statusSupplier: ''
  };
  form.value = true;
};

const fetchSuppliers = async () => {
  try {
    await supplierStore.fetchAll({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0,
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchSuppliers = async (params: {
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
    await supplierStore.fetchAll({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar proveedores');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Empresa';
  state.value          = 'Activos';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchSuppliers();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await supplierStore.downloadExcel({
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
    await supplierStore.downloadPdf({
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
  fetchSuppliers();
};

const handleActionCompleted = () => {
  fetchSuppliers();
};

onMounted(() => {
  fetchSuppliers();
});
</script>