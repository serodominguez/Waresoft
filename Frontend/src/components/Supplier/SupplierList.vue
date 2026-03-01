<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="suppliers" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalSuppliers" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as Supplier).companyName }}</td>
            <td>{{ (item as Supplier).contact }}</td>
            <td>{{ (item as Supplier).phoneNumber }}</td>
            <td>{{ (item as Supplier).auditCreateDate }}</td>
            <td>{{ (item as Supplier).statusSupplier }}</td>
            <td class="text-center">
              <v-btn v-if="canEdit && (item as Supplier).statusSupplier == 'Activo'" icon="edit" color="indigo" variant="text"
                @click="$emit('edit-supplier', item)" size="small" title="Editar">
              </v-btn>
              <template v-if="canEdit && (item as Supplier).statusSupplier == 'Inactivo'">
                <v-btn icon="check" color="green" variant="text" @click="$emit('open-modal', { supplier: item, action: 1 })"
                  size="small" title="Activar">
                </v-btn>
              </template>
              <template v-if="canEdit && (item as Supplier).statusSupplier == 'Activo'">
                <v-btn icon="block" color="red" variant="text" @click="$emit('open-modal', { supplier: item, action: 2 })"
                  size="small" title="Inactivar">
                </v-btn>
              </template>
              <v-btn v-if="canDelete" icon="delete" color="blue-grey" variant="text"
                @click="$emit('open-modal', { supplier: item, action: 0 })" size="small" title="Eliminar">
              </v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Proveedores</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-pdf-box" color="red" @click="handleDownloadPdf" :loading="downloadingPdf"
              title="Descargar PDF">
            </v-btn>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-excel-box" color="green" @click="handleDownloadExcel"
              :loading="downloadingExcel" title="Descargar Excel"></v-btn>
            <v-btn icon="tune" @click="drawerModel = !drawerModel" title="Filtros"></v-btn>
            <v-btn v-if="canCreate" icon="add_box" color="blue-darken-1" @click="$emit('open-form')" title="Agregar"></v-btn>
            <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
              <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo"
                hide-details single-line v-model="search" @click:append-inner="handleSearch()"
                @keyup.enter="handleSearch()">
              </v-text-field>
            </v-col>
          </v-toolbar>
        </template>
        <template v-slot:no-data>
          <v-btn color="indigo" @click="$emit('fetch-suppliers')"> Reset </v-btn>
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
import { Supplier } from '@/interfaces/supplierInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';

interface Props extends Omit<BaseListProps<Supplier>, 'items' | 'totalItems'> {
  suppliers: Supplier[];
  totalSuppliers: number;
}

const props = withDefaults(defineProps<Props>(), {
  drawer: false,
  selectedFilter: 'Nombre',
  state: 'Activos',
  startDate: null,
  endDate: null,
  downloadingExcel: false,
  downloadingPdf: false,
  itemsPerPage: 10
});

const emit = defineEmits<{
  'open-form': [];
  'open-modal': [payload: { supplier: Supplier; action: 0 | 1 | 2 }]
  'edit-supplier': [supplier: Supplier];
  'fetch-suppliers': [];
  'search-suppliers': [params: {
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

const pages = ref("Proveedores por Página");
const search = ref<string | null>(null);
const filterOptions = ref(['Empresa', 'Contacto']);

const headers = computed(() => [
  { title: 'Empresa', key: 'companyName', sortable: false },
  { title: 'Contacto', key: 'contact', sortable: false },
  { title: 'Teléfono', key: 'phoneNumber', sortable: false },
  { title: 'Fecha de creación', key: 'auditCreateDate', sortable: false },
  { title: 'Estado', key: 'statusSupplier', sortable: false },
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
  emit('search-suppliers', {
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