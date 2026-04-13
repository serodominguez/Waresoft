<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="customers" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalCustomers" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as Customer).names }}</td>
            <td>{{ (item as Customer).lastNames }}</td>
            <td>{{ (item as Customer).phoneNumber }}</td>
            <td>{{ (item as Customer).auditCreateDate }}</td>
            <td>
              <v-chip :color="(item as Customer).statusCustomer === 'Activo' ? 'green' : 'red'"
                :variant="(item as Customer).statusCustomer === 'Activo' ? 'tonal' : 'tonal'" size="small" rounded="lg">
                <v-icon
                  :icon="(item as Customer).statusCustomer === 'Activo' ? 'mdi:mdi-check-circle' : 'mdi:mdi-close-circle'"
                  start size="12"></v-icon>
                {{ (item as Customer).statusCustomer }}
              </v-chip>
            </td>
            <td class="text-center">
              <v-tooltip v-bind="tooltipProps" text="Editar" location="bottom">
                <template v-slot:activator="{ props }">
                  <v-btn v-bind="props" v-if="canEdit && (item as Customer).statusCustomer == 'Activo'" icon
                    variant="text" color="blue-darken-1" size="small" @click="$emit('edit-customer', item)">
                    <v-icon icon="mdi-pencil" size="24"></v-icon>
                  </v-btn>
                </template>
              </v-tooltip>
              <v-tooltip v-bind="tooltipProps" text="Activar" location="bottom">
                <template v-slot:activator="{ props }">
                  <v-btn v-bind="props" v-if="canEdit && (item as Customer).statusCustomer == 'Inactivo'" icon
                    variant="text" color="green-darken-1" size="small"
                    @click="$emit('open-modal', { customer: item, action: 1 })">
                    <v-icon icon="mdi-check-circle" size="24"></v-icon>
                  </v-btn>
                </template>
              </v-tooltip>
              <v-tooltip v-bind="tooltipProps" text="Inactivar" location="bottom">
                <template v-slot:activator="{ props }">
                  <v-btn v-bind="props" v-if="canEdit && (item as Customer).statusCustomer == 'Activo'" icon
                    variant="text" color="red-darken-1" size="small" @click="$emit('open-modal', { customer: item, action: 2 })">
                    <v-icon icon="mdi-close-circle" size="24"></v-icon>
                  </v-btn>
                </template>
              </v-tooltip>
              <v-tooltip v-bind="tooltipProps" text="Eliminar" location="bottom">
                <template v-slot:activator="{ props }">
                  <v-btn v-bind="props" v-if="canDelete" icon variant="text" color="blue-grey-darken-1" size="small"
                    @click="$emit('open-modal', { customer: item, action: 0 })">
                    <v-icon icon="mdi-trash-can" size="24"></v-icon>
                  </v-btn>
                </template>
              </v-tooltip>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title><v-avatar color="indigo" size="36" class="mr-3">
                <v-icon icon="mdi-account-box" color="white" size="18"></v-icon>
              </v-avatar>Gestión de Clientes</v-toolbar-title>
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
            <v-tooltip v-bind="tooltipProps" text="Agregar Cliente" location="bottom">
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
      v-model:state="stateModel" v-model:start-date="startDateModel" v-model:end-date="endDateModel"
      @apply-filters="handleSearch" @clear-filters="handleClearFilters" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { Customer } from '@/interfaces/customerInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';
import { useResponsiveTooltip } from '@/composables/useResponsiveTooltip';

interface Props extends Omit<BaseListProps<Customer>, 'items' | 'totalItems'> {
  customers: Customer[];
  totalCustomers: number;
}

const props = withDefaults(defineProps<Props>(), {
  drawer: false,
  selectedFilter: 'Nombres',
  state: 'Activos',
  startDate: null,
  endDate: null,
  downloadingExcel: false,
  downloadingPdf: false,
  itemsPerPage: 10
});

const emit = defineEmits<{
  'open-form': [];
  'open-modal': [payload: { customer: Customer; action: 0 | 1 | 2 }];
  'edit-customer': [customer: Customer];
  'fetch-customers': [];
  'search-customers': [params: {
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
  'clear-filters': [];
}>();

const pages = ref("Clientes por Página");
const search = ref<string | null>(null);
const { tooltipProps } = useResponsiveTooltip();
const filterOptions = ref(['Nombres', 'Apellidos', 'Carnet']);

const headers = computed(() => [
  { title: 'Nombres', key: 'names', sortable: false },
  { title: 'Apellidos', key: 'lastNames', sortable: false },
  { title: 'Teléfono', key: 'phoneNumber', sortable: false },
  { title: 'Fecha de creación', key: 'auditCreateDate', sortable: false },
  { title: 'Estado', key: 'statusCustomer', sortable: false },
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

const handleSearch = () => {
  emit('search-customers', {
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

const handleClearFilters = () => {
  search.value = null;
  emit('clear-filters');
};
</script>