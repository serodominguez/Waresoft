<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localRole.idRole ? 'Editar Rol' : 'Agregar Rol' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text class="pb-0">
        <v-form ref="formRef" v-model="valid">
          <v-container class="pa-0">
            <v-row density="compact">
              <v-col cols="12" md="12">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localRole.roleName"
                  :rules="[rules.required, rules.onlyLetters]" counter="20" :maxlength="20" label="Nombre del rol"
                  required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-card-actions class="px-6 pb-4 pt-2">
        <v-btn color="green" dark elevation="4" @click="saveRole" :disabled="!valid" :loading="saving">Guardar</v-btn>
        <v-btn color="red" dark elevation="4" @click="close">Cancelar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useToast } from 'vue-toastification';
import { useRoleStore } from '@/stores/roleStore';
import { Role } from '@/interfaces/roleInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

interface Props {
  modelValue: boolean;
  role?: Role | null;
}

const props = withDefaults(defineProps<Props>(), {
  role: () => ({
    idRole: null,
    roleName: '',
    auditCreateDate: '',
    statusRole: ''
  })
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [];
}>();

const roleStore = useRoleStore();
const toast = useToast();

const formRef = ref<FormRef | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const localRole = ref<Role>({ ...props.role } as Role);

const rules = {
  required: (value: string) => !!value || 'Este campo es requerido.',
  onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.',
};

watch(() => props.modelValue, (newValue: boolean) => {
  isOpen.value = newValue;
});

watch(isOpen, (newValue: boolean) => {
  emit('update:modelValue', newValue);
});

watch(() => props.role, (newRole) => {
  if (newRole) {
    localRole.value = { ...newRole } as Role;
  }
}, { deep: true });

const close = () => {
  isOpen.value = false;
};

const saveRole = async () => {
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
    const isEditing = !!localRole.value.idRole;
    let result;

    if (isEditing && localRole.value.idRole !== null) {
      result = await roleStore.edit(
        localRole.value.idRole,
        { ...localRole.value }
      );
    } else {
      result = await roleStore.register({ ...localRole.value });
    }

    if (result.isSuccess) {
      const successMsg = isEditing
        ? 'Rol editado con éxito!'
        : 'Rol agregado con éxito!';

      toast.success(successMsg);
      emit('saved');
      close();
    }
  } catch (error: any) {
    const isEditing = !!localRole.value.idRole;
    const customMessage = isEditing
      ? 'Error en editar el rol'
      : 'Error en agregar el rol';

    handleApiError(error, customMessage);
  } finally {
    saving.value = false;
  }
};
</script>