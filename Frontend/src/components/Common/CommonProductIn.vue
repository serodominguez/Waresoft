<template>
  <v-dialog v-model="isOpen" max-width="1200px" persistent>
    <v-card elevation="2">
      <v-card-title class="bg-surface-light pt-4">
        <span>Seleccionar Producto</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-row justify="center" align="end">
          <v-col cols="4" md="2">
            <v-select color="indigo" variant="solo" density="compact" v-model="selectedFilter" :items="filterOptions"
              label="Buscar por" hide-details />
          </v-col>
          <v-col cols="6" md="6">
            <v-text-field color="indigo" label="Búsqueda" variant="solo" density="compact" hide-details single-line
              v-model="search" @keyup.enter="handleSearch">
              <template v-slot:append-inner>
                <v-tooltip v-bind="tooltipProps" text="Buscar" location="bottom">
                  <template v-slot:activator="{ props }">
                    <v-icon v-bind="props" icon="mdi-magnify" @click="handleSearch" style="cursor: pointer;"></v-icon>
                  </template>
                </v-tooltip>
              </template>
            </v-text-field>
          </v-col>
          <v-col class="d-flex align-center" cols="2" md="2">
            <v-tooltip v-bind="tooltipProps" text="Restablecer" location="bottom">
              <template v-slot:activator="{ props }">
                <v-btn v-bind="props" icon="mdi-backspace" variant="text" color="red" size="small"
                  @click="clearSearch" />
              </template>
            </v-tooltip>
          </v-col>
        </v-row>
        <v-divider class="my-4"></v-divider>
        <v-data-table-server class="elevation-1" :key="tableKey" :headers="headers" :items="products"
          :items-per-page-options="[5, 10]" :items-per-page-text="pages" :items-per-page="itemsPerPage"
          :items-length="totalProducts" :loading="loading" loading-text="Cargando... Espere por favor"
          @update:items-per-page="handleItemsPerPageUpdate" @update:page="handlePageChange">
          <template v-slot:item="{ item }">
            <tr>
              <td>{{ item.code }}</td>
              <td>{{ item.description }}</td>
              <td>{{ item.material }}</td>
              <td>{{ item.color }}</td>
              <td>{{ item.categoryName }}</td>
              <td>{{ item.brandName }}</td>
              <td class="text-center">
                <v-tooltip v-bind="tooltipProps" text="Agegar" location="bottom">
                  <template v-slot:activator="{ props }">
                    <v-btn v-bind="props" icon variant="text" color="indigo" size="small"
                      @click="handleProductAdd(item)">
                      <v-icon icon="mdi-plus-circle" size="24"></v-icon>
                    </v-btn>
                  </template>
                </v-tooltip>
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
      <v-card-actions class="px-4 pb-4">
        <v-spacer></v-spacer>
        <v-btn color="red" dark elevation="4" @click="handleClose">Cerrar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useProductStore } from '@/stores/productStore';
import { Product } from '@/interfaces/productInterface';
import { handleApiError } from '@/helpers/errorHandler';
import { useResponsiveTooltip } from '@/composables/useResponsiveTooltip';

// Props del componente
interface Props {
  modelValue: boolean;
}

const props = defineProps<Props>();

// Emits del componente
const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'close': [];
  'product-added': [product: Product];
}>();

// Inicialización del store
const productStore = useProductStore();

// Estado reactivo - Configuración de tabla
const tableKey = ref(0);
const pages = ref("Productos por Página");
const currentPage = ref(1);
const itemsPerPage = ref(5);

// Estado reactivo - Filtros y búsqueda
const search = ref<string | null>(null);
const selectedFilter = ref('');
const { tooltipProps } = useResponsiveTooltip();
const filterOptions = ref(['Código', 'Descripción', 'Material', 'Color', 'Categoría', 'Marca']);

// Estado reactivo - Datos y estado
const hasSearched = ref(false);
const products = ref<Product[]>([]);
const totalProducts = ref(0);
const loading = ref(false);

// Mapa de filtros
const FILTER_MAP: Record<string, number> = {
  "Código": 1,
  "Descripción": 2,
  "Material": 3,
  "Color": 4,
  "Marca": 5,
  "Categoría": 6
};

// Computed properties
const headers = computed(() => [
  { title: 'Código', key: 'code', sortable: false },
  { title: 'Descripción', key: 'description', sortable: false },
  { title: 'Material', key: 'material', sortable: false },
  { title: 'Color', key: 'color', sortable: false },
  { title: 'Categoría', key: 'categoryName', sortable: false },
  { title: 'Marca', key: 'brandName', sortable: false },
  { title: 'Acciones', key: 'actions', sortable: false, align: 'center' as const },
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

// Construye los parámetros de filtro para la petición
const buildFilterParams = () => ({
  textFilter: search.value?.trim() || null,
  numberFilter: FILTER_MAP[selectedFilter.value],
  stateFilter: 1
});

// Resetea todos los campos del modal a sus valores iniciales
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

const clearSearch = () => {
  search.value = null;
  selectedFilter.value = '';
  hasSearched.value = false;
  products.value = [];
  totalProducts.value = 0;
  tableKey.value++;
};

// Obtiene productos del store
const fetchProducts = async () => {
  try {
    loading.value = true;

    await productStore.fetchProducts({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      ...buildFilterParams()
    });

    products.value = productStore.products || [];
    totalProducts.value = productStore.totalProducts || 0;
    hasSearched.value = true;

  } catch (error) {
    handleApiError(error, 'Error al buscar productos');
    products.value = [];
    totalProducts.value = 0;
  } finally {
    loading.value = false;
  }
};

// Maneja la búsqueda de productos
const handleSearch = async () => {
  currentPage.value = 1;
  await fetchProducts();
};

// Maneja el cambio de items por página
const handleItemsPerPageUpdate = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  fetchProducts();
};

// Maneja el cambio de página
const handlePageChange = (page: number) => {
  currentPage.value = page;
  fetchProducts();
};

// Maneja el agregado de un producto
const handleProductAdd = (product: Product) => {
  emit('product-added', product);
};

// Maneja el cierre del modal
const handleClose = () => {
  resetModalState();
  emit('update:modelValue', false);
  emit('close');
};
</script>