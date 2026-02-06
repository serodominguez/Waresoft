<template>
  <v-dialog v-model="isOpen" max-width="1200px" persistent>
    <v-card elevation="2">
      <v-card-title class="bg-surface-light pt-4">
        <span>Seleccione el Producto</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-row justify="center" align="end">
          <v-col cols="4" md="2" lg="2" xl="2" class="mb-2">
            <v-select color="indigo" variant="underlined" v-model="selectedFilter" :items="filterOptions"
              label="Opciones" hide-details />
          </v-col>
          <v-col cols="8" md="6" lg="6" xl="6" class="mb-2">
            <v-text-field append-inner-icon="search" density="compact" label="Búsqueda" variant="underlined"
              hide-details single-line v-model="search" @click:append-inner="handleSearch"
              @keyup.enter="handleSearch" />
          </v-col>
        </v-row>
        <v-data-table-server :key="tableKey" :headers="headers" :items="products" :items-per-page-options="[5, 10]"
          :items-per-page-text="pages" :items-per-page="itemsPerPage" :items-length="totalProducts" :loading="loading"
          loading-text="Cargando... Espere por favor" @update:items-per-page="handleItemsPerPageUpdate"
          @update:page="handlePageChange">
          <template v-slot:item="{ item }">
            <tr>
              <td>{{ item.code }}</td>
              <td>{{ item.description }}</td>
              <td>{{ item.material }}</td>
              <td>{{ item.color }}</td>
              <td>{{ item.categoryName }}</td>
              <td>{{ item.brandName }}</td>
              <td class="text-center">{{ item.price }}</td>
              <td class="text-center">{{ item.stockAvailable }}</td>
              <td class="text-center">
                <v-btn color="blue" icon="add" variant="text" @click="handleProductAdd(item)" size="small"
                  title="Agregar" />
              </td>
            </tr>
          </template>
          <template v-slot:no-data>
            <div class="text-center py-4">
              <p v-if="!loading">{{ noDataMessage }}</p>
            </div>
          </template>
        </v-data-table-server>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="red" dark class="mb-2" elevation="4" @click="handleClose">Cerrar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useInventoryStore } from '@/stores/inventoryStore';
import { Inventory } from '@/interfaces/inventoryInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface Props {
  modelValue: boolean;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'close': [];
  'product-added': [product: Inventory];
}>();

const inventoryStore = useInventoryStore();

const tableKey = ref(0);
const pages = ref("Productos por Página");
const currentPage = ref(1);
const itemsPerPage = ref(5);

const search = ref<string | null>(null);
const selectedFilter = ref('');
const filterOptions = ref(['Código', 'Descripción', 'Material', 'Color', 'Categoría', 'Marca']);

const hasSearched = ref(false);
const products = ref<Inventory[]>([]);
const totalProducts = ref(0);
const loading = ref(false);

const FILTER_MAP: Record<string, number> = {
  "Código": 1,
  "Descripción": 2,
  "Material": 3,
  "Color": 4,
  "Marca": 5,
  "Categoría": 6
};

const headers = computed(() => [
  { title: 'Código', key: 'code', sortable: false },
  { title: 'Descripción', key: 'description', sortable: false },
  { title: 'Material', key: 'material', sortable: false },
  { title: 'Color', key: 'color', sortable: false },
  { title: 'Categoría', key: 'categoryName', sortable: false },
  { title: 'Marca', key: 'brandName', sortable: false },
  { title: 'Precio', key: 'price', sortable: false,  align: 'center' as const },
  { title: 'Cantidad', key: 'stockAvaiblable', sortable: false,  align: 'center' as const },
  { title: 'Agregar', key: 'actions', sortable: false, align: 'center' as const },
]);

const isOpen = computed({
  get: () => props.modelValue,
  set: (value: boolean) => emit('update:modelValue', value)
});

const noDataMessage = computed(() => 
  hasSearched.value 
    ? 'No hay productos para mostrar' 
    : 'Realice una búsqueda para ver los productos'
);

const buildFilterParams = () => ({
  textFilter: search.value?.trim() || null,
  numberFilter: FILTER_MAP[selectedFilter.value],
  stateFilter: 1
});

const resetModalState = () => {
  search.value = null;
  selectedFilter.value = '';
  currentPage.value = 1;
  itemsPerPage.value = 5;
  hasSearched.value = false;
  products.value = [];
  totalProducts.value = 0;
  tableKey.value++;
};

const fetchProducts = async () => {
  try {
    loading.value = true;

    await inventoryStore.fetchInventories({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      ...buildFilterParams()
    });

    products.value = inventoryStore.inventories || [];
    totalProducts.value = inventoryStore.totalInventories || 0;
    hasSearched.value = true;

  } catch (error) {
    handleApiError(error, 'Error al buscar productos');
    products.value = [];
    totalProducts.value = 0;
  } finally {
    loading.value = false;
  }
};

const handleSearch = async () => {
  currentPage.value = 1;
  await fetchProducts();
};

const handleItemsPerPageUpdate = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  fetchProducts();
};

const handlePageChange = (page: number) => {
  currentPage.value = page;
  fetchProducts();
};

const handleProductAdd = (product: Inventory) => {
  emit('product-added', product);
};

const handleClose = () => {
  resetModalState();
  emit('update:modelValue', false);
  emit('close');
};
</script>