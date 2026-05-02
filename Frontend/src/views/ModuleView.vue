<template>
  <div>
    <ModuleList :modules="modules" :loading="loading" :totalModules="totalModules" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-module="openForm"
      @fetch-modules="fetchModules" @search-modules="searchModules" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf"
      @clear-filters="clearFilters" />

    <ModuleForm v-model="form" :module="selectedModule" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedModule?.idModule || 0" :item="selectedModule?.moduleName || ''"
      :action="action" moduleName="module" entityName="Module" name="Módulo" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useModuleStore } from '@/stores/moduleStore';
import { useAuthStore } from '@/stores/authStore';
import { Module } from '@/interfaces/moduleInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import { usePagination } from '@/composables/usePagination';
import ModuleList from '@/components/Module/ModuleList.vue';
import ModuleForm from '@/components/Module/ModuleForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const moduleStore = useModuleStore();
const authStore   = useAuthStore();
const toast       = useToast();

const filterMap: Record<string, number> = { "Módulo": 1 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Módulo', filterMap);

const search         = ref<string | null>(null);
const drawer         = ref(false);
const form           = ref(false);
const modal          = ref(false);
const selectedModule = ref<Module | null>(null);
const action         = ref<0 | 1 | 2>(0);
const downloadingExcel = ref(false);
const downloadingPdf   = ref(false);

const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    moduleStore.fetchAll({
      pageNumber:  params.pageNumber,
      pageSize:    params.pageSize,
      ...getFilterParams(search.value),
    });
  }
);

const modules     = computed(() => moduleStore.list);
const loading     = computed(() => moduleStore.loading);
const totalModules = computed(() => moduleStore.total);

const canCreate   = computed(() => authStore.hasPermission('modulos', 'crear'));
const canRead     = computed(() => authStore.hasPermission('modulos', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('modulos', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('modulos', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('modulos', 'descargar'));

const openModal = (payload: { module: Module; action: 0 | 1 | 2 }) => {
  selectedModule.value = payload.module;
  action.value         = payload.action;
  modal.value          = true;
};

const openForm = (module?: Module) => {
  selectedModule.value = module ? { ...module } : {
    idModule:      null,
    moduleName:    '',
    auditCreateDate: '',
    statusModule:  ''
  };
  form.value = true;
};

const fetchModules = async () => {
  try {
    await moduleStore.fetchAll({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0,
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchModules = async (params: {
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
    await moduleStore.fetchAll({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar módulos');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Módulo';
  state.value          = 'Activos';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchModules();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await moduleStore.downloadExcel({
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
    await moduleStore.downloadPdf({
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
  fetchModules();
};

const handleActionCompleted = () => {
  fetchModules();
};

onMounted(() => {
  fetchModules();
});
</script>