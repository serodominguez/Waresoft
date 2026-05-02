<template>
  <div>
    <GoodsReceiptList v-if="!form" :goodsreceipt="goodsreceipt" :loading="loading"
      :totalGoodsReceipt="totalGoodsReceipt" :current-page="currentPage" :downloadingExcel="downloadingExcel"
      :printing-pdf-id="currentPrintingId" :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead"
      :canEdit="canEdit" :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @view-goodsreceipt="openForm" @fetch-goodsreceipt="fetchGoodsReceipt" @search-goodsreceipt="searchGoodsReceipt"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel"
      @download-pdf="downloadPdf" @print-pdf="printPdf" @clear-filters="clearFilters" />

    <GoodsReceiptForm v-if="form" v-model="form" :receipt="selectedGoodsReceipt"
      :receiptDetails="selectedReceiptDetails" @saved="handleSaved" @close="closeForm" />

    <CommonModal v-model="modal" :itemId="selectedGoodsReceipt?.idReceipt || 0" :item="selectedGoodsReceipt?.code || ''"
      :action="action" moduleName="goodsreceipt" entityName="GoodsReceipt" name="Entrada" gender="female"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { useToast } from 'vue-toastification';
import { useGoodsReceiptStore } from '@/stores/goodsReceiptStore';
import { useAuthStore } from '@/stores/authStore';
import { GoodsReceipt } from '@/interfaces/goodsReceiptInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useMovementFilters } from '@/composables/useMovementFilters';
import { usePagination } from '@/composables/usePagination';
import { GoodsStateMap } from '@/constants/goodsStatus';
import GoodsReceiptList from '@/components/GoodsReceipt/GoodsReceiptList.vue';
import GoodsReceiptForm from '@/components/GoodsReceipt/GoodsReceiptForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

// ─── Stores ───────────────────────────────────────────────────────────────────
const goodsReceiptStore = useGoodsReceiptStore();
const authStore         = useAuthStore();
const toast             = useToast();

const { goodsreceipt, selectedGoodsReceipt, selectedReceiptDetails, loading, totalGoodsReceipt } =
  storeToRefs(goodsReceiptStore);

// ─── Filtros ──────────────────────────────────────────────────────────────────
const filterMap = {
  "Código":   1,
  "Tienda":   2,
  "Proveedor": 3
};

const { selectedFilter, state, startDate, endDate, getFilterParams } = useMovementFilters(
  'Código',
  filterMap,
  GoodsStateMap,
  'Completado'
);

// ─── Estado local ─────────────────────────────────────────────────────────────
const search           = ref<string | null>(null);
const drawer           = ref(false);
const form             = ref(false);
const modal            = ref(false);
const action           = ref<0 | 1 | 2 | 3>(0);
const downloadingExcel  = ref(false);
const downloadingPdf    = ref(false);
const currentPrintingId = ref<number | null>(null);

// ─── Paginación ───────────────────────────────────────────────────────────────
// El callback recibe pageNumber y pageSize desde el composable
// y los combina con los parámetros fijos de movimientos (sort, order)
const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    goodsReceiptStore.fetchGoodsReceipt({
      pageNumber: params.pageNumber,
      pageSize:   params.pageSize,
      sort:       'IdReceipt',
      order:      'desc',
      ...getFilterParams(search.value),
    });
  }
);

// ─── Computed ─────────────────────────────────────────────────────────────────
const canCreate   = computed(() => authStore.hasPermission('entrada de productos', 'crear'));
const canRead     = computed(() => authStore.hasPermission('entrada de productos', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('entrada de productos', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('entrada de productos', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('entrada de productos', 'descargar'));

// ─── Métodos ──────────────────────────────────────────────────────────────────

const openModal = (payload: { goodsreceipt: GoodsReceipt; action: 0 | 1 | 2 | 3 }) => {
  goodsReceiptStore.selectedItem = payload.goodsreceipt;
  action.value                   = payload.action;
  modal.value                    = true;
};

const openForm = async (goodsreceipt?: GoodsReceipt) => {
  if (goodsreceipt?.idReceipt) {
    try {
      await goodsReceiptStore.fetchGoodsReceiptById(goodsreceipt.idReceipt);
    } catch (error) {
      handleApiError(error, 'Error al cargar los detalles de la entrada');
      return;
    }
  } else {
    goodsReceiptStore.selectedItem = {
      idReceipt:       null,
      code:            '',
      type:            '',
      storeName:       '',
      documentType:    '',
      documentNumber:  '',
      documentDate:    '',
      idSupplier:      null,
      companyName:     '',
      totalAmount:     0,
      annotations:     '',
      auditCreateDate: '',
      statusReceipt:   ''
    };
  }
  form.value = true;
};

const closeForm = () => {
  goodsReceiptStore.selectedReceiptDetails = [];
  goodsReceiptStore.selectedItem           = null;
  form.value                               = false;
  fetchGoodsReceipt();
};

// Carga inicial y recarga tras acciones (guardar, cancelar)
const fetchGoodsReceipt = async () => {
  try {
    await goodsReceiptStore.fetchGoodsReceipt({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      sort:       'IdReceipt',
      order:      'desc',
      ...getFilterParams(null),
    });
  } catch (error) {
    handleSilentError(error);
  }
};

// Búsqueda con filtros activos
const searchGoodsReceipt = async (params: {
  search: string | null;
  selectedFilter: string;
  state: string;
  startDate: Date | null;
  endDate: Date | null;
}) => {
  search.value         = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value          = params.state;
  startDate.value      = params.startDate;
  endDate.value        = params.endDate;
  currentPage.value    = 1;

  try {
    await goodsReceiptStore.fetchGoodsReceipt({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      sort:       'IdReceipt',
      order:      'desc',
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar las entradas');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Código';
  state.value          = 'Completado';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchGoodsReceipt();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await goodsReceiptStore.downloadGoodsReceiptExcel({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      sort:       'IdReceipt',
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
    await goodsReceiptStore.downloadGoodsReceiptPdf({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      sort:       'IdReceipt',
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

const printPdf = async (goodsreceipt: GoodsReceipt) => {
  if (!goodsreceipt.idReceipt) return;

  currentPrintingId.value = goodsreceipt.idReceipt;
  try {
      await goodsReceiptStore.openGoodsReceiptPdf(goodsreceipt.idReceipt);
      toast.success('PDF abierto correctamente');
  } catch (error) {
    handleApiError(error, 'Error al abrir el PDF');
  } finally {
    currentPrintingId.value = null;
  }
};

const handleSaved = () => {
  fetchGoodsReceipt();
};

const handleActionCompleted = () => {
  fetchGoodsReceipt();
};

// ─── Lifecycle ────────────────────────────────────────────────────────────────
onMounted(() => {
  fetchGoodsReceipt();
});
</script>