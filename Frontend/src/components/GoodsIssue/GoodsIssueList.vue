<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="goodsissue" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalGoodsIssue" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as GoodsIssue).code }}</td>
            <td>{{ (item as GoodsIssue).type }}</td>
            <td>{{ (item as GoodsIssue).userName }}</td>
            <td>{{ (item as GoodsIssue).auditCreateDate }}</td>
            <td class="text-center">
              <v-chip :color="getStatusColor((item as GoodsIssue).statusIssue)" variant="flat" size="small">
                {{ (item as GoodsIssue).statusIssue }}
              </v-chip>
            </td>
            <td class="text-center">
              <template v-if="canRead">
                <v-btn icon="preview" variant="text" @click="$emit('view-goodsissue', item)" size="small" title="Ver">
                </v-btn>
              </template>
              <template v-if="canRead && (item as GoodsIssue).statusIssue == 'Completado'">
                <v-btn icon="print" variant="text" @click="$emit('print-pdf', item)" size="small" title="Imprimir">
                </v-btn>
              </template>
              <template v-if="canDelete && (item as GoodsIssue).statusIssue != 'Cancelado'">
                <v-btn icon="cancel" variant="text" @click="$emit('open-modal', { goodsissue: item, action: 3 })"
                  size="small" title="Cancelar">
                </v-btn>
              </template>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Salidas</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-pdf-box" @click="handleDownloadPdf" :loading="downloadingPdf"
              title="Descargar PDF">
            </v-btn>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-excel-box" @click="handleDownloadExcel"
              :loading="downloadingExcel" title="Descargar Excel"></v-btn>
            <v-btn v-if="canRead" icon="tune" @click="drawerModel = !drawerModel" title="Filtros"></v-btn>
            <v-btn v-if="canCreate" icon="add_box" @click="$emit('open-form')" title="Agregar"></v-btn>
            <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
              <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo"
                hide-details single-line v-model="search" @click:append-inner="handleSearch()"
                @keyup.enter="handleSearch()">
              </v-text-field>
            </v-col>
          </v-toolbar>
        </template>
        <template v-slot:no-data>
          <v-btn color="indigo" @click="$emit('fetch-goodsissue')"> Reset </v-btn>
        </template>
      </v-data-table-server>
    </v-card>
    <CommonFilters v-model="drawerModel" :filters="filterOptions" v-model:selected-filter="selectedFilterModel"
      :status-options="GOODS_STATUS_OPTIONS" v-model:state="stateModel" v-model:start-date="startDateModel"
      v-model:end-date="endDateModel" @apply-filters="applyFilters" @clear-filters="clearFilters" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { GoodsIssue } from '@/interfaces/goodsIssueInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFiltersMovements.vue';
import { GOODS_STATUS_OPTIONS } from '@/constants/goodsStatus';

interface Props extends Omit<BaseListProps<GoodsIssue>, 'items' | 'totalItems'> {
  goodsissue: GoodsIssue[];
  totalGoodsIssue: number;
}

const props = withDefaults(defineProps<Props>(), {
  drawer: false,
  selectedFilter: 'Código',
  state: 'Completado',
  startDate: null,
  endDate: null,
  downloadingExcel: false,
  downloadingPdf: false,
  itemsPerPage: 10
});

const emit = defineEmits<{
  'open-form': [];
  'open-modal': [payload: { goodsissue: GoodsIssue; action: 0 | 1 | 2 | 3 }];
  'view-goodsissue': [item: GoodsIssue];
  'fetch-goodsissue': [];
  'search-goodsissue': [params: {
    search: string | null;
    selectedFilter: string;
    state: string;
    startDate: Date | null;
    endDate: Date | null;
  }];
  'update-items-per-page': [value: number];
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
  'print-pdf': [item: GoodsIssue];
  'update:drawer': [value: boolean];
  'update:selectedFilter': [value: string];
  'update:state': [value: string];
  'update:startDate': [value: Date | null];
  'update:endDate': [value: Date | null];
}>();

const pages = "Salidas por Página";
const search = ref<string | null>(null);
const filterOptions = ['Código', 'Tienda', 'Personal'];

const headers = computed(() => [
  { title: 'Código', key: 'code', sortable: false },
  { title: 'Tipo', key: 'type', sortable: false },
  { title: 'Personal', key: 'userName', sortable: false },
  { title: 'Fecha de registro', key: 'auditCreateDate', sortable: false },
  { title: 'Estado', key: 'statusIssue', sortable: false, align: 'center' as const },
  { title: 'Acciones', key: 'actions', sortable: false, align: 'center' as const },
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

const getStatusColor = (status: string): string => {
  const statusLower = status.toLowerCase();

  if (statusLower === 'completado') {
    return 'green';
  } else if (statusLower === 'cancelado') {
    return 'red';
  } 

  return 'grey';
};

const applyFilters = () => {
  drawerModel.value = false;
  emit('search-goodsissue', {
    search: null,
    selectedFilter: selectedFilterModel.value,
    state: stateModel.value,
    startDate: startDateModel.value,
    endDate: endDateModel.value
  });
};

const clearFilters = () => {
  selectedFilterModel.value = 'Código';
  stateModel.value = 'Completado';
  startDateModel.value = null;
  endDateModel.value = null;
};

const handleSearch = () => {
  emit('search-goodsissue', {
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