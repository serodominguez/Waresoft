<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localModule.idModule ? 'Editar Módulo' : 'Agregar Módulo' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="formRef" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localModule.moduleName"
                  :rules="[rules.required, rules.onlyLetters]" counter="25" :maxlength="25" label="Nombre del Módulo"
                  required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="indigo" dark class="mb-2" elevation="4" @click="saveModule" :disabled="!valid"
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
import { useModuleStore } from '@/stores/moduleStore';
import { Module } from '@/interfaces/moduleInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

interface Props {
  modelValue: boolean;
  module?: Module | null;
}

const props = withDefaults(defineProps<Props>(), {
  module: () => ({
    idModule: null,
    moduleName: '',
    auditCreateDate: '',
    statusModule: ''
  })
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [module: Module];
}>();

const moduleStore = useModuleStore();
const toast = useToast();

const formRef = ref<FormRef | null>(null);

const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const localModule = ref<Module>({ ...props.module } as Module);

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

watch(() => props.module, (newModule) => {
  if (newModule) {
    localModule.value = { ...newModule } as Module;
  }
}, { deep: true });

const close = () => {
  isOpen.value = false;
};

const saveModule = async () => {
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
    const isEditing = !!localModule.value.idModule;
    let result;

    if (isEditing && localModule.value.idModule !== null) {
      result = await moduleStore.editModule(
        localModule.value.idModule,
        { ...localModule.value }
      );
    } else {
      result = await moduleStore.registerModule({ ...localModule.value });
    }

    if (result.isSuccess) {
      const successMsg = isEditing
        ? 'Módulo actualizado con éxito!'
        : 'Módulo registrado con éxito!';

      toast.success(successMsg);
      emit('saved', { ...localModule.value });
      close();
    }

  } catch (error: any) {
    const isEditing = !!localModule.value.idModule;
    const customMessage = isEditing
      ? 'Error en actualizar el módulo'
      : 'Error en guardar el módulo';

    handleApiError(error, customMessage);
  } finally {
    saving.value = false;
  }
};
</script>