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
            <td class="text-center" :class="{ 'text-red': ((item as Inventory).stockInTransit ?? 0) > 0 }">
              {{ (item as Inventory).stockInTransit }}
            </td>
            <td class="text-center" :class="{ 'text-red': ((item as Inventory).price ?? 0) <= 0 }">
              {{ (item as Inventory).price }}
            </td>
            <td class="text-center"><v-chip :color="getStatusColor((item as Inventory).replenishment)" variant="flat"
                size="small">
                {{ (item as Inventory).replenishment }}
              </v-chip></td>
            <td class="text-center">
              <v-btn v-if="canEdit" icon="currency_exchange" variant="text" @click="$emit('edit-inventory', item)"
                size="small" title="Editar">
              </v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Inventario</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-document" @click="handleDownloadInventorySheet"
              :loading="downloadingSheet" title="Descargar Planilla">
            </v-btn>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-pdf-box" @click="handleDownloadPdf" :loading="downloadingPdf"
              title="Descargar Pdf"></v-btn>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-excel-box" @click="handleDownloadExcel"
              :loading="downloadingExcel" title="Descargar Excel"></v-btn>
            <v-btn icon="tune" @click="drawerModel = !drawerModel" title="Filtros"></v-btn>
            <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
              <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo"
                hide-details single-line v-model="search" @click:append-inner="handleSearch()"
                @keyup.enter="handleSearch()">
              </v-text-field>
            </v-col>
          </v-toolbar>
        </template>
        <template v-slot:no-data>
          <v-btn color="indigo" @click="$emit('fetch-inventories')"> Reset </v-btn>
        </template>
      </v-data-table-server>
    </v-card>
    <CommonFilters v-model="drawerModel" :filters="filterOptions" v-model:selected-filter="selectedFilterModel"
      v-model:state="stateModel" v-model:start-date="startDateModel" v-model:end-date="endDateModel"
      @apply-filters="handleSearch" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { Inventory } from '@/interfaces/inventoryInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';

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
}>();

// Estado reactivo
const pages = ref("Productos por Página");
const search = ref<string | null>(null);
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
  { title: 'En transito', key: 'stockInTransit', sortable: false, align: 'center' as const },
  { title: 'Precio', key: 'price', sortable: false, align: 'center' as const },
  { title: 'Reposición', key: 'replenishment', sortable: false, align: 'center' as const },
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

const getStatusColor = (status: string): string => {
  const statusLower = status.toLowerCase();

  if (statusLower === 'disponible') {
    return 'green';
  } else if (statusLower === 'no disponible') {
    return 'yellow';
  } else if (statusLower === 'descontinuado') {
    return 'red';
  }

  return 'grey';
};

// Métodos
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
</script>