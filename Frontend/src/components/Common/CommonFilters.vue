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
                <v-switch v-model="stateModel" :label="`Estado: ${stateModel}`" false-value="Inactivos"
                    true-value="Activos" color="indigo" hide-details></v-switch>
            </div>
            <div class="px-4 py-2">
                <v-date-input v-model="startDateModel" label="Desde:" prepend-icon="" variant="underlined"
                    persistent-placeholder hide-details></v-date-input>
            </div>
            <div class="px-4 py-2">
                <v-date-input v-model="endDateModel" label="Hasta:" prepend-icon="" variant="underlined"
                    persistent-placeholder hide-details></v-date-input>
            </div>
            <v-list-item class="pt-6">
                <v-btn color="indigo" block @click="emit('apply-filters')">APLICAR</v-btn>
            </v-list-item>
            <v-list-item>
                <v-btn color="indigo" block @click="clearFilters">LIMPIAR</v-btn>
            </v-list-item>
        </v-list>
    </v-navigation-drawer>
</template>

<script setup lang="ts">
import { useFiltersSync } from '@/composables/useModelSync';

// Props del componente
interface Props {
    modelValue: boolean;
    filters: string[];
    selectedFilter: string;
    state?: string;
    startDate?: Date | null;
    endDate?: Date | null;
}

const props = withDefaults(defineProps<Props>(), {
    state: 'Activos',
    startDate: null,
    endDate: null
});

// Emits del componente
const emit = defineEmits<{
    'update:modelValue': [value: boolean];
    'update:selectedFilter': [value: string];
    'update:state': [value: string];
    'update:startDate': [value: Date | null];
    'update:endDate': [value: Date | null];
    'apply-filters': [];
}>();

// Usa el composable para toda la lógica de sincronización
const {
    drawerModel,
    selectedFilterModel,
    stateModel,
    startDateModel,
    endDateModel,
    clearFilters
} = useFiltersSync(props, emit);
</script>