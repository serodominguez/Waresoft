<template>
  <div>
    <CustomerList :customers="customers" :loading="loading" :totalCustomers="totalCustomers" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-customer="openForm"
      @fetch-customers="fetchCustomers" @search-customers="searchCustomers" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf"
      @clear-filters="clearFilters" />

    <CustomerForm v-model="form" :customer="selectedCustomer" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedCustomer?.idCustomer || 0" :item="selectedCustomer?.names || ''"
      :action="action" moduleName="customer" entityName="Customer" name="Cliente" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useCustomerStore } from '@/stores/customerStore';
import { useAuthStore } from '@/stores/authStore';
import { Customer } from '@/interfaces/customerInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import { usePagination } from '@/composables/usePagination';
import CustomerList from '@/components/Customer/CustomerList.vue';
import CustomerForm from '@/components/Customer/CustomerForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const customerStore = useCustomerStore();
const authStore     = useAuthStore();
const toast         = useToast();

const filterMap: Record<string, number> = { "Nombres": 1, "Apellidos": 2, "Carnet": 3 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Nombres', filterMap);

const search           = ref<string | null>(null);
const drawer           = ref(false);
const form             = ref(false);
const modal            = ref(false);
const selectedCustomer = ref<Customer | null>(null);
const action           = ref<0 | 1 | 2>(0);
const downloadingExcel = ref(false);
const downloadingPdf   = ref(false);

const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    customerStore.fetchAll({
      pageNumber:  params.pageNumber,
      pageSize:    params.pageSize,
      ...getFilterParams(search.value),
    });
  }
);

const customers     = computed(() => customerStore.list);
const loading       = computed(() => customerStore.loading);
const totalCustomers = computed(() => customerStore.total);

const canCreate   = computed(() => authStore.hasPermission('clientes', 'crear'));
const canRead     = computed(() => authStore.hasPermission('clientes', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('clientes', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('clientes', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('clientes', 'descargar'));

const openModal = (payload: { customer: Customer; action: 0 | 1 | 2 }) => {
  selectedCustomer.value = payload.customer;
  action.value           = payload.action;
  modal.value            = true;
};

const openForm = (customer?: Customer) => {
  selectedCustomer.value = customer ? { ...customer } : {
    idCustomer:        null,
    names:             '',
    lastNames:         '',
    identificationNumber: '',
    phoneNumber:       null,
    auditCreateDate:   '',
    statusCustomer:    ''
  };
  form.value = true;
};

const fetchCustomers = async () => {
  try {
    await customerStore.fetchAll({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0,
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchCustomers = async (params: {
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
    await customerStore.fetchAll({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar clientes');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Nombres';
  state.value          = 'Activos';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchCustomers();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await customerStore.downloadExcel({
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
    await customerStore.downloadPdf({
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
  fetchCustomers();
};

const handleActionCompleted = () => {
  fetchCustomers();
};


onMounted(() => {
  fetchCustomers();
});
</script>