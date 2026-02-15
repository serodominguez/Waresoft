<template>
 <div>
    <GoodsReceiptList v-if="!form" :goodsreceipt="goodsreceipt" :loading="loading"
      :totalGoodsReceipt="totalGoodsReceipt" :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf"
      :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" :canDownload="canDownload"
      :items-per-page="itemsPerPage" v-model:drawer="drawer" v-model:selectedFilter="selectedFilter"
      v-model:state="state" v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm"
      @open-modal="openModal" @view-goodsreceipt="openForm" @fetch-goodsreceipt="fetchGoodsReceipt"
      @search-goodsreceipt="searchGoodsReceipt" @update-items-per-page="updateItemsPerPage" @change-page="changePage"
      @download-excel="downloadExcel" @download-pdf="downloadPdf" @print-pdf="printPdf" @clear-filters="clearFilters" />

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
import { useAuthStore } from '@/stores/auth';
import { GoodsReceipt } from '@/interfaces/goodsReceiptInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useMovementFilters } from '@/composables/useMovementFilters';
import { GOODS_STATE_MAP } from '@/constants/goodsStatus';
import GoodsReceiptList from '@/components/GoodsReceipt/GoodsReceiptList.vue';
import GoodsReceiptForm from '@/components/GoodsReceipt/GoodsReceiptForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const goodsReceiptStore = useGoodsReceiptStore();
const authStore = useAuthStore();
const toast = useToast();

// Destructuring store con storeToRefs para mantener la reactividad
const { goodsreceipt, selectedGoodsReceipt, selectedReceiptDetails, loading, totalGoodsReceipt } = storeToRefs(goodsReceiptStore);

// Composable de filtros
const filterMap = {
  "Código": 1,
  "Tienda": 2,
  "Proveedor": 3
};

const { selectedFilter, state, startDate, endDate, getFilterParams } = useMovementFilters(
  'Código',
  filterMap,
  GOODS_STATE_MAP,
  'Completado'
);

// Data
const currentPage = ref(1);
const itemsPerPage = ref(10);
const search = ref<string | null>(null);
const drawer = ref(false);
const form = ref(false);
const modal = ref(false);
const action = ref<0 | 1 | 2| 3>(0);
  
const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

// Computed
const canCreate = computed(() => authStore.hasPermission('entrada de productos', 'crear'));
const canRead = computed(() => authStore.hasPermission('entrada de productos', 'leer'));
const canEdit = computed(() => authStore.hasPermission('entrada de productos', 'editar'));
const canDelete = computed(() => authStore.hasPermission('entrada de productos', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('entrada de productos', 'descargar'));

const clearFilters = () => {
  selectedFilter.value = 'Código';
  state.value = 'Completado';
  startDate.value = null;
  endDate.value = null;
  search.value = null;
  
  fetchGoodsReceipt();
};

// Methods
const openModal = (payload: { goodsreceipt: GoodsReceipt, action: 0 | 1 | 2 | 3 }) => {
  goodsReceiptStore.selectedItem = payload.goodsreceipt;
  action.value = payload.action;
  modal.value = true;
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
      idReceipt: null,
      code: '',
      type: '',
      storeName: '',
      documentType: '',
      documentNumber: '',
      documentDate: '',
      idSupplier: null,
      companyName: '',
      totalAmount: 0,
      annotations: '',
      auditCreateDate: '',
      statusReceipt: ''
    };
  }

  form.value = true;
};

const closeForm = () => {
  goodsReceiptStore.selectedReceiptDetails = [];
  goodsReceiptStore.selectedItem = null;
  form.value = false;
};

const fetchGoodsReceipt = async (params?: any) => {
  try {
    await goodsReceiptStore.fetchGoodsReceipt(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      sort: 'IdReceipt',
      order: 'desc',
      ...getFilterParams(null),
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchGoodsReceipt = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await goodsReceiptStore.fetchGoodsReceipt({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      sort: 'IdReceipt',
      order: 'desc',
      ...getFilterParams(search.value)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar las entradas');
  }
};

const refreshGoodsReceipt = () => {
  if (search.value?.trim()) {
    searchGoodsReceipt({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchGoodsReceipt();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshGoodsReceipt();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshGoodsReceipt();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await goodsReceiptStore.downloadGoodsReceiptExcel({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      sort: 'IdReceipt',
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
    await goodsReceiptStore.downloadGoodsReceiptPdf({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      sort: 'IdReceipt',
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

const printPdf = async (goodsreceipt: GoodsReceipt) => {
  if (!goodsreceipt.idReceipt) return;

  try {
    const result = await goodsReceiptStore.openGoodsReceiptPdf(goodsreceipt.idReceipt);

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
  fetchGoodsReceipt();
};

const handleActionCompleted = () => {
  fetchGoodsReceipt();
};

// Lifecycle
onMounted(() => {
  fetchGoodsReceipt();
});
</script>