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
              <v-chip :color="stateColor((item as Transfer).statusTransfer)" variant="tonal" size="small">
                <template v-slot:prepend>
                  <v-icon :icon="stateIcon((item as Transfer).statusTransfer)" size="14" class="mr-1"></v-icon>
                </template>
                {{ (item as Transfer).statusTransfer }}
              </v-chip>
            </td>
            <td class="text-center">
              <template v-if="canRead">
                <v-btn icon variant="text" color="indigo" size="small" title="Visualizar"
                  @click="$emit('view-transfer', item)">
                  <v-icon icon="mdi-open-in-new" size="22"></v-icon>
                </v-btn>
              </template>
              <template v-if="canRead && (item as Transfer).statusTransfer != 'Cancelado'">
                <v-btn icon variant="text" size="small" title="Imprimir" @click="$emit('print-pdf', item)">
                  <v-icon icon="mdi-printer-outline" size="22"></v-icon>
                </v-btn>
              </template>
              <template
                v-if="canDelete && !['Cancelado', 'Recibido'].includes((item as Transfer).statusTransfer) && (item as Transfer).idStoreOrigin == currentUser?.storeId">
                <v-btn icon variant="text" color="red" size="small" title="Cancelar"
                  @click="$emit('open-modal', { transfer: item, action: 3 })">
                  <v-icon icon="mdi-cancel" size="22"></v-icon>
                </v-btn>
              </template>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title> <v-avatar color="purple-darken-1" size="36" class="mr-3">
                <v-icon icon="mdi-cart-arrow-up" color="white" size="18"></v-icon>
              </v-avatar>Gestión de Traspasos</v-toolbar-title>
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
      :status-options="TRANSFER_STATUS_OPTIONS" v-model:state="stateModel" v-model:start-date="startDateModel"
      v-model:end-date="endDateModel" @apply-filters="handleSearch" @clear-filters="handleClearFilters" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { storeToRefs } from 'pinia';
import { Transfer } from '@/interfaces/transferInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFiltersMovements.vue';
import { TRANSFER_STATUS_OPTIONS } from '@/constants/transferStatus';

interface Props extends Omit<BaseListProps<Transfer>, 'items' | 'totalItems'> {
  transfers: Transfer[];
  totalTransfers: number;
}
const authStore = useAuthStore();
const { currentUser } = storeToRefs(authStore);

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

const stateColor = (status: string): string => {
  const statusLower = status.toLowerCase();

  if (statusLower === 'enviado') {
    return 'blue-darken-2';
  } else if (statusLower === 'pendiente') {
    return 'orange';
  } else if (statusLower === 'recibido') {
    return 'green';
  } else if (statusLower === 'cancelado') {
    return 'red';
  }

  return 'grey';
};

const stateIcon = (status: string): string => {
  const statusLower = status.toLowerCase();
  if (statusLower === 'enviado')   return 'mdi-truck-delivery';
  if (statusLower === 'pendiente') return 'mdi-clock-outline';
  if (statusLower === 'recibido')  return 'mdi-package-variant-closed-check';
  if (statusLower === 'cancelado') return 'mdi-archive-cancel';
  return 'mdi-circle-outline';
}

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