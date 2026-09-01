<template>
    <div>
        <InventoryPeriodList :periods="periods" :loading="loading" :totalPeriods="totalPeriods" :canCreate="canCreate"
            :canEdit="canEdit" :canDownload="canDownload" :items-per-page="itemsPerPage"
            :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" v-model:drawer="drawer"
            v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
            v-model:endDate="endDate" @open-form="openForm" @open-close-modal="openCloseModal"
            @update-items-per-page="updateItemsPerPage" @change-page="changePage" @search-periods="searchPeriods"
            @download-excel="downloadExcel" @download-pdf="downloadPdf" @clear-filters="clearFilters" />

        <InventoryPeriodForm v-model="form" @saved="handleSaved" />

        <InventoryPeriodCloseModal v-model="closeModal" :period="selectedPeriod" @closed="handleClosed"
            @go-to-physical-stock="handleGoToPhysicalStock" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useInventoryPeriodStore } from '@/stores/inventoryPeriodStore';
import { useAuthStore } from '@/stores/authStore';
import { useToast } from 'vue-toastification';
import { InventoryPeriod } from '@/interfaces/inventoryPeriodInterface';
import { handleSilentError, handleApiError } from '@/helpers/errorHandler';
import { useMovementFilters } from '@/composables/useMovementFilters';
import { usePagination } from '@/composables/usePagination';
import { PeriodStateMap } from '@/constants/periodStatus';
import InventoryPeriodList from '@/components/InventoryPeriod/InventoryPeriodList.vue';
import InventoryPeriodForm from '@/components/InventoryPeriod/InventoryPeriodOpenForm.vue';
import InventoryPeriodCloseModal from '@/components/InventoryPeriod/InventoryPeriodCloseModal.vue';

const periodStore = useInventoryPeriodStore();
const authStore   = useAuthStore();
const toast       = useToast();

// ─── Filtros ──────────────────────────────────────────────────────────────────
const filterMap = { 'Período': 1 };

const { selectedFilter, state, startDate, endDate, getFilterParams } = useMovementFilters(
    'Período', filterMap, PeriodStateMap, 'Abierto'
);

// ─── Estado local ─────────────────────────────────────────────────────────────
const search          = ref<string | null>(null);
const drawer          = ref(false);
const form            = ref(false);
const closeModal      = ref(false);
const selectedPeriod  = ref<InventoryPeriod | null>(null);
const downloadingExcel = ref(false);
const downloadingPdf   = ref(false);

// ─── Paginación ───────────────────────────────────────────────────────────────
const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
    (params) => periodStore.fetchPeriods({
        pageNumber: params.pageNumber,
        pageSize:   params.pageSize,
        sort:       'IdPeriod',
        ...getFilterParams(search.value),
    })
);

// ─── Computed ─────────────────────────────────────────────────────────────────
const periods      = computed(() => periodStore.list);
const loading      = computed(() => periodStore.loading);
const totalPeriods = computed(() => periodStore.total);

const canCreate   = computed(() => authStore.hasPermission('periodo', 'crear'));
const canEdit     = computed(() => authStore.hasPermission('periodo', 'editar'));
const canDownload = computed(() => authStore.hasPermission('periodo', 'descargar'));

// ─── Métodos ──────────────────────────────────────────────────────────────────
const openForm = () => { form.value = true; };

const openCloseModal = (period: InventoryPeriod) => {
    selectedPeriod.value = period;
    closeModal.value = true;
};

const fetchPeriods = async () => {
    try {
        await periodStore.fetchPeriods({
            pageNumber: currentPage.value,
            pageSize:   itemsPerPage.value,
            sort:       'IdPeriod',
            ...getFilterParams(search.value),
        });
    } catch (error) {
        handleSilentError(error);
    }
};

const searchPeriods = async (params: {
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
        await periodStore.fetchPeriods({
            pageNumber: 1,
            pageSize:   itemsPerPage.value,
            sort:       'IdPeriod',
            ...getFilterParams(params.search),
        });
    } catch (error) {
        handleApiError(error, 'Error al buscar los períodos');
    }
};

const clearFilters = () => {
    search.value         = null;
    selectedFilter.value = 'Período';
    state.value          = 'Abierto';
    startDate.value      = null;
    endDate.value        = null;
    currentPage.value    = 1;
    fetchPeriods();
};

const downloadExcel = async (params: { search: string | null }) => {
    downloadingExcel.value = true;
    try {
        await periodStore.downloadPeriodsExcel({
            pageNumber: currentPage.value,
            pageSize:   itemsPerPage.value,
            sort:       'IdPeriod',
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
        await periodStore.downloadPeriodsPdf({
            pageNumber: currentPage.value,
            pageSize:   itemsPerPage.value,
            sort:       'IdPeriod',
            ...getFilterParams(params.search),
        });
        toast.success('Archivo PDF descargado correctamente');
    } catch (error) {
        handleApiError(error, 'Error al descargar el archivo PDF');
    } finally {
        downloadingPdf.value = false;
    }
};

const handleSaved  = () => fetchPeriods();
const handleClosed = () => fetchPeriods();
const handleGoToPhysicalStock = (period: InventoryPeriod) => {
    console.log('Ir a stock físico del período:', period.idPeriod);
};

onMounted(() => fetchPeriods());
</script>