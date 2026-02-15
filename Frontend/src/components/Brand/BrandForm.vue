<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localBrand.idBrand ? 'Editar Marca' : 'Agregar Marca' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="formRef" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localBrand.brandName"
                  :rules="[rules.required]" counter="25" :maxlength="25" label="Nombre de la marca" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveBrand" :disabled="!valid"
            :loading="saving">Guardar</v-btn>
          <v-btn color="red" dark class="mb-2" elevation="4" @click="close">Cancelar</v-btn>
        </v-card-actions>
      </v-col>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useToast } from 'vue-toastification';
import { useBrandStore } from '@/stores/brandStore';
import { Brand } from '@/interfaces/brandInterface';
import { handleApiError } from '@/helpers/errorHandler';

// Interfaz para el tipo de referencia del formulario
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

const props = withDefaults(defineProps<Props>(), {
  brand: () => ({
    idBrand: null,
    brandName: '',
    auditCreateDate: '',
    statusBrand: ''
  })
});

// Definir emits
const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [brand: Brand];
}>();

// Inicialización de servicios
const brandStore = useBrandStore();
const toast = useToast();

// Referencias del formulario - IMPORTANTE: el nombre debe coincidir con ref="formRef" en el template
const formRef = ref<FormRef | null>(null);

// Estado reactivo del componente
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const localBrand = ref<Brand>({ ...props.brand } as Brand);

// Reglas de validación
const rules = {
  required: (value: string) => !!value || 'Este campo es requerido.'
};

// Watchers para sincronizar props con estado local
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

// Cierra el diálogo
const close = () => {
  isOpen.value = false;
};

// Guarda o actualiza la entidad
const saveBrand = async () => {
  // Valida el formulario
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
    // Determina si es edición o creación según si existe id de la entidad
    const isEditing = !!localBrand.value.idBrand;
    let result;

    if (isEditing && localBrand.value.idBrand !== null) {
      // Llama a la action de Pinia para editar entidad existente
      result = await brandStore.editBrand(
        localBrand.value.idBrand,
        { ...localBrand.value }
      );
    } else {
      // Llama a la action de Pinia para registrar nuevo
      result = await brandStore.registerBrand({ ...localBrand.value });
    }

    // Si la operación fue exitosa
    if (result.isSuccess) {
      const successMsg = isEditing
        ? 'Marca editada con éxito!'
        : 'Marca agregada con éxito!';

      toast.success(successMsg);
      emit('saved', { ...localBrand.value });
      close();
    }

  } catch (error: any) {
    // Manejo de errores
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