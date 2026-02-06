<template>
  <div>
    <ModuleList :modules="modules" :loading="loading" :totalModules="totalModules" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-module="openForm"
      @fetch-modules="fetchModules" @search-modules="searchModules" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf" />

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
import { useAuthStore } from '@/stores/auth';
import { Module } from '@/interfaces/moduleInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import ModuleList from '@/components/Module/ModuleList.vue';
import ModuleForm from '@/components/Module/ModuleForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const moduleStore = useModuleStore();
const authStore = useAuthStore();

const toast = useToast();

const filterMap: Record<string, number> = { "Módulo": 1 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Módulo', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);

const search = ref<string | null>(null);
const drawer = ref(false);

const form = ref(false);
const modal = ref(false);

const selectedModule = ref<Module | null>(null);

const action = ref<0 | 1 | 2>(0);

const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

const modules = computed(() => moduleStore.modules);
const loading = computed(() => moduleStore.loading);
const totalModules = computed(() => moduleStore.totalModules);

const canCreate = computed((): boolean => authStore.hasPermission('modulos', 'crear'));
const canRead = computed((): boolean => authStore.hasPermission('modulos', 'leer'));
const canEdit = computed((): boolean => authStore.hasPermission('modulos', 'editar'));
const canDelete = computed((): boolean => authStore.hasPermission('modulos', 'eliminar'));
const canDownload = computed((): boolean => authStore.hasPermission('modulos', 'descargar'));

const openModal = (payload: { module: Module, action: 0 | 1 | 2 }) => {
  selectedModule.value = payload.module;
  action.value = payload.action;
  modal.value = true;
};

const openForm = (module?: Module) => {
  selectedModule.value = module ? { ...module } : {
    idModule: null,
    moduleName: '',
    auditCreateDate: '',
    statusModule: ''
  };
  form.value = true;
};

const fetchModules = async (params?: any) => {
  try {
    await moduleStore.fetchModules(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchModules = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await moduleStore.fetchModules({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar módulos');
  }
};

const refreshModules = () => {
  if (search.value?.trim()) {
    searchModules({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchModules();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshModules();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshModules();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await moduleStore.downloadModulesExcel({
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
    await moduleStore.downloadModulesPdf({
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
  fetchModules();
};

const handleActionCompleted = () => {
  fetchModules();
};

onMounted(() => {
  fetchModules();
});
</script>