<template>
  <div>
    <CategoryList :categories="categories" :loading="loading" :totalCategories="totalCategories" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-category="openForm"
      @fetch-categories="fetchCategories" @search-categories="searchCategories" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf"
      @clear-filters="clearFilters" />

    <CategoryForm v-model="form" :category="selectedCategory" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedCategory?.idCategory || 0" :item="selectedCategory?.categoryName || ''"
      :action="action" moduleName="category" entityName="Category" name="Categoría" gender="female"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useCategoryStore } from '@/stores/categoryStore';
import { useAuthStore } from '@/stores/authStore';
import { Category } from '@/interfaces/categoryInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import { usePagination } from '@/composables/usePagination';
import CategoryList from '@/components/Category/CategoryList.vue';
import CategoryForm from '@/components/Category/CategoryForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const categoryStore = useCategoryStore();
const authStore     = useAuthStore();
const toast         = useToast();

const filterMap: Record<string, number> = { "Categoría": 1, "Descripción": 2 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Categoría', filterMap);

const search            = ref<string | null>(null);
const drawer            = ref(false);
const form              = ref(false);
const modal             = ref(false);
const selectedCategory  = ref<Category | null>(null);
const action            = ref<0 | 1 | 2>(0);
const downloadingExcel  = ref(false);
const downloadingPdf    = ref(false);

const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    categoryStore.fetchAll({
      pageNumber:  params.pageNumber,
      pageSize:    params.pageSize,
      ...getFilterParams(search.value),
    });
  }
);

const categories    = computed(() => categoryStore.list);
const loading       = computed(() => categoryStore.loading);
const totalCategories = computed(() => categoryStore.total);

const canCreate   = computed(() => authStore.hasPermission('categorias', 'crear'));
const canRead     = computed(() => authStore.hasPermission('categorias', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('categorias', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('categorias', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('categorias', 'descargar'));

const openModal = (payload: { category: Category; action: 0 | 1 | 2 }) => {
  selectedCategory.value = payload.category;
  action.value           = payload.action;
  modal.value            = true;
};

const openForm = (category?: Category) => {
  selectedCategory.value = category ? { ...category } : {
    idCategory:      null,
    categoryName:    '',
    description:     '',
    auditCreateDate: '',
    statusCategory:  ''
  };
  form.value = true;
};

const fetchCategories = async () => {
  try {
    await categoryStore.fetchAll({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0,
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchCategories = async (params: {
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
    await categoryStore.fetchAll({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar categorías');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Categoría';
  state.value          = 'Activos';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchCategories();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await categoryStore.downloadExcel({
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
    await categoryStore.downloadPdf({
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
  fetchCategories();
};

const handleActionCompleted = () => {
  fetchCategories();
};

onMounted(() => {
  fetchCategories();
});
</script>