<template>
    <v-dialog v-model="isOpen" max-width="400px" persistent>
        <v-card>
            <v-card-title class="bg-surface-light pt-4">
                <span>Imprimir Código de Barras</span>
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text class="pb-0">
                <v-form ref="formRef" v-model="valid">
                    <v-container class="pa-0">
                        <v-row density="compact">
                            <v-col cols="12">
                                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localCode"
                                    label="Código del producto" readonly />
                            </v-col>
                            <v-col cols="12">
                                <v-text-field color="indigo" variant="outlined" density="compact"
                                    v-model.number="quantity" label="Cantidad de etiquetas" type="number"
                                    :rules="[rules.required, rules.range]" ref="quantityFieldRef"
                                    @focus="($event.target as HTMLInputElement).select()" required />
                            </v-col>
                        </v-row>
                    </v-container>
                </v-form>
            </v-card-text>
            <v-card-actions class="px-6 pb-4 pt-2">
                <v-btn color="green" dark elevation="4" @click="handleGenerate" :disabled="!valid"
                    :loading="generating">
                    Generar
                </v-btn>
                <v-btn color="red" dark elevation="4" @click="close">Cancelar</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue';
import { useToast } from 'vue-toastification';
import { useProductStore } from '@/stores/productStore';
import { handleApiError } from '@/helpers/errorHandler';

interface Props {
  modelValue: boolean;
  productId: number | null;
  productCode: string;
}

const props = withDefaults(defineProps<Props>(), {
  productId: null,
  productCode: ''
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
}>();

const productStore = useProductStore();
const toast = useToast();

const formRef = ref<{ validate: () => Promise<{ valid: boolean }> }>();
const quantityFieldRef = ref<HTMLInputElement>();
const valid = ref(false);
const generating = ref(false);
const quantity = ref<number>(1);
const localCode = computed(() => props.productCode);

const rules = {
  required: (value: number) => !!value || 'Este campo es requerido.',
  range: (value: number) => (value >= 1 && value <= 200) || 'La cantidad debe estar entre 1 y 200.',
};

const isOpen = computed({
  get: () => props.modelValue,
  set: (value: boolean) => emit('update:modelValue', value)
});

watch(
  () => props.modelValue,
  (newValue) => {
    if (newValue) {
      quantity.value = 1;
      nextTick(() => {
        if (quantityFieldRef.value) {
          quantityFieldRef.value.focus();
        }
      });
    }
  }
);

const close = () => {
  isOpen.value = false;
};

const handleGenerate = async () => {
  if (!formRef.value) return;

  const { valid: isValid } = await formRef.value.validate();
  if (!isValid) {
    toast.warning('Por favor completa todos los campos requeridos');
    return;
  }

  try {
    generating.value = true;
    await productStore.generateBarcodePdf(props.productId!, quantity.value);
    toast.success('PDF generado con éxito!');
    close();
  } catch (error: any) {
    handleApiError(error, 'Error al generar el PDF');
  } finally {
    generating.value = false;
  }
};
</script>