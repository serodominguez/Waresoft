<template>
  <div>
    <CategoryList :categories="categories" :loading="loading" :totalCategories="totalCategories"
      :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead"
      :canEdit="canEdit" :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @edit-category="openForm" @fetch-categories="fetchCategories" @search-categories="searchCategories"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel"
      @download-pdf="downloadPdf" />

    <CategoryForm v-model="form" :category="selectedCategory" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedCategory?.idCategory || 0"
      :item="selectedCategory?.categoryName || ''" :action="action" moduleName="category" entityName="Category"
      name="Categoría" gender="female" @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useCategoryStore } from '@/stores/categoryStore';
import { useAuthStore } from '@/stores/auth';
import { Category } from '@/interfaces/categoryInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import CategoryList from '@/components/Category/CategoryList.vue';
import CategoryForm from '@/components/Category/CategoryForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const categoryStore = useCategoryStore();
const authStore = useAuthStore();

const toast = useToast();

const filterMap: Record<string, number> = {
  "Categoría": 1,
  "Descripción": 2
};
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Categoría', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);

const search = ref<string | null>(null);
const drawer = ref(false);

const form = ref(false);
const modal = ref(false);

const selectedCategory = ref<Category | null>(null);

const action = ref<0 | 1 | 2>(0);

const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

const categories = computed(() => categoryStore.categories);
const loading = computed(() => categoryStore.loading);
const totalCategories = computed(() => categoryStore.totalCategories);

const canCreate = computed((): boolean => authStore.hasPermission('categorias', 'crear'));
const canRead = computed((): boolean => authStore.hasPermission('categorias', 'leer'));
const canEdit = computed((): boolean => authStore.hasPermission('categorias', 'editar'));
const canDelete = computed((): boolean => authStore.hasPermission('categorias', 'eliminar'));
const canDownload = computed((): boolean => authStore.hasPermission('categorias', 'descargar'));

const openModal = (payload: { category: Category, action: 0 | 1 | 2 }) => {
  selectedCategory.value = payload.category;
  action.value = payload.action;
  modal.value = true;
};

const openForm = (category?: Category) => {
  selectedCategory.value = category ? { ...category } : {
    idCategory: null,
    categoryName: '',
    description: '',
    auditCreateDate: '',
    statusCategory: ''
  };
  form.value = true;
};

const fetchCategories = async (params?: any) => {
  try {
    await categoryStore.fetchCategories(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchCategories = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await categoryStore.fetchCategories({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar categorías');
  }
};

const refreshCategories = () => {
  if (search.value?.trim()) {
    searchCategories({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchCategories();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshCategories();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshCategories();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await categoryStore.downloadCategoriesExcel({
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
    await categoryStore.downloadCategoriesPdf({
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
  fetchCategories();
};

const handleActionCompleted = () => {
  fetchCategories();
};

onMounted(() => {
  fetchCategories();
});
</script>