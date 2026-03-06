<template>
  <v-data-table :headers="headers" :items="permissions" :loading="loading" loading-text="Cargando permisos..."
    no-data-text="Seleccione un rol y presione en Cargar" class="elevation-1 mt-4" :items-per-page="-1"
    :hide-default-footer="true">

    <!-- Headers personalizados con checkboxes -->
    <template v-slot:header.permissions.crear="{ column }">
      <div class="d-flex align-center gap-1">
        <v-checkbox v-model="allChecked.crear" color="indigo" hide-details :indeterminate="isIndeterminate('crear')"
          @change="toggleAll('crear')" density="compact" :disabled="!hasPermissions" />
        {{ column.title }}
      </div>
    </template>
    <template v-slot:header.permissions.leer="{ column }">
      <div class="d-flex align-center gap-1">
        <v-checkbox v-model="allChecked.leer" color="indigo" hide-details :indeterminate="isIndeterminate('leer')"
          @change="toggleAll('leer')" density="compact" :disabled="!hasPermissions" />
        {{ column.title }}
      </div>
    </template>
    <template v-slot:header.permissions.editar="{ column }">
      <div class="d-flex align-center gap-1">
        <v-checkbox v-model="allChecked.editar" color="indigo" hide-details :indeterminate="isIndeterminate('editar')"
          @change="toggleAll('editar')" density="compact" :disabled="!hasPermissions" />
        {{ column.title }}
      </div>
    </template>
    <template v-slot:header.permissions.eliminar="{ column }">
      <div class="d-flex align-center gap-1">
        <v-checkbox v-model="allChecked.eliminar" color="indigo" hide-details :indeterminate="isIndeterminate('eliminar')"
          @change="toggleAll('eliminar')" density="compact" :disabled="!hasPermissions" />
        {{ column.title }}
      </div>
    </template>
    <template v-slot:header.permissions.descargar="{ column }">
      <div class="d-flex align-center gap-1">
        <v-checkbox v-model="allChecked.descargar" color="indigo" hide-details :indeterminate="isIndeterminate('descargar')"
          @change="toggleAll('descargar')" density="compact" :disabled="!hasPermissions" />
        {{ column.title }}
      </div>
    </template>

    <!-- Celdas de items (sin cambios) -->
    <template v-slot:item.permissions.crear="{ item }">
      <v-checkbox v-model="item.permissions.crear" color="indigo" hide-details @change="$emit('permission-changed')" />
    </template>
    <template v-slot:item.permissions.leer="{ item }">
      <v-checkbox v-model="item.permissions.leer" color="indigo" hide-details @change="$emit('permission-changed')" />
    </template>
    <template v-slot:item.permissions.editar="{ item }">
      <v-checkbox v-model="item.permissions.editar" color="indigo" hide-details @change="$emit('permission-changed')" />
    </template>
    <template v-slot:item.permissions.eliminar="{ item }">
      <v-checkbox v-model="item.permissions.eliminar" color="indigo" hide-details @change="$emit('permission-changed')" />
    </template>
    <template v-slot:item.permissions.descargar="{ item }">
      <v-checkbox v-model="item.permissions.descargar" color="indigo" hide-details @change="$emit('permission-changed')" />
    </template>
  </v-data-table>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { PermissionsByModule } from '@/interfaces/permissionInterface';

type PermissionKey = 'crear' | 'leer' | 'editar' | 'eliminar' | 'descargar';

// Props
interface Props {
  permissions: PermissionsByModule[];
  loading: boolean;
}

const props = defineProps<Props>();

// Emits
const emit = defineEmits<{
  'permission-changed': [];
}>();

// Estado de los checkboxes del header (calculado dinámicamente)
const allChecked = computed(() => ({
  crear:     props.permissions.every(p => p.permissions.crear),
  leer:      props.permissions.every(p => p.permissions.leer),
  editar:    props.permissions.every(p => p.permissions.editar),
  eliminar:  props.permissions.every(p => p.permissions.eliminar),
  descargar: props.permissions.every(p => p.permissions.descargar),
}));

// Determina si el checkbox debe mostrar estado indeterminado (algunos pero no todos)
const isIndeterminate = (key: PermissionKey): boolean => {
  const some = props.permissions.some(p => p.permissions[key]);
  const all  = props.permissions.every(p => p.permissions[key]);
  return some && !all;
};

// Marca o desmarca todos los items de esa columna
const toggleAll = (key: PermissionKey) => {
  const newValue = !allChecked.value[key];
  props.permissions.forEach(p => {
    p.permissions[key] = newValue;
  });
  emit('permission-changed');
};

const hasPermissions = computed(() => props.permissions.length > 0);

// Headers
const headers = computed(() => [
  { title: 'Módulo',     key: 'module',                sortable: false },
  { title: 'Crear',      key: 'permissions.crear',     sortable: false },
  { title: 'Leer',       key: 'permissions.leer',      sortable: false },
  { title: 'Editar',     key: 'permissions.editar',    sortable: false },
  { title: 'Eliminar',   key: 'permissions.eliminar',  sortable: false },
  { title: 'Descargar',  key: 'permissions.descargar', sortable: false },
]);
</script>