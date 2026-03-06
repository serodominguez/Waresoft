<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="movements" :items-per-page-text="pages"
        :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalMovements"
        :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td class="text-center">{{ (item as KardexMovement).code }}</td>
            <td class="text-center">{{ (item as KardexMovement).date }}</td>
            <td class="text-center">{{ (item as KardexMovement).movementType }}</td>
            <td class="text-center">{{ (item as KardexMovement).type }}</td>
            <td class="text-center">
              <span :class="(item as KardexMovement).quantity < 0 ? 'text-red' : 'text-green'">
                {{ (item as KardexMovement).quantity > 0
                  ? `+${(item as KardexMovement).quantity}`
                  : (item as KardexMovement).quantity }}
              </span>
            </td>
            <td class="text-center">{{ (item as KardexMovement).accumulatedStock }}</td>
            <td class="text-center">
              <v-chip :color="stateColor((item as KardexMovement).state)" variant="tonal" size="small">
                {{ (item as KardexMovement).state }}
              </v-chip>
            </td>
          </tr>
        </template>
        <template v-slot:no-data>
          <span class="text-grey">No hay movimientos para mostrar</span>
        </template>
      </v-data-table-server>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { KardexMovement } from '@/interfaces/kardexInterface';

interface Props {
  movements: KardexMovement[];
  totalMovements: number;
  loading: boolean;
  itemsPerPage?: number;
}

withDefaults(defineProps<Props>(), {
  itemsPerPage: 10,
});

defineEmits<{
  'update-items-per-page': [value: number];
  'change-page': [page: number];
}>();

const pages = 'Movimientos por Página';

const headers = computed(() => [
  { title: 'Código', key: 'code', sortable: false, align: 'center' as const },
  { title: 'Fecha', key: 'date', sortable: false, align: 'center' as const },
  { title: 'Movimiento', key: 'movementType',     sortable: false, align: 'center' as const },
  { title: 'Tipo', key: 'type', sortable: false, align: 'center' as const },
  { title: 'Cantidad', key: 'quantity', sortable: false, align: 'center' as const },
  { title: 'Acumulado', key: 'accumulatedStock', sortable: false, align: 'center' as const },
  { title: 'Estado', key: 'state', sortable: false, align: 'center' as const },
]);

const stateColor = (state: string): string => {
  const map: Record<string, string> = {
    'Completado': 'green',
    'Recibido': 'blue',
    'Enviado': 'orange',
  };
  return map[state] ?? 'grey';
};
</script>