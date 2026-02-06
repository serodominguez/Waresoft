<template>
  <div>
    <CustomerList :customers="customers" :loading="loading" :totalCustomers="totalCustomers"
      :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead"
      :canEdit="canEdit" :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-customer="openForm"
      @fetch-customers="fetchCustomers" @search-customers="searchCustomers" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf" />

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
import { useAuthStore } from '@/stores/auth';
import { Customer } from '@/interfaces/customerInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import CustomerList from '@/components/Customer/CustomerList.vue';
import CustomerForm from '@/components/Customer/CustomerForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const customerStore = useCustomerStore();
const authStore = useAuthStore();

const toast = useToast();

const filterMap: Record<string, number> = {
  "Nombres": 1,
  "Apellidos": 2,
  "Carnet": 3
};
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Nombres', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);

const search = ref<string | null>(null);
const drawer = ref(false);

const form = ref(false);
const modal = ref(false);

const selectedCustomer = ref<Customer | null>(null);

const action = ref<0 | 1 | 2>(0);

const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

const customers = computed(() => customerStore.customers);
const loading = computed(() => customerStore.loading);
const totalCustomers = computed(() => customerStore.totalCustomers);

const canCreate = computed((): boolean => authStore.hasPermission('clientes', 'crear'));
const canRead = computed((): boolean => authStore.hasPermission('clientes', 'leer'));
const canEdit = computed((): boolean => authStore.hasPermission('clientes', 'editar'));
const canDelete = computed((): boolean => authStore.hasPermission('clientes', 'eliminar'));
const canDownload = computed((): boolean => authStore.hasPermission('clientes', 'descargar'));

const openModal = (payload: { customer: Customer; action: 0 | 1 | 2 }) => {
  selectedCustomer.value = payload.customer;
  action.value = payload.action;
  modal.value = true;
};

const openForm = (customer?: Customer) => {
  selectedCustomer.value = customer ? { ...customer } : {
    idCustomer: null,
    names: '',
    lastNames: '',
    identificationNumber: '',
    phoneNumber: null,
    auditCreateDate: '',
    statusCustomer: ''
  };
  form.value = true;
};

const fetchCustomers = async (params?: any) => {
  try {
    await customerStore.fetchCustomers(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchCustomers = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await customerStore.fetchCustomers({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar clientes');
  }
};

const refreshCustomers = () => {
  if (search.value?.trim()) {
    searchCustomers({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchCustomers();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshCustomers();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshCustomers();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await customerStore.downloadCustomersExcel({
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
    await customerStore.downloadCustomersPdf({
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
  fetchCustomers();
};

const handleActionCompleted = () => {
  fetchCustomers();
};

onMounted(() => {
  fetchCustomers();
});
</script>