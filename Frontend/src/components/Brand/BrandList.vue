<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="brands" :search="search || undefined" :items-per-page-text="pages"
        :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalBrands"
        :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as Brand).brandName }}</td>
            <td>{{ (item as Brand).auditCreateDate }}</td>
            <td>{{ (item as Brand).statusBrand }}</td>
            <td class="text-center">
              <v-btn v-if="canEdit && (item as Brand).statusBrand == 'Activo'" icon="edit" variant="text"
                @click="$emit('edit-brand', item)" size="small" title="Editar">
              </v-btn>
              <template v-if="canEdit && (item as Brand).statusBrand == 'Inactivo'">
                <v-btn icon="check" variant="text"
                  @click="$emit('open-modal', { brand: item, action: 1 })" size="small" title="Activar">
                </v-btn>
              </template>
              <template v-if="canEdit && (item as Brand).statusBrand == 'Activo'">
                <v-btn icon="block" variant="text" @click="$emit('open-modal', { brand: item, action: 2 })"
                  size="small" title="Desactivar">
                </v-btn>
              </template>
              <v-btn v-if="canDelete" icon="delete" variant="text"
                @click="$emit('open-modal', { brand: item, action: 0 })" size="small" title="Eliminar">
              </v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Marcas</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-pdf-box" @click="handleDownloadPdf"
              :loading="downloadingPdf" title="Descargar PDF">
            </v-btn>
            <v-btn v-if="canDownload" icon="mdi:mdi-microsoft-excel" @click="handleDownloadExcel"
              :loading="downloadingExcel" title="Descargar Excel"></v-btn>
            <v-btn icon="tune" @click="drawerModel = !drawerModel" title="Filtros"></v-btn>
            <v-btn v-if="canCreate" icon="add_box" @click="$emit('open-form')" title="Registrar"></v-btn>
            <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
              <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo"
                hide-details single-line v-model="search" @click:append-inner="handleSearch()"
                @keyup.enter="handleSearch()">
              </v-text-field>
            </v-col>
          </v-toolbar>
        </template>
        <template v-slot:no-data>
          <v-btn color="indigo" @click="$emit('fetch-brands')"> Reset </v-btn>
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
import { Brand } from '@/interfaces/brandInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';

/**
 * Props recibidas del componente padre
 * Incluye datos, permisos y configuración de filtros
 */
interface Props extends Omit<BaseListProps<Brand>, 'items' | 'totalItems'> {
  brands: Brand[];      // En lugar de 'items'
  totalBrands: number;  // En lugar de 'totalItems'
}

const props = withDefaults(defineProps<Props>(), {
  drawer: false,
  selectedFilter: 'Marca',
  state: 'Activos',
  startDate: null,
  endDate: null,
  downloadingExcel: false,
  downloadingPdf: false,
  itemsPerPage: 10
});

/**
 * Eventos que este componente puede emitir al padre
 * Sigue el patrón de comunicación padre-hijo de Vue
 */
const emit = defineEmits<{
  'open-form': [];
  'open-modal': [payload: { brand: Brand; action: 0 | 1 | 2 }]
  'edit-brand': [brand: Brand];
  'fetch-brands': [];
  'search-brands': [params: {
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
  'update:drawer': [value: boolean];
  'update:selectedFilter': [value: string];
  'update:state': [value: string];
  'update:startDate': [value: Date | null];
  'update:endDate': [value: Date | null];
}>();

// Estado reactivo
const pages = ref("Marcas por Página");
const search = ref<string | null>(null);
const filterOptions = ref(['Marca']);

// Define las columnas/encabezados de la tabla
const headers = computed(() => [
  { title: 'Marca', key: 'brandName', sortable: false },
  { title: 'Fecha de registro', key: 'auditCreateDate', sortable: false },
  { title: 'Estado', key: 'statusBrand', sortable: false },
  { title: 'Acciones', key: 'actions', sortable: false, align: 'center' as const },
]);

// Computed properties bidireccionales para v-model

// Drawer de filtros
const drawerModel = computed({
  get: () => props.drawer,
  set: (value: boolean) => emit('update:drawer', value)
});

// Filtro seleccionado
const selectedFilterModel = computed({
  get: () => props.selectedFilter,
  set: (value: string) => emit('update:selectedFilter', value)
});

// Estado del filtro
const stateModel = computed({
  get: () => props.state,
  set: (value: string) => emit('update:state', value)
});

// Fecha de inicio
const startDateModel = computed({
  get: () => props.startDate,
  set: (value: Date | null) => emit('update:startDate', value)
});

// Fecha de fin
const endDateModel = computed({
  get: () => props.endDate,
  set: (value: Date | null) => emit('update:endDate', value)
});

// Maneja la búsqueda de marcas con todos los filtros activos
const handleSearch = () => {
  emit('search-brands', {
    search: search.value,
    selectedFilter: selectedFilterModel.value,
    state: stateModel.value,
    startDate: startDateModel.value,
    endDate: endDateModel.value
  });
};

// Maneja la descarga del reporte Excel
const handleDownloadExcel = () => {
  emit('download-excel', {
    search: search.value,
    selectedFilter: selectedFilterModel.value,
    stateFilter: stateModel.value,
    startDate: startDateModel.value,
    endDate: endDateModel.value
  });
};

// Maneja la descarga del reporte Pdf
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