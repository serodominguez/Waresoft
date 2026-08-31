<template>
    <v-dialog v-model="isOpen" max-width="450px" persistent>
        <v-card>
            <v-card-title class="bg-surface-light pt-4">
                <span>Cerrar Período</span>
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text class="pt-4">
                ¿Desea registrar el stock físico antes de cerrar el período
                <strong>{{ period?.periodName }}</strong>?
            </v-card-text>
            <v-card-actions class="px-6 pb-4 pt-2">
                <v-btn color="indigo" dark elevation="4" @click="goToPhysicalStock">
                    Sí, registrar stock
                </v-btn>
                <v-btn color="red" dark elevation="4" @click="confirmClose" :loading="closing">
                    No, cerrar directamente
                </v-btn>
                <v-btn color="grey" dark elevation="4" @click="close">Cancelar</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useToast } from 'vue-toastification';
import { useInventoryPeriodStore } from '@/stores/inventoryPeriodStore';
import { InventoryPeriod } from '@/interfaces/inventoryPeriodInterface';
import { handleApiError } from '@/helpers/errorHandler';

const props = defineProps<{
  modelValue: boolean;
  period: InventoryPeriod | null;
}>();

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'closed': [];
  'go-to-physical-stock': [period: InventoryPeriod];
}>();

const periodStore = useInventoryPeriodStore();
const toast = useToast();

const isOpen = ref(props.modelValue);
const closing = ref(false);

watch(() => props.modelValue, (val) => { isOpen.value = val; });
watch(isOpen, (val) => { emit('update:modelValue', val); });

const close = () => { isOpen.value = false; };

const confirmClose = async () => {
  if (!props.period) return;

  closing.value = true;
  try {
    await periodStore.closePeriod({
      idPeriod: props.period.idPeriod,
      physicalCounts: [],
    });
    toast.success('Período cerrado con éxito!');
    emit('closed');
    close();
  } catch (error: any) {
    handleApiError(error, 'Error al cerrar el período');
  } finally {
    closing.value = false;
  }
};

const goToPhysicalStock = () => {
  if (!props.period) return;
  emit('go-to-physical-stock', props.period);
  close();
};
</script>