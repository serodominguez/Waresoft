<template>
  <v-navigation-drawer v-model="drawerModel" temporary app>
    <v-list>
      <v-list-item>
        <div class="d-flex justify-space-between align-center w-100">
          <v-list-item-title class="text-h6">Filtros</v-list-item-title>
          <v-btn icon="close" variant="text" size="small" @click="drawerModel = false"></v-btn>
        </div>
      </v-list-item>
      <div class="px-4 pt-4 pb-2">
        <v-select v-model="selectedFilterModel" :items="filters" label="Buscar por:" variant="outlined"
          density="compact" hide-details></v-select>
      </div>
      <div class="px-4 py-2">
        <v-select v-model="stateModel" :items="statusOptions" label="Estado" variant="outlined" density="compact"
          hide-details></v-select>
      </div>
      <div class="px-4 py-2">
        <v-date-input v-model="startDateModel" label="Desde:" prepend-icon="" variant="underlined"
          persistent-placeholder hide-details></v-date-input>
      </div>
      <div class="px-4 py-2">
        <v-date-input v-model="endDateModel" label="Hasta:" prepend-icon="" variant="underlined" persistent-placeholder
          hide-details></v-date-input>
      </div>
      <v-list-item class="pt-6">
        <v-btn color="indigo" block @click="applyFilters">Aplicar</v-btn>
      </v-list-item>
      <v-list-item>
        <v-btn color="indigo" block @click="clearFilters">Limpiar</v-btn>
      </v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  modelValue: boolean;
  filters: string[];
  selectedFilter: string;
  statusOptions: string[];
  state?: string;
  startDate?: Date | null;
  endDate?: Date | null;
}

const props = withDefaults(defineProps<Props>(), {
  state: 'Todos',
  startDate: null,
  endDate: null
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'update:selectedFilter': [value: string];
  'update:state': [value: string];
  'update:startDate': [value: Date | null];
  'update:endDate': [value: Date | null];
  'apply-filters': [];
  'clear-filters': []; 
}>();

const drawerModel = computed({
  get: () => props.modelValue,
  set: (value: boolean) => emit('update:modelValue', value)
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

const applyFilters = () => {
  emit('apply-filters');
};

const clearFilters = () => {
  emit('clear-filters');
};
</script>