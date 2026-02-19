<template>
  <div>
    <StockList :stores="stores" :rows="rows" :totalRows="totalRows" :loading="loading" :canRead="canRead" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @fetch-stock="fetchStock" @search-stock="searchStock"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useInventoryStore } from '@/stores/inventoryStore';
import { useAuthStore } from '@/stores/auth';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import StockList from '@/components/Inventory/StockList.vue';

const inventoryStore = useInventoryStore();
const authStore = useAuthStore();

const filterMap: Record<string, number> = {
  'Código': 1,
  'Descripción': 2,
  'Material': 3,
  'Color': 4,
  'Marca': 5,
  'Categoría': 6,
};

const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Código', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);
const search = ref<string | null>(null);
const drawer = ref(false);

const loading = computed(() => inventoryStore.loading);
const totalRows = computed(() => inventoryStore.totalRows);
const stores = computed(() => inventoryStore.inventoryPivot?.stores ?? []);
const rows = computed(() => inventoryStore.inventoryPivot?.rows ?? []);

const canRead = computed((): boolean => authStore.hasPermission('inventario', 'leer'));

const fetchStock = async (params?: any) => {
  try {
    await inventoryStore.fetchInventoryPivot(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      sort: 'IdProduct',
      order: 'asc',
      ...getFilterParams(search.value)
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchStock = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await inventoryStore.fetchInventoryPivot({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar stock');
  }
};

const refreshStock = () => {
  if (search.value?.trim()) {
    searchStock({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchStock();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshStock();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshStock();
};

onMounted(() => {
  fetchStock();
});
</script>