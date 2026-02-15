<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="users" :search="search || undefined" :items-per-page-text="pages"
        :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalUsers"
        :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as User).userName }}</td>
            <td>{{ (item as User).names }}</td>
            <td>{{ (item as User).lastNames }}</td>
            <td>{{ (item as User).phoneNumber }}</td>
            <td>{{ (item as User).roleName }}</td>
            <td>{{ (item as User).storeName }}</td>
            <td>{{ (item as User).auditCreateDate }}</td>
            <td>{{ (item as User).statusUser }}</td>
            <td class="text-center">
              <v-btn v-if="canEdit && (item as User).statusUser == 'Activo'" icon="edit" variant="text"
                @click="$emit('edit-user', item)" size="small" title="Editar">
              </v-btn>
              <template v-if="canEdit && (item as User).statusUser == 'Inactivo'">
                <v-btn icon="check" variant="text" @click="$emit('open-modal', { user: item, action: 1 })" size="small"
                  title="Activar">
                </v-btn>
              </template>
              <template v-if="canEdit && (item as User).statusUser == 'Activo'">
                <v-btn icon="block" variant="text" @click="$emit('open-modal', { user: item, action: 2 })" size="small"
                  title="Inactivar">
                </v-btn>
              </template>
              <v-btn v-if="canDelete" icon="delete" variant="text"
                @click="$emit('open-modal', { user: item, action: 0 })" size="small" title="Eliminar">
              </v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Usuarios</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-pdf-box" @click="handleDownloadPdf" :loading="downloadingPdf"
              title="Descargar PDF">
            </v-btn>
            <v-btn v-if="canDownload" icon="mdi:mdi-file-excel-box" @click="handleDownloadExcel"
              :loading="downloadingExcel" title="Descargar Excel"></v-btn>
            <v-btn icon="tune" @click="drawerModel = !drawerModel" title="Filtros"></v-btn>
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
          <v-btn color="indigo" @click="$emit('fetch-users')"> Reset </v-btn>
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
import { User } from '@/interfaces/userInterface';
import { BaseListProps } from '@/interfaces/baselistInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';

interface Props extends Omit<BaseListProps<User>, 'items' | 'totalItems'> {
  users: User[];
  totalUsers: number;
}

const props = withDefaults(defineProps<Props>(), {
  drawer: false,
  selectedFilter: 'Usuario',
  state: 'Activos',
  startDate: null,
  endDate: null,
  downloadingExcel: false,
  downloadingPdf: false,
  itemsPerPage: 10
});

const emit = defineEmits<{
  'open-form': [];
  'open-modal': [payload: { user: User; action: 0 | 1 | 2 }]
  'edit-user': [user: User];
  'fetch-users': [];
  'search-users': [params: {
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

const pages = ref("Usuarios por Página");
const search = ref<string | null>(null);
const filterOptions = ref(['Usuario', 'Nombres', 'Apellidos', 'Tienda', 'Rol']);

const headers = computed(() => [
  { title: 'Usuario', key: 'userName', sortable: false },
  { title: 'Nombres', key: 'names', sortable: false },
  { title: 'Apellidos', key: 'lastNames', sortable: false },
  { title: 'Teléfono', key: 'phoneNumber', sortable: false },
  { title: 'Rol', key: 'roleName', sortable: false },
  { title: 'Establecimiento', key: 'storeName', sortable: false },
  { title: 'Fecha de creación', key: 'auditCreateDate', sortable: false },
  { title: 'Estado', key: 'statusUser', sortable: false },
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
  emit('search-users', {
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
