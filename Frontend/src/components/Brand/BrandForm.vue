<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localBrand.idBrand ? 'Editar Marca' : 'Agregar Marca' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text class="pb-0">
        <v-form ref="formRef" v-model="valid">
          <v-container class="pa-0">
            <v-row density="compact">
              <v-col cols="12" md="12">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localBrand.brandName"
                  :rules="[rules.required]" counter="25" :maxlength="25" label="Nombre de la marca" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-card-actions class="px-6 pb-4 pt-2">
        <v-btn color="green" dark elevation="4" @click="saveBrand" :disabled="!valid" :loading="saving">Guardar</v-btn>
        <v-btn color="red" dark elevation="4" @click="close">Cancelar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useToast } from 'vue-toastification';
import { useBrandStore } from '@/stores/brandStore';
import { Brand } from '@/interfaces/brandInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

/**
 * Props del componente:
 * - modelValue: Controla si el diálogo está abierto o cerrado (patrón v-model)
 * - brand: Objeto con los datos de la entidad a editar, o valores por defecto para nueva entidad
 */
interface Props {
  modelValue: boolean;
  brand?: Brand | null;
}

//Props & Emits
const props = withDefaults(defineProps<Props>(), {
  brand: () => ({
    idBrand: null,
    brandName: '',
    auditCreateDate: '',
    statusBrand: ''
  })
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [];
}>();

//Servicios
const brandStore = useBrandStore();
const toast = useToast();

//Refs
const formRef = ref<FormRef | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const localBrand = ref<Brand>({ ...props.brand } as Brand);

//Reglas de validación
const rules = {
  required: (value: string) => !!value || 'Este campo es requerido.'
};

//Watchers
watch(() => props.modelValue, (newValue: boolean) => {
  isOpen.value = newValue;
});

watch(isOpen, (newValue: boolean) => {
  emit('update:modelValue', newValue);
});

watch(() => props.brand, (newBrand) => {
  if (newBrand) {
    localBrand.value = { ...newBrand } as Brand;
  }
}, { deep: true });

//Methods
const close = () => {
  isOpen.value = false;
};

const saveBrand = async () => {
  if (!formRef.value) {
    toast.warning('Error al acceder al formulario');
    return;
  }

  const validation = await formRef.value.validate();

  if (!validation.valid) {
    toast.warning('Por favor completa todos los campos requeridos');
    return;
  }

  saving.value = true;

  try {
    const isEditing = !!localBrand.value.idBrand;
    let result;

    if (isEditing && localBrand.value.idBrand !== null) {
      result = await brandStore.edit(
        localBrand.value.idBrand,
        { ...localBrand.value }
      );
    } else {
      result = await brandStore.register({ ...localBrand.value });
    }

    if (result.isSuccess) {
      const successMsg = isEditing
        ? 'Marca editada con éxito!'
        : 'Marca agregada con éxito!';

      toast.success(successMsg);
      emit('saved');
      close();
    }

  } catch (error: any) {
    const isEditing = !!localBrand.value.idBrand;
    const customMessage = isEditing
      ? 'Error al editar la marca'
      : 'Error al agregar la marca';

    handleApiError(error, customMessage);
  } finally {
    saving.value = false;
  }
};
</script>