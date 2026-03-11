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
            <td>
              <v-chip :color="(item as Supplier).statusSupplier === 'Activo' ? 'green' : 'red'"
                :variant="(item as Supplier).statusSupplier === 'Activo' ? 'tonal' : 'tonal'" size="small" rounded="lg">
                <v-icon
                  :icon="(item as Supplier).statusSupplier === 'Activo' ? 'mdi:mdi-check-circle' : 'mdi:mdi-close-circle'"
                  start size="12"></v-icon>
                {{ (item as Supplier).statusSupplier }}
              </v-chip>
            </td>
            <td class="text-center">
              <v-btn v-if="canEdit && (item as Supplier).statusSupplier == 'Activo'" icon variant="text" color="indigo"
                size="small" title="Editar" @click="$emit('edit-supplier', item)">
                <v-icon icon="mdi-pencil-outline" size="22"></v-icon>
              </v-btn>
              <v-btn v-if="canEdit && (item as Supplier).statusSupplier == 'Inactivo'" icon variant="text" color="green"
                size="small" title="Activar" @click="$emit('open-modal', { supplier: item, action: 1 })">
                <v-icon icon="mdi-check-circle-outline" size="22"></v-icon>
              </v-btn>
              <v-btn v-if="canEdit && (item as Supplier).statusSupplier == 'Activo'" icon variant="text" color="red"
                size="small" title="Inactivar" @click="$emit('open-modal', { supplier: item, action: 2 })">
                <v-icon icon="mdi-close-circle-outline" size="22"></v-icon>
              </v-btn>
              <v-btn v-if="canDelete" icon variant="text" color="blue-grey" size="small" title="Eliminar"
                @click="$emit('open-modal', { supplier: item, action: 0 })">
                <v-icon icon="mdi-trash-can-outline" size="22"></v-icon>
              </v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title> <v-avatar color="purple-darken-1" size="36" class="mr-3">
                <v-icon icon="mdi-contacts" color="white" size="18"></v-icon>
              </v-avatar>Gestión de Proveedores</v-toolbar-title>
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
              title="Agregar" class="mr-2">
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