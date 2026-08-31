<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>Abrir Período de Inventario</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text class="pb-0">
        <v-form ref="formRef" v-model="valid">
          <v-container class="pa-0">
            <v-row density="compact">
              <v-col cols="12">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localPeriod.periodName"
                  :rules="[rules.required]" counter="50" :maxlength="50" label="Nombre del período" required />
              </v-col>
              <v-col cols="12" md="6">
                <v-date-input color="indigo" variant="outlined" density="compact" v-model="localPeriod.startDate"
                  :rules="[rules.required]" label="Fecha de inicio" prepend-icon="" required />
              </v-col>
              <v-col cols="12" md="6">
                <v-date-input color="indigo" variant="outlined" density="compact" v-model="localPeriod.endDate"
                  :rules="[rules.required, rules.afterStart]" label="Fecha de fin" prepend-icon="" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-card-actions class="px-6 pb-4 pt-2">
        <v-btn color="green" dark elevation="4" @click="save" :disabled="!valid" :loading="saving">
          Guardar
        </v-btn>
        <v-btn color="red" dark elevation="4" @click="close">Cancelar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useToast } from 'vue-toastification';
import { useInventoryPeriodStore } from '@/stores/inventoryPeriodStore';
import { InventoryPeriodOpenRequest } from '@/interfaces/inventoryPeriodInterface';
import { formatDateForApi } from '@/utils/date';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

const props = defineProps<{ modelValue: boolean }>();

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [];
}>();

const periodStore = useInventoryPeriodStore();
const toast = useToast();

const formRef = ref<FormRef | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);

const defaultPeriod = (): InventoryPeriodOpenRequest => ({
  periodName: '',
  startDate: '',
  endDate: '',
});

const localPeriod = ref<InventoryPeriodOpenRequest>(defaultPeriod());

const rules = {
  required: (value: string) => !!value || 'Este campo es requerido.',
  afterStart: (value: string) => {
    if (!localPeriod.value.startDate || !value) return true;
    return value >= localPeriod.value.startDate || 'La fecha de fin debe ser posterior al inicio.';
  },
};

watch(() => props.modelValue, (val) => { isOpen.value = val; });
watch(isOpen, (val) => { emit('update:modelValue', val); });

const close = () => {
  isOpen.value = false;
  localPeriod.value = defaultPeriod();
};

const save = async () => {
  if (!formRef.value) return;

  const validation = await formRef.value.validate();
  if (!validation.valid) {
    toast.warning('Por favor completa todos los campos requeridos');
    return;
  }

  saving.value = true;
  try {
    const periodData = {
      periodName: localPeriod.value.periodName,
      startDate:  formatDateForApi(localPeriod.value.startDate),
      endDate:    formatDateForApi(localPeriod.value.endDate),
    };

    const result = await periodStore.openPeriod(periodData);
    
    if (result.isSuccess) {
      toast.success('Período abierto con éxito!');
      emit('saved');
      close();
    }
  } catch (error) {
      handleApiError(error, 'Error al abrir el período');
  } finally {
    saving.value = false;
  }
};
</script>