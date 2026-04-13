<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="inventories" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalInventories" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as Inventory).code }}</td>
            <td>{{ (item as Inventory).description }}</td>
            <td>{{ (item as Inventory).material }}</td>
            <td>{{ (item as Inventory).color }}</td>
            <td>{{ (item as Inventory).categoryName }}</td>
            <td>{{ (item as Inventory).brandName }}</td>
            <td>{{ (item as Inventory).unitMeasure }}</td>
            <td class="text-center" :class="{ 'text-red': ((item as Inventory).stockAvailable ?? 0) <= 0 }">
              {{ (item as Inventory).stockAvailable }}
            </td>
            <td class="text-center"
              :class="{ 'text-red': ((item as Inventory).calculatedStock ?? 0) !== ((item as Inventory).stockAvailable ?? 0) }">
              {{ (item as Inventory).calculatedStock }}
            </td>
            <td class="text-center" :class="{ 'text-red': ((item as Inventory).stockDifference ?? 0) != 0 }">
              {{ (item as Inventory).stockDifference }}
            </td>
            <td class="text-center" :class="{ 'text-red': ((item as Inventory).stockInTransit ?? 0) > 0 }">
              {{ (item as Inventory).stockInTransit }}
            </td>
            <td class="text-center" :class="{ 'text-red': ((item as Inventory).price ?? 0) <= 0 }">
              {{ (item as Inventory).price }}
            </td>
            <td>{{ (item as Inventory).auditCreateDate }}</td>
            <td class="text-center">
              <v-chip :color="getStatusColor((item as Inventory).replenishment)" variant="tonal" size="small">
                <template v-slot:prepend>
                  <v-icon :icon="getStatusIcon((item as Inventory).replenishment)" size="14" class="mr-1"></v-icon>
                </template>
                {{ (item as Inventory).replenishment }}
              </v-chip>
            </td>
            <td class="text-center">
              <v-tooltip v-bind="tooltipProps" text="Editar Precio" location="bottom">
                <template v-slot:activator="{ props }">
                  <v-btn v-bind="props" v-if="canEdit" icon variant="text" color="blue-darken-1" size="small"
                    @click="$emit('edit-inventory', item)">
                    <v-icon icon="mdi-cash-edit" size="24"></v-icon>
                  </v-btn>
                </template>
              </v-tooltip>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title> <v-avatar color="indigo" size="36" class="mr-3">
                <v-icon icon="mdi-warehouse" color="white" size="18"></v-icon>
              </v-avatar>Gestión de Inventario</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-tooltip v-bind="tooltipProps" text="Planilla Inventario" location="bottom">
              <template v-slot:activator="{ props }">
                <v-btn v-bind="props" v-if="canDownload" icon variant="text" color="indigo" size="38"
                  @click="handleDownloadInventorySheet" :loading="downloadingSheet" :disabled="downloadingSheet"
                  class="mr-2">
                  <v-icon icon="mdi-file-document" size="26"></v-icon>
                </v-btn>
              </template>
            </v-tooltip>
            <v-tooltip v-bind="tooltipProps" text="Descargar PDF" location="bottom">
              <template v-slot:activator="{ props }">
                <v-btn v-bind="props" v-if="canDownload" icon variant="text" color="red" size="38"
                  @click="handleDownloadPdf" :loading="downloadingPdf" :disabled="downloadingPdf" class="mr-2">
                  <v-icon icon="mdi-file-pdf-box" size="26"></v-icon>
                </v-btn>
              </template>
            </v-tooltip>
            <v-tooltip v-bind="tooltipProps" text="Descargar Excel" location="bottom">
              <template v-slot:activator="{ props }">
                <v-btn v-bind="props" v-if="canDownload" icon variant="text" color="green" size="38"
                  @click="handleDownloadExcel" :loading="downloadingExcel" :disabled="downloadingExcel" class="mr-2">
                  <v-icon icon="mdi-file-excel-box" size="26"></v-icon>
                </v-btn>
              </template>
            </v-tooltip>
            <v-tooltip v-bind="tooltipProps" text="Filtros" location="bottom">
              <template v-slot:activator="{ props }">
                <v-btn v-bind="props" icon variant="text" size="38" @click="drawerModel = !drawerModel" class="mr-4">
                  <v-icon icon="mdi-tune-variant" size="26"></v-icon>
                </v-btn>
              </template>
            </v-tooltip>
            <v-text-field v-if="canRead" density="compact" label="Búsqueda" variant="solo" hide-details single-line
              v-model="search" class="mr-4" style="width: 100%; max-width: 300px;" @keyup.enter="handleSearch()">
              <template v-slot:append-inner>
                <v-tooltip v-bind="tooltipProps" text="Buscar" location="bottom">
                  <template v-slot:activator="{ props }">
                    <v-icon v-bind="props" icon="mdi-magnify" @click="handleSearch()" style="cursor: pointer;"></v-icon>
                  </template>
                </v-tooltip>
              </template>
            </v-text-field>
          </v-toolbar>
        </template>
        <template v-slot:no-data>
          <span class="text-grey">No se encontraron resultados</span>
        </template>
      </v-data-table-server>
    </v-card>
    <CommonFilters v-model="drawerModel" :filters="filterOptions" v-model:selected-filter="selectedFilterModel"
      v-model:state="stateModel" v-model:start-date="startDateModel" v-model:end-date="endDateModel"
      @apply-filters="handleSearch" @clear-filters="handleClearFilters" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { Inventory } from '@/interfaces/inventoryInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';
import { useResponsiveTooltip } from '@/composables/useResponsiveTooltip';
// Props
interface Props extends Omit<BaseListProps<Inventory>, 'items' | 'totalItems'> {
  inventories: Inventory[];
  totalInventories: number;
  downloadingSheet: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  canCreate: false,
  canDelete: false,
  drawer: false,
  selectedFilter: 'Código',
  state: 'Activos',
  startDate: null,
  endDate: null,
  downloadingExcel: false,
  downloadingPdf: false,
  downloadingSheet: false,
  itemsPerPage: 10
});

// Emits
const emit = defineEmits<{
  'open-form': [];
  'open-modal': [payload: { inventory: Inventory; action: 0 | 1 | 2 }];
  'edit-inventory': [inventory: Inventory];
  'fetch-inventories': [];
  'search-inventories': [params: {
    search: string | null;
    selectedFilter: string;
    state: string;
    startDate: Date | null;
    endDate: Date | null;
  }];
  'update-items-per-page': [itemsPerPage: number];
  'change-page': [page: number];
  'download-excel': [params: {
    search: string | null;
    selectedFilter: string;
    stateFilter: string;
    startDate: Date | null;
    endDate: Date | null;
  }];
  'download-pdf': [params: {
    search: string | null;
    selectedFilter: string;
    stateFilter: string;
    startDate: Date | null;
    endDate: Date | null;
  }];
  'download-inventory-sheet': [params: {
    search: string | null;
    selectedFilter: string;
    stateFilter: string;
    startDate: Date | null;
    endDate: Date | null;
  }];
  'update:drawer': [value: boolean];
  'update:selectedFilter': [value: string];
  'update:state': [value: string];
  'update:startDate': [value: Date | null];
  'update:endDate': [value: Date | null];
  'clear-filters': [];
}>();

// Estado reactivo
const pages = ref("Productos por Página");
const search = ref<string | null>(null);
const { tooltipProps } = useResponsiveTooltip();
const filterOptions = ref(['Código', 'Descripción', 'Material', 'Color', 'Precio', 'Categoría', 'Marca']);

// Computed properties
const headers = computed(() => [
  { title: 'Código', key: 'code', sortable: false },
  { title: 'Descripción', key: 'description', sortable: false },
  { title: 'Material', key: 'material', sortable: false },
  { title: 'Color', key: 'color', sortable: false },
  { title: 'Categoría', key: 'categoryName', sortable: false },
  { title: 'Marca', key: 'brandName', sortable: false },
  { title: 'Unidad', key: 'unitMeasure', sortable: false },
  { title: 'Existencias', key: 'stockAvailable', sortable: false, align: 'center' as const },
  { title: 'Calculado', key: 'calculatedStock', sortable: false, align: 'center' as const },
  { title: 'Diferencias', key: 'stockDifference', sortable: false, align: 'center' as const },
  { title: 'En transito', key: 'stockInTransit', sortable: false, align: 'center' as const },
  { title: 'Precio', key: 'price', sortable: false, align: 'center' as const },
  { title: 'Fecha de creación', key: 'replenishment', sortable: false },
  { title: 'Reposición', key: 'auditCreateDate', sortable: false, align: 'center' as const },
  { title: 'Acciones', key: 'actions', sortable: false, align: 'center' as const },
]);

// Computed bidireccionales para v-model
const drawerModel = computed({
  get: () => props.drawer,
  set: (value: boolean) => emit('update:drawer', value)
});

const selectedFilterModel = computed({
  get: () => props.selectedFilter,
  set: (value: string) => emit('update:selectedFilter', value)
});

const stateModel = computed({
  get: () => props.state,
  set: (value: string) => emit('update:state', value)
});

const startDateModel = computed({
  get: () => props.startDate,
  set: (value: Date | null) => emit('update:startDate', value)
});

const endDateModel = computed({
  get: () => props.endDate,
  set: (value: Date | null) => emit('update:endDate', value)
});

// Métodos
const getStatusColor = (status: string): string => {
  const statusLower = status.toLowerCase();

  if (statusLower === 'disponible') {
    return 'green';
  } else if (statusLower === 'no disponible') {
    return 'orange';
  } else if (statusLower === 'descontinuado') {
    return 'red';
  }

  return 'grey';
};

const getStatusIcon = (status: string): string => {
  const statusLower = status.toLowerCase();
  if (statusLower === 'disponible') return 'mdi-check-circle';
  if (statusLower === 'no disponible') return 'mdi-minus-circle';
  if (statusLower === 'descontinuado') return 'mdi-close-circle';
  return 'mdi-circle-outline';
};

const handleSearch = () => {
  emit('search-inventories', {
    search: search.value,
    selectedFilter: selectedFilterModel.value,
    state: stateModel.value,
    startDate: startDateModel.value,
    endDate: endDateModel.value
  });
};

const handleDownloadExcel = () => {
  emit('download-excel', {
    search: search.value,
    selectedFilter: selectedFilterModel.value,
    stateFilter: stateModel.value,
    startDate: startDateModel.value,
    endDate: endDateModel.value
  });
};

const handleDownloadPdf = () => {
  emit('download-pdf', {
    search: search.value,
    selectedFilter: selectedFilterModel.value,
    stateFilter: stateModel.value,
    startDate: startDateModel.value,
    endDate: endDateModel.value
  });
};

const handleDownloadInventorySheet = () => {
  emit('download-inventory-sheet', {
    search: search.value,
    selectedFilter: selectedFilterModel.value,
    stateFilter: stateModel.value,
    startDate: startDateModel.value,
    endDate: endDateModel.value
  });
};

const handleClearFilters = () => {
  search.value = null;
  emit('clear-filters');
};
</script>