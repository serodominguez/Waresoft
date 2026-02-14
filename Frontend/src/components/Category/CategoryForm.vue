<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localCategory.idCategory ? 'Editar Categoría' : 'Agregar Categoría' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="formRef" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCategory.categoryName"
                  :rules="[rules.required]" counter="25" :maxlength="25"
                  label="Nombre de la Categoría" required />
              </v-col>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCategory.description" counter="50" :maxlength="50" label="Descripción" />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveCategory" :disabled="!valid"
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
import { useCategoryStore } from '@/stores/categoryStore';
import { Category } from '@/interfaces/categoryInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

interface Props {
  modelValue: boolean;
  category?: Category | null;
}

const props = withDefaults(defineProps<Props>(), {
  category: () => ({
    idCategory: null,
    categoryName: '',
    description: '',
    auditCreateDate: '',
    statusCategory: ''
  })
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [category: Category];
}>();

const categoryStore = useCategoryStore();
const toast = useToast();

const formRef = ref<FormRef | null>(null);

const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const localCategory = ref<Category>({ ...props.category } as Category);

const rules = {
  required: (value: string) => !!value || 'Este campo es requerido.',
  onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.'
};

watch(() => props.modelValue, (newValue: boolean) => {
  isOpen.value = newValue;
});

watch(isOpen, (newValue: boolean) => {
  emit('update:modelValue', newValue);
});

watch(() => props.category, (newCategory) => {
  if (newCategory) {
    localCategory.value = { ...newCategory } as Category;
  }
}, { deep: true });

const close = () => {
  isOpen.value = false;
};

const saveCategory = async () => {
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
    const isEditing = !!localCategory.value.idCategory;
    let result;

    if (isEditing && localCategory.value.idCategory !== null) {
      result = await categoryStore.editCategory(
        localCategory.value.idCategory,
        { ...localCategory.value }
      );
    } else {
      result = await categoryStore.registerCategory({ ...localCategory.value });
    }

    if (result.isSuccess) {
      const successMsg = isEditing
        ? 'Categoría actualizada con éxito!'
        : 'Categoría registrada con éxito!';

      toast.success(successMsg);
      emit('saved', { ...localCategory.value });
      close();
    }

  } catch (error: any) {
    const isEditing = !!localCategory.value.idCategory;
    const customMessage = isEditing
      ? 'Error en actualizar la categoría'
      : 'Error en guardar la categoría';

    handleApiError(error, customMessage);
  } finally {
    saving.value = false;
  }
};
</script>