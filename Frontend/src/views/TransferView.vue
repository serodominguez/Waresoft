<template>
  <div>
    <TransferList v-if="!form" :transfers="transfers" :loading="loading" :totalTransfers="totalTransfers"
      :current-page="currentPage" :downloadingExcel="downloadingExcel" :printing-pdf-id="currentPrintingId" 
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @view-transfer="openForm"
      @fetch-transfer="fetchTransfers" @search-transfer="searchTransfers" @update-items-per-page="updateItemsPerPage" 
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf" @print-pdf="printPdf" 
      @clear-filters="clearFilters" />

    <TransferForm v-if="form" v-model="form" :transfer="selectedTransfer" :transferDetails="selectedTransferDetails"
      @saved="handleSaved" @close="closeForm" />

    <CommonModal v-model="modal" :itemId="selectedTransfer?.idTransfer || 0" :item="selectedTransfer?.code || ''"
      :action="action" moduleName="transfer" entityName="Transfer" name="Traspaso" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { useToast } from 'vue-toastification';
import { useTransferStore } from '@/stores/transferStore';
import { useAuthStore } from '@/stores/authStore';
import { Transfer } from '@/interfaces/transferInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useMovementFilters } from '@/composables/useMovementFilters';
import { usePagination } from '@/composables/usePagination';
import { TransferSteteMap } from '@/constants/transferStatus';
import TransferList from '@/components/Transfer/TransferList.vue';
import TransferForm from '@/components/Transfer/TransferForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const transferStore = useTransferStore();
const authStore     = useAuthStore();
const toast         = useToast();

const { transfers, selectedTransfer, selectedTransferDetails, loading, totalTransfers } = storeToRefs(transferStore);

const filterMap: Record<string, number> = { "Código": 1, "Origen": 2, "Destino": 3 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useMovementFilters(
  'Código', filterMap, TransferSteteMap, 'Todos'
);

const search            = ref<string | null>(null);
const drawer            = ref(false);
const form              = ref(false);
const modal             = ref(false);
const action            = ref<0 | 1 | 2 | 3>(0);
const downloadingExcel  = ref(false);
const downloadingPdf    = ref(false);
const currentPrintingId = ref<number | null>(null);

const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    transferStore.fetchTransfers({
      pageNumber: params.pageNumber,
      pageSize:   params.pageSize,
      sort:       'IdTransfer',
      order:      'desc',
      ...getFilterParams(search.value),
    });
  }
);

const canCreate   = computed(() => authStore.hasPermission('traspaso de productos', 'crear'));
const canRead     = computed(() => authStore.hasPermission('traspaso de productos', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('traspaso de productos', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('traspaso de productos', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('traspaso de productos', 'descargar'));

const openModal = (payload: { transfer: Transfer; action: 0 | 1 | 2 | 3 }) => {
  transferStore.selectedItem = payload.transfer;
  action.value               = payload.action;
  modal.value                = true;
};

const openForm = async (transfer?: Transfer) => {
  if (transfer?.idTransfer) {
    try {
      await transferStore.fetchTransferById(transfer.idTransfer);
    } catch (error) {
      handleApiError(error, 'Error al cargar los detalles del traspaso');
      return;
    }
  } else {
    transferStore.selectedItem = {
      idTransfer:        null,
      code:              '',
      sendDate:          '',
      idStoreOrigin:     null,
      storeOrigin:       '',
      receiveDate:       '',
      idStoreDestination: null,
      storeDestination:  '',
      totalAmount:       0,
      annotations:       '',
      sendUser:          '',
      receiveUser:       '',
      statusTransfer:    ''
    };
  }
  form.value = true;
};

const closeForm = () => {
  transferStore.selectedTransferDetails = [];
  transferStore.selectedItem            = null;
  form.value                            = false;
  fetchTransfers();
};

const fetchTransfers = async () => {
  try {
    await transferStore.fetchTransfers({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      sort:        'IdTransfer',
      order:       'desc',
      ...getFilterParams(null),
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchTransfers = async (params: {
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
    await transferStore.fetchTransfers({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      sort:       'IdTransfer',
      order:      'desc',
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar los traspasos');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Código';
  state.value          = 'Todos';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchTransfers();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await transferStore.downloadTransferExcel({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      sort:       'IdTransfer',
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
    await transferStore.downloadTransferPdf({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      sort:       'IdTransfer',
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

const printPdf = async (transfer: Transfer) => {
  if (!transfer.idTransfer) return;

  currentPrintingId.value = transfer.idTransfer;
  try {
    await transferStore.openTransferPdf(transfer.idTransfer);
    toast.success('PDF abierto correctamente');
  } catch (error) {
    handleApiError(error, 'Error al abrir el PDF');
  } finally {
    currentPrintingId.value = null;
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