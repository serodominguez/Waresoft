<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="goodsreceipt" :page="currentPage" :search="search || undefined"
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
              <v-chip :color="stateColor((item as GoodsReceipt).statusReceipt)" variant="tonal" size="small">
                <template v-slot:prepend>
                  <v-icon :icon="stateIcon((item as GoodsReceipt).statusReceipt)" size="14" class="mr-1"></v-icon>
                </template>
                {{ (item as GoodsReceipt).statusReceipt }}
              </v-chip>
            </td>
            <td class="text-center">
              <template v-if="canRead">
                <v-tooltip v-bind="tooltipProps" text="Visualizar" location="bottom">
                  <template v-slot:activator="{ props }">
                    <v-btn v-bind="props" icon variant="text" color="deep-purple-darken-1" size="small"
                      @click="$emit('view-goodsreceipt', item)">
                      <v-icon icon="mdi-file-eye" size="24"></v-icon>
                    </v-btn>
                  </template>
                </v-tooltip>
              </template>
              <template v-if="canRead && (item as GoodsReceipt).statusReceipt == 'Completado'">
                <v-tooltip v-bind="tooltipProps" text="Imprimir" location="bottom">
                  <template v-slot:activator="{ props }">
                    <v-btn v-bind="props" icon variant="text" size="small" @click="$emit('print-pdf', item)"
                      :loading="printingPdfId === (item as GoodsReceipt).idReceipt"
                      :disabled="printingPdfId === (item as GoodsReceipt).idReceipt">
                      <v-icon icon="mdi-printer" size="24"></v-icon>
                    </v-btn>
                  </template>
                </v-tooltip>
              </template>
              <template v-if="canDelete && (item as GoodsReceipt).statusReceipt != 'Cancelado'">
                <v-tooltip v-bind="tooltipProps" text="Cancelar" location="bottom">
                  <template v-slot:activator="{ props }">
                    <v-btn v-bind="props" icon variant="text" color="red-darken-1" size="small"
                      @click="$emit('open-modal', { goodsreceipt: item, action: 3 })">
                      <v-icon icon="mdi-file-cancel" size="22"></v-icon>
                    </v-btn>
                  </template>
                </v-tooltip>
              </template>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title> <v-avatar color="indigo" size="36" class="mr-3">
                <v-icon icon="mdi-cart-plus" color="white" size="18"></v-icon>
              </v-avatar>Gestión de Entradas</v-toolbar-title>
            <v-spacer></v-spacer>
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
            <v-tooltip v-bind="tooltipProps" text="Registrar Salida" location="bottom">
              <template v-slot:activator="{ props }">
                <v-btn v-bind="props" v-if="canCreate" icon variant="text" color="indigo" size="38"
                  @click="$emit('open-form')" class="mr-2">
                  <v-icon icon="mdi-plus-box" size="26"></v-icon>
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
      :status-options="GoodsStatusOptions" v-model:state="stateModel" v-model:start-date="startDateModel"
      v-model:end-date="endDateModel" @apply-filters="handleSearch" @clear-filters="handleClearFilters" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { GoodsReceipt } from '@/interfaces/goodsReceiptInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFiltersMovements.vue'
import { GoodsStatusOptions } from '@/constants/goodsStatus';
import { useResponsiveTooltip } from '@/composables/useResponsiveTooltip';

interface Props extends Omit<BaseListProps<GoodsReceipt>, 'items' | 'totalItems'> {
  goodsreceipt: GoodsReceipt[];
  totalGoodsReceipt: number;
  currentPage: number;
  printingPdfId?: number | string | null;
}

const props = withDefaults(defineProps<Props>(), {
  drawer: false,
  selectedFilter: 'Código',
  state: 'Completado',
  startDate: null,
  endDate: null,
  downloadingExcel: false,
  downloadingPdf: false,
  itemsPerPage: 10,
  currentPage: 1,
  printingPdfId: null
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
const { tooltipProps } = useResponsiveTooltip();
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
  { title: 'Estado', key: 'statusReceipt', sortable: false, align: 'center' as const },
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

// Methods
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