<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>Editar Precio</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="formRef" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.code" label="Código"
                  readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.description"
                  label="Descripción" readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.material" label="Material"
                  readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.unitMeasure"
                  label="Unidad de medida" readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.brandName" label="Marca"
                  readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.categoryName"
                  label="Categoría" readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.stockAvailable"
                  label="Cantidad" readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model.number="localInventory.price" counter="5"
                  type="number" :rules="[rules.required]" :maxlength="5" label="Precio" ref="priceFieldRef"
                  @focus="($event.target as HTMLInputElement).select()" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="savePrice" :disabled="!valid"
            :loading="saving">Guardar</v-btn>
          <v-btn color="red" dark class="mb-2" elevation="4" @click="close">Cancelar</v-btn>
        </v-card-actions>
      </v-col>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, nextTick } from 'vue';
import { useToast } from 'vue-toastification';
import { useInventoryStore } from '@/stores/inventoryStore';
import { Inventory } from '@/interfaces/inventoryInterface';
import { handleApiError } from '@/helpers/errorHandler';

// Props
interface Props {
  modelValue: boolean;
  inventory?: Inventory | null;
}

const props = withDefaults(defineProps<Props>(), {
  inventory: () => ({
    idStore: null,
    idProduct: null,
    code: '',
    description: '',
    material: '',
    color: '',
    unitMeasure: '',
    stockAvailable: null,
    stockInTransit: null,
    price: null,
    replenishment: '',
    brandName: '',
    categoryName: '',
    auditCreateDate: ''
  })
});

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [];
}>();

// Servicios
const inventoryStore = useInventoryStore();
const toast = useToast();

// Referencias del template
const formRef = ref<{ validate: () => Promise<{ valid: boolean }> }>();
const priceFieldRef = ref<HTMLInputElement>();

// Estado reactivo
const valid = ref(false);
const saving = ref(false);
const localInventory = reactive<Inventory>({ ...props.inventory } as Inventory);

// Reglas de validación
const rules = {
  required: (value: string | number) => !!value || 'Este campo es requerido.',
  onlyNumbers: (value: string) => !value || /^[0-9]+$/.test(value) || 'Solo se permiten números.',
};

// Computed bidireccional para v-model
const isOpen = computed({
  get: () => props.modelValue,
  set: (value: boolean) => emit('update:modelValue', value)
});

// Watchers
watch(
  () => props.modelValue,
  (newValue) => {
    if (newValue) {
      // Espera a que el modal se renderice completamente
      nextTick(() => {
        if (priceFieldRef.value) {
          priceFieldRef.value.focus();
        }
      });
    }
  }
);

watch(
  () => props.inventory,
  (newInventory) => {
    if (newInventory) {
      Object.assign(localInventory, { ...newInventory });
    }
  },
  { deep: true }
);

// Métodos
const close = () => {
  isOpen.value = false;
};

const savePrice = async () => {
  if (!formRef.value) return;

  const { valid: isValid } = await formRef.value.validate();
  
  if (!isValid) {
    toast.warning('Por favor completa todos los campos requeridos');
    return;
  }

  try {
    saving.value = true;

    const result = await inventoryStore.editInventoryPrice({ ...localInventory });

    if (result.isSuccess) {
      toast.success('Precio actualizado con éxito!');
      emit('saved');
      close();
    }
  } catch (error: any) {
    handleApiError(error, 'Error al actualizar el precio');
  } finally {
    saving.value = false;
  }
};
</script>