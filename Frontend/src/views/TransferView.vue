<template>
  <div>
    <TransferList v-if="!form" :transfers="transfers" :loading="loading" :totalTransfers="totalTransfers"
      :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead"
      :canEdit="canEdit" :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @view-transfer="openForm" @fetch-transfer="fetchTransfers" @search-transfer="searchTransfers"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel"
      @download-pdf="downloadPdf" @print-pdf="printPdf" />

    <TransferForm v-if="form" v-model="form" :transfer="selectedTransfer" :transferDetails="selectedTransferDetails"
      @saved="handleSaved" @close="closeForm" />

    <CommonModal v-model="modal" :itemId="selectedTransfer?.idTransfer || 0" :item="selectedTransfer?.code || ''"
      :action="action" moduleName="transfer" entityName="Transfer" name="Traspaso" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { storeToRefs } from 'pinia';
import { useTransferStore } from '@/stores/transferStore';
import { useAuthStore } from '@/stores/auth';
import { Transfer } from '@/interfaces/transferInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import TransferList from '@/components/Transfer/TransferList.vue';
import TransferForm from '@/components/Transfer/TransferForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const transferStore = useTransferStore();
const authStore = useAuthStore();
const toast = useToast();

const { transfers, selectedTransfer, selectedTransferDetails, loading, totalTransfers } = storeToRefs(transferStore);

const filterMap = {
  "Código": 1,
  "Tienda": 2
};

const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Código', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);
const search = ref<string | null>(null);
const drawer = ref(false);
const form = ref(false);
const modal = ref(false);
const action = ref<0 | 1 | 2>(0);

const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

const stateFilter = computed(() => state.value === 'Activos' ? 1 : 0);


const canCreate = computed(() => authStore.hasPermission('traspaso de productos', 'crear'));
const canRead = computed(() => authStore.hasPermission('traspaso de productos', 'leer'));
const canEdit = computed(() => authStore.hasPermission('traspaso de productos', 'editar'));
const canDelete = computed(() => authStore.hasPermission('traspaso de productos', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('traspaso de productos', 'descargar'));

const openModal = (payload: { transfer: Transfer, action: 0 | 1 | 2 }) => {
  transferStore.selectedItem = payload.transfer;
  action.value = payload.action;
  modal.value = true;
};

const openForm = async (transfer?: Transfer) => {
  if (transfer?.idTransfer) {
    try {
      await transferStore.fetchTransferById(transfer?.idTransfer);
    } catch (error) {
      handleApiError(error, 'Error al cargar los detalles de la salida');
      return;
    }
  } else {
    transferStore.selectedItem = {
      idTransfer: null,
      code: '',
      sendDate: '',
      idStoreOrigin: null,
      storeOrigin: '',
      receiveDate: '',
      idStoreDestination: null,
      storeDestination: '',
      totalAmount: 0,
      annotations: '',
      userName: '',
      statusTransfer: ''
    };
  }

  form.value = true;
};

const closeForm = () => {
  transferStore.selectedTransferDetails = [];
  transferStore.selectedItem = null;
  form.value = false;
};

const fetchTransfers = async (params?: any) => {
  try {
    await transferStore.fetchTransfers(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: stateFilter.value,
      sort: 'IdTransfer',
      order: 'desc'
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchTransfers = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await transferStore.fetchTransfers({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      sort: 'IdTransfer',
      order: 'desc',
      ...getFilterParams(search.value)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar los traspasos');
  }
};

const refreshTransfers = () => {
  if (search.value?.trim()) {
    searchTransfers({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchTransfers();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshTransfers();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshTransfers();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await transferStore.downloadTransferExcel({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      sort: 'IdTransfer',
      order: 'desc',
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
    await transferStore.downloadTransferPdf({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      sort: 'IdTransfer',
      order: 'desc',
      ...getFilterParams(params.search)
    });
    toast.success('Archivo PDF descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo PDF');
  } finally {
    downloadingPdf.value = false;
  }
};

const printPdf = async (transfer: Transfer) => {
  if (!transfer.idTransfer) return;

  try {
    const result = await transferStore.openTransferPdf(transfer.idTransfer);

    if (result.isSuccess) {
      toast.success('PDF abierto correctamente');
    } else {
      toast.error('Error al abrir el PDF');
    }
  } catch (error) {
    handleApiError(error, 'Error al abrir el PDF');
  }
};

const handleSaved = () => {
  fetchTransfers();
};

const handleActionCompleted = () => {
  fetchTransfers();
};

onMounted(() => {
  fetchTransfers();
});
</script>