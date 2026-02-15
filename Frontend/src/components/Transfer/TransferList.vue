<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="transfers" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalTransfers" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as Transfer).code }}</td>
            <td>{{ (item as Transfer).storeOrigin }}</td>
            <td>{{ (item as Transfer).sendDate }}</td>
            <td>{{ (item as Transfer).sendUser }}</td>
            <td>{{ (item as Transfer).storeDestination }}</td>
            <td>{{ (item as Transfer).receiveDate }}</td>
            <td>{{ (item as Transfer).receiveUser }}</td>
            <td class="text-center">
              <v-chip :color="getStatusColor((item as Transfer).statusTransfer)" variant="flat" size="small">
                {{ (item as Transfer).statusTransfer }}
              </v-chip>
            </td>
            <td class="text-center">
              <template v-if="canRead">
                <v-btn icon="preview" variant="text" @click="$emit('view-transfer', item)" size="small" title="Ver">
                </v-btn>
              </template>
              <template v-if="canRead && (item as Transfer).statusTransfer != 'Cancelado'">
                <v-btn icon="print" variant="text" @click="$emit('print-pdf', item)" size="small" title="Imprimir">
                </v-btn>
              </template>
              <template v-if="canDelete && !['Cancelado', 'Recibido'].includes((item as Transfer).statusTransfer)">
                <v-btn icon="cancel" variant="text" @click="$emit('open-modal', { transfer: item, action: 3 })"
                  size="small" title="Cancelar">
                </v-btn>
              </template>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Traspasos</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-pdf-box" @click="handleDownloadPdf" :loading="downloadingPdf"
              title="Descargar PDF">
            </v-btn>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-excel-box" @click="handleDownloadExcel"
              :loading="downloadingExcel" title="Descargar Excel"></v-btn>
            <v-btn v-if="canRead" icon="refresh" @click="handleSearch" title="Actualizar"></v-btn>
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
          <v-btn color="indigo" @click="$emit('fetch-transfer')"> Reset </v-btn>
        </template>
      </v-data-table-server>
    </v-card>
    <CommonFilters v-model="drawerModel" :filters="filterOptions" v-model:selected-filter="selectedFilterModel"
      :status-options="TRANSFER_STATUS_OPTIONS" v-model:state="stateModel" v-model:start-date="startDateModel"
      v-model:end-date="endDateModel" @apply-filters="handleSearch" @clear-filters="handleClearFilters" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { Transfer } from '@/interfaces/transferInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFiltersMovements.vue';
import { TRANSFER_STATUS_OPTIONS } from '@/constants/transferStatus';

interface Props extends Omit<BaseListProps<Transfer>, 'items' | 'totalItems'> {
  transfers: Transfer[];
  totalTransfers: number;
}

const props = withDefaults(defineProps<Props>(), {
  drawer: false,
  selectedFilter: 'Código',
  state: 'Todos',
  startDate: null,
  endDate: null,
  downloadingExcel: false,
  downloadingPdf: false,
  itemsPerPage: 10
});

const emit = defineEmits<{
  'open-form': [];
  'open-modal': [payload: { transfer: Transfer; action: 0 | 1 | 2 | 3 }];
  'view-transfer': [item: Transfer];
  'fetch-transfer': [];
  'search-transfer': [params: {
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
    state: string;
    startDate: Date | null;
    endDate: Date | null;
  }];
  'download-pdf': [params: {
    search: string | null;
    selectedFilter: string;
    state: string;
    startDate: Date | null;
    endDate: Date | null;
  }];
  'print-pdf': [item: Transfer];
  'update:drawer': [value: boolean];
  'update:selectedFilter': [value: string];
  'update:state': [value: string]; 
  'update:startDate': [value: Date | null];
  'update:endDate': [value: Date | null];
  'clear-filters': [];
}>();

const pages = "Traspasos por Página";
const search = ref<string | null>(null);
const filterOptions = ['Código', 'Origen', 'Destino'];

const headers = computed(() => [
  { title: 'Código', key: 'code', sortable: false },
  { title: 'Origen', key: 'storeOrigin', sortable: false },
  { title: 'Fecha de envio', key: 'sendDate', sortable: false },
  { title: 'Enviado por', key: 'sendUser', sortable: false },
  { title: 'Destino', key: 'storeDestination', sortable: false },
  { title: 'Fecha de recepción ', key: 'receiveDate', sortable: false },
  { title: 'Recibido por', key: 'receiveUser', sortable: false },
  { title: 'Estado', key: 'statusTransfer', sortable: false, align: 'center' as const },
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

  if (statusLower === 'enviado') {
    return 'blue';
  } else if (statusLower === 'pendiente') {
    return 'yellow';
  } else if (statusLower === 'recibido') {
    return 'green';
  } else if (statusLower === 'cancelado') {
    return 'red';
  }

  return 'grey';
};

const handleSearch = () => {
  emit('search-transfer', {
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
};

const handleDownloadExcel = () => {
  emit('download-excel', {
    search: search.value,
    selectedFilter: selectedFilterModel.value,
    state: stateModel.value,
    startDate: startDateModel.value,
    endDate: endDateModel.value
  });
};

const handleDownloadPdf = () => {
  emit('download-pdf', {
    search: search.value,
    selectedFilter: selectedFilterModel.value,
    state: stateModel.value,
    startDate: startDateModel.value,
    endDate: endDateModel.value
  });
};
</script>