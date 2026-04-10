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
            <td class="text-center">
              <v-tooltip text="Visualizar" location="bottom">
                <template v-slot:activator="{ props }">
                  <v-btn v-bind="props" icon size="small" variant="text" color="deep-purple-darken-1"
                    :loading="loadingPdfId === (item as KardexMovement).idMovement"
                    @click="handleOpenPdf(item as KardexMovement)">
                    <v-icon icon="mdi-file-eye" size="24" />
                  </v-btn>
                </template>
              </v-tooltip>
            </td>
          </tr>
        </template>
        <template v-slot:no-data>
          <span class="text-grey">No hay movimientos para mostrar</span>
        </template>
      </v-data-table-server>
    </v-card>
    <CommonViewerPdf v-model="pdfModal" :title="pdfModalTitle" :url="pdfUrl" :error="pdfError"
      @close="handleClosePdfModal" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { KardexMovement } from '@/interfaces/kardexInterface';
import { useGoodsIssueStore } from '@/stores/goodsIssueStore';
import { useGoodsReceiptStore } from '@/stores/goodsReceiptStore';
import { useTransferStore } from '@/stores/transferStore';
import CommonViewerPdf from '../Common/CommonViewerPdf.vue';

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

const goodsIssueStore = useGoodsIssueStore();
const goodsReceiptStore = useGoodsReceiptStore();
const transferStore = useTransferStore();

const pages = 'Movimientos por Página';

const pdfModal = ref(false);
const pdfUrl = ref<string | null>(null);
const pdfError = ref(false);
const pdfModalTitle = ref('');
const loadingPdfId = ref<number | null>(null);

const MOVEMENT_MAP: Record<string, { getBlob: (id: number) => Promise<Blob>; label: string }> = {
  'Entrada': {
    getBlob: (id) => goodsReceiptStore.getBlobGoodsReceiptPdf(id),
    label: 'Entrada',
  },
  'Salida': {
    getBlob: (id) => goodsIssueStore.getBlobGoodsIssuePdf(id),
    label: 'Salida',
  },
  'Traspaso': {
    getBlob: (id) => transferStore.getBlobTransferPdf(id),
    label: 'Traspaso',
  },
};

const headers = computed(() => [
  { title: 'Código', key: 'code', sortable: false, align: 'center' as const },
  { title: 'Fecha', key: 'date', sortable: false, align: 'center' as const },
  { title: 'Movimiento', key: 'movementType', sortable: false, align: 'center' as const },
  { title: 'Tipo', key: 'type', sortable: false, align: 'center' as const },
  { title: 'Cantidad', key: 'quantity', sortable: false, align: 'center' as const },
  { title: 'Acumulado', key: 'accumulatedStock', sortable: false, align: 'center' as const },
  { title: 'Estado', key: 'state', sortable: false, align: 'center' as const },
  { title: 'Acciones', key: 'actions', sortable: false, align: 'center' as const },
]);

const stateColor = (state: string): string => {
  const map: Record<string, string> = {
    'Completado': 'green',
    'Recibido': 'blue',
    'Enviado': 'orange',
  };
  return map[state] ?? 'grey';
};

const handleOpenPdf = async (item: KardexMovement) => {
  const handler = MOVEMENT_MAP[item.movementType];
  if (!handler) {
    console.warn(`movementType no mapeado: "${item.movementType}"`);
    return;
  }

  pdfUrl.value = null;
  pdfError.value = false;
  pdfModalTitle.value = `${handler.label} — ${item.code}`;
  pdfModal.value = true;
  loadingPdfId.value = item.idMovement;

  try {
    const blob = await handler.getBlob(item.idMovement);
    pdfUrl.value = window.URL.createObjectURL(blob);
  } catch {
    pdfError.value = true;
  } finally {
    loadingPdfId.value = null;
  }
};

const handleClosePdfModal = () => {
  if (pdfUrl.value) {
    window.URL.revokeObjectURL(pdfUrl.value);
    pdfUrl.value = null;
  }
  pdfError.value = false;
};
</script>