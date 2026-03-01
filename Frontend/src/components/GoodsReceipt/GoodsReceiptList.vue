<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="goodsreceipt" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalGoodsReceipt" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as GoodsReceipt).code }}</td>
            <td>{{ (item as GoodsReceipt).type }}</td>
            <td>{{ (item as GoodsReceipt).companyName }}</td>
            <td>{{ (item as GoodsReceipt).documentType }}</td>
            <td>{{ (item as GoodsReceipt).documentDate }}</td>
            <td>{{ (item as GoodsReceipt).documentNumber }}</td>
            <td>{{ (item as GoodsReceipt).auditCreateDate }}</td>
            <td class="text-center">
              <v-chip :color="getStatusColor((item as GoodsReceipt).statusReceipt)" variant="flat" size="small">
                {{ (item as GoodsReceipt).statusReceipt }}
              </v-chip>
            </td>
            <td class="text-center">
              <template v-if="canRead">
                <v-btn icon="preview" color="indigo" variant="text" @click="$emit('view-goodsreceipt', item)" size="small" title="Ver">
                </v-btn>
              </template>
              <template v-if="canRead && (item as GoodsReceipt).statusReceipt == 'Completado'">
                <v-btn icon="print" variant="text" @click="$emit('print-pdf', item)" size="small" title="Imprimir">
                </v-btn>
              </template>
              <template v-if="canDelete && (item as GoodsReceipt).statusReceipt != 'Cancelar'">
                <v-btn icon="cancel" color="red" variant="text" @click="$emit('open-modal', { goodsreceipt: item, action: 3 })"
                  size="small" title="Cancelar">
                </v-btn>
              </template>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Entradas</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-pdf-box" color="red" @click="handleDownloadPdf" :loading="downloadingPdf"
              title="Descargar PDF">
            </v-btn>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-excel-box" color="green" @click="handleDownloadExcel"
              :loading="downloadingExcel" title="Descargar Excel"></v-btn>
            <v-btn v-if="canRead" icon="tune" @click="drawerModel = !drawerModel" title="Filtros"></v-btn>
            <v-btn v-if="canCreate" icon="add_box" color="blue-darken-1" @click="$emit('open-form')" title="Registrar"></v-btn>
            <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
              <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo"
                hide-details single-line v-model="search" @click:append-inner="handleSearch()"
                @keyup.enter="handleSearch()">
              </v-text-field>
            </v-col>
          </v-toolbar>
        </template>
        <template v-slot:no-data>
          <v-btn color="indigo" @click="$emit('fetch-goodsreceipt')"> Reset </v-btn>
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
import { GoodsReceipt } from '@/interfaces/goodsReceiptInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFiltersMovements.vue'
import { GOODS_STATUS_OPTIONS } from '@/constants/goodsStatus';

interface Props extends Omit<BaseListProps<GoodsReceipt>, 'items' | 'totalItems'> {
  goodsreceipt: GoodsReceipt[];
  totalGoodsReceipt: number;
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
  'open-modal': [payload: { goodsreceipt: GoodsReceipt; action: 0 | 1 | 2 | 3 }];
  'view-goodsreceipt': [item: GoodsReceipt];
  'fetch-goodsreceipt': [];
  'search-goodsreceipt': [params: {
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
  'print-pdf': [item: GoodsReceipt];
  'update:drawer': [value: boolean];
  'update:selectedFilter': [value: string];
  'update:state': [value: string];
  'update:startDate': [value: Date | null];
  'update:endDate': [value: Date | null];
  'clear-filters': [];
}>();

// Data
const pages = "Entradas por Página";
const search = ref<string | null>(null);
const filterOptions = ['Código', 'Tienda', 'Proveedor'];

// Computed
const headers = computed(() => [
  { title: 'Código', key: 'code', sortable: false },
  { title: 'Tipo', key: 'type', sortable: false },
  { title: 'Proveedor', key: 'companyName', sortable: false },
  { title: 'Tipo de documento', key: 'documentType', sortable: false },
  { title: 'Fecha del documento', key: 'documentDate', sortable: false },
  { title: 'Número de documento', key: 'documenNumber', sortable: false },
  { title: 'Fecha de registro', key: 'auditCreateDate', sortable: false },
  { title: 'Estado', key: 'statusReceipt', sortable: false, align: 'center' as const  },
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


// Methods
const handleSearch = () => {
  emit('search-goodsreceipt', {
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