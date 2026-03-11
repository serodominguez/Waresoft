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
              <v-chip :color="stateColor((item as GoodsIssue).statusIssue)" variant="tonal" size="small">
                <template v-slot:prepend>
                  <v-icon :icon="stateIcon((item as GoodsIssue).statusIssue)" size="14" class="mr-1"></v-icon>
                </template>
                {{ (item as GoodsIssue).statusIssue }}
              </v-chip>
            </td>
            <td class="text-center">
              <template v-if="canRead">
                <v-btn icon variant="text" color="indigo" size="small" title="Visualizar"
                  @click="$emit('view-goodsissue', item)">
                  <v-icon icon="mdi-open-in-new" size="22"></v-icon>
                </v-btn>
              </template>
              <template v-if="canRead && (item as GoodsIssue).statusIssue == 'Completado'">
                <v-btn icon variant="text" size="small" title="Imprimir" @click="$emit('print-pdf', item)">
                  <v-icon icon="mdi-printer-outline" size="22"></v-icon>
                </v-btn>
              </template>
              <template v-if="canDelete && (item as GoodsIssue).statusIssue != 'Cancelado'">
                <v-btn icon variant="text" color="red" size="small" title="Cancelar"
                  @click="$emit('open-modal', { goodsissue: item, action: 3 })">
                  <v-icon icon="mdi-cancel" size="22"></v-icon>
                </v-btn>
              </template>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title> <v-avatar color="purple-darken-1" size="36" class="mr-3">
                <v-icon icon="mdi-cart-minus" color="white" size="18"></v-icon>
              </v-avatar>Gestión de Salidas</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canDownload" icon variant="text" color="red-darken-1" size="38" @click="handleDownloadPdf"
              :loading="downloadingPdf" title="Descargar PDF" class="mr-2">
              <v-icon icon="mdi-file-pdf-box" size="26"></v-icon>
            </v-btn>
            <v-btn v-if="canDownload" icon variant="text" color="green" size="38" @click="handleDownloadExcel"
              :loading="downloadingExcel" title="Descargar Excel" class="mr-2">
              <v-icon icon="mdi-file-excel-box" size="26"></v-icon>
            </v-btn>
            <v-btn v-if="canCreate" icon variant="text" color="purple-darken-1" size="38" @click="$emit('open-form')"
              title="Registrar" class="mr-2">
              <v-icon icon="mdi-plus-box" size="26"></v-icon>
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
      :status-options="GOODS_STATUS_OPTIONS" v-model:state="stateModel" v-model:start-date="startDateModel"
      v-model:end-date="endDateModel" @apply-filters="handleSearch" @clear-filters="handleClearFilters" />
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
  'clear-filters': [];
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

const stateColor = (status: string): string => {
  const statusLower = status.toLowerCase();

  if (statusLower === 'completado') {
    return 'green';
  } else if (statusLower === 'cancelado') {
    return 'red';
  } 

  return 'grey';
};

const stateIcon = (status: string): string => {
  const statusLower = status.toLowerCase();
  if (statusLower === 'completado') return 'mdi-check-circle';
  if (statusLower === 'cancelado') return 'mdi-close-circle';
  return 'mdi-circle-outline';
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

const handleClearFilters = () => {
  search.value = null;
  emit('clear-filters');
}

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