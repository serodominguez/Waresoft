<template>
    <div>
        <v-card elevation="2">
            <v-data-table :headers="dynamicHeaders" :items="rows" :loading="loading"
                loading-text="Cargando... Espere por favor" items-per-page-text="Artículos por Página">
                <template v-slot:top>
                    <v-toolbar>
                        <v-toolbar-title>Consolidado de Existencias</v-toolbar-title>
                    </v-toolbar>
                </template>
                <template v-slot:item="{ item }">
                    <tr>
                        <td>{{ item.codigo }}</td>
                        <td>{{ item.color }}</td>
                        <td>{{ item.marca }}</td>
                        <td>{{ item.categoria }}</td>
                        <td v-for="store in stores" :key="store">
                            {{ item.stockByStore[store] ?? 0 }}
                        </td>
                    </tr>
                </template>
                <template v-slot:no-data>
                    <span>Sin datos disponibles</span>
                </template>
            </v-data-table>
        </v-card>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { InventoryPivotRow } from '@/interfaces/inventoryInterface';

interface Props {
  stores: string[];
  rows: InventoryPivotRow[];
  loading: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  stores: () => [],
  rows: () => [],
  loading: false
});

const dynamicHeaders = computed(() => [
  { title: 'Código', key: 'codigo', sortable: false },
  { title: 'Color', key: 'color', sortable: false },
  { title: 'Marca', key: 'marca', sortable: false },
  { title: 'Categoría', key: 'categoria', sortable: false },
  ...props.stores.map(store => ({
    title: store,
    key: store,
    sortable: false
  }))
]);
</script>