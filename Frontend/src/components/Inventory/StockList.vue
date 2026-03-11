<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="dynamicHeaders" :items="rows" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalRows" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as InventoryPivotRow).code }}</td>
            <td>{{ (item as InventoryPivotRow).color }}</td>
            <td>{{ (item as InventoryPivotRow).brandName }}</td>
            <td>{{ (item as InventoryPivotRow).categoryName }}</td>
            <td>{{ (item as InventoryPivotRow).auditCreateDate }}</td>
            <td class="text-center" v-for="store in stores" :key="store">
              {{ (item as InventoryPivotRow).stockByStore[store] ?? 0 }}
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title> <v-avatar color="purple-darken-1" size="36" class="mr-3">
                <v-icon icon="mdi-clipboard-text-search-outline" color="white" size="18"></v-icon>
              </v-avatar>Consolidado de existencias</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canDownload" icon variant="text" color="red-darken-1" size="38" @click="handleDownloadPdf"
              :loading="downloadingPdf" title="Descargar PDF" class="mr-2">
              <v-icon icon="mdi-file-pdf-box" size="26"></v-icon>
            </v-btn>
            <v-btn v-if="canDownload" icon variant="text" color="green" size="38" @click="handleDownloadExcel"
              :loading="downloadingExcel" title="Descargar Excel" class="mr-2">
              <v-icon icon="mdi-file-excel-box" size="26"></v-icon>
            </v-btn>
                        <v-btn icon variant="text" size="38" @click="drawerModel = !drawerModel" title="Filtros" class="mr-4">
              <v-icon icon="mdi-tune-variant" size="26"></v-icon>
            </v-btn>
            <v-text-field v-if="canRead" append-inner-icon="mdi-magnify" density="compact" label="Búsqueda"
              variant="solo" hide-details single-line v-model="search" class="mr-4"
              style="width: 100%; max-width: 300px;" @click:append-inner="handleSearch()" @keyup.enter="handleSearch()">
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
      @apply-filters="handleSearch" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { InventoryPivotRow } from '@/interfaces/inventoryInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';

interface Props extends Pick<BaseListProps<InventoryPivotRow>, 'loading' | 'canRead' | 'canDownload' | 'itemsPerPage' | 'drawer' | 'selectedFilter' | 
  'state' | 'startDate' | 'endDate' | 'downloadingExcel'| 'downloadingPdf'> {
  stores: string[];
  rows: InventoryPivotRow[];
  totalRows: number;
}

const props = withDefaults(defineProps<Props>(), {
  stores: () => [],
  rows: () => [],
  loading: false,
  drawer: false,
  selectedFilter: 'Código',
  state: 'Activos',
  startDate: null,
  endDate: null,
  downloadingExcel: false,
  downloadingPdf: false,
  itemsPerPage: 10
});

const emit = defineEmits<{
  'fetch-stock': [];
  'search-stock': [params: {
    search: string | null;
    selectedFilter: string;
    state: string;
    startDate: Date | null;
    endDate: Date | null;
  }];
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
  'update-items-per-page': [itemsPerPage: number];
  'change-page': [page: number];
  'update:drawer': [value: boolean];
  'update:selectedFilter': [value: string];
  'update:state': [value: string];
  'update:startDate': [value: Date | null];
  'update:endDate': [value: Date | null];
}>();

const pages = ref('Artículos por Página');
const search = ref<string | null>(null);
const filterOptions = ref(['Código', 'Descripción', 'Material', 'Color', 'Marca', 'Categoría']);

const dynamicHeaders = computed(() => [
  { title: 'Código', key: 'code', sortable: false },
  { title: 'Color', key: 'color', sortable: false },
  { title: 'Marca', key: 'brandName', sortable: false },
  { title: 'Categoría', key: 'categoryName', sortable: false },
  { title: 'Fecha de creación', key: 'auditCreateDate', sortable: false },
  ...props.stores.map(store => ({ title: store, key: store, sortable: false, align: 'center' as const }))
]);

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

const handleSearch = () => {
  emit('search-stock', {
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
</script>