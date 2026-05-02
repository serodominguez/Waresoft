<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localSupplier.idSupplier ? 'Editar Proveedor' : 'Agregar Proveedor' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text class="pb-0">
        <v-form ref="formRef" v-model="valid">
          <v-container class="pa-0">
            <v-row density="compact">
              <v-col cols="12" md="12">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localSupplier.companyName"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" label="Nombre de la empresa"
                  required />
              </v-col>
              <v-col cols="12" md="12">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localSupplier.contact"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" label="Contacto" />
              </v-col>
              <v-col cols="6" md="6">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localSupplier.phoneNumber"
                  counter="8" :rules="[rules.onlyNumbers]" :maxlength="8" label="Teléfono" />
              </v-col>
              <v-col cols="6" md="6">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localSupplier.email"
                  counter="50" :rules="[rules.email]" :maxlength="50" label="Correo" />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-card-actions class="px-6 pb-4 pt-2">
        <v-btn color="green" dark elevation="4" @click="saveSupplier" :disabled="!valid"
          :loading="saving">Guardar</v-btn>
        <v-btn color="red" dark elevation="4" @click="close">Cancelar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useToast } from 'vue-toastification';
import { useSupplierStore } from '@/stores/supplierStore';
import { Supplier } from '@/interfaces/supplierInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

interface Props {
  modelValue: boolean;
  supplier?: Supplier | null;
}

const props = withDefaults(defineProps<Props>(), {
  supplier: () => ({
    idSupplier: null,
    companyName: '',
    contact: '',
    phoneNumber: null,
    email: '',
    auditCreateDate: '',
    statusSupplier: ''
  })
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [];
}>();

const supplierStore = useSupplierStore();
const toast = useToast();

const formRef = ref<FormRef | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const localSupplier = ref<Supplier>({ ...props.supplier } as Supplier);

const rules = {
  required: (value: string) => !!value || 'Este campo es requerido.',
  onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.',
  onlyNumbers: (value: string) => !value || /^[0-9]+$/.test(value) || 'Solo se permiten números.',
  email: (value: string) => !value || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) || 'Formato de correo inválido.',
};

watch(() => props.modelValue, (newValue: boolean) => {
  isOpen.value = newValue;
});

watch(isOpen, (newValue: boolean) => {
  emit('update:modelValue', newValue);
});

watch(() => props.supplier, (newSupplier) => {
  if (newSupplier) {
    localSupplier.value = { ...newSupplier } as Supplier;
  }
}, { deep: true });

const close = () => {
  isOpen.value = false;
};

const saveSupplier = async () => {
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
    const isEditing = !!localSupplier.value.idSupplier;

    const supplierData = {
      ...localSupplier.value,
      phoneNumber: localSupplier.value.phoneNumber ?? null
    };

    let result;

    if (isEditing && localSupplier.value.idSupplier !== null) {
      result = await supplierStore.edit(localSupplier.value.idSupplier, supplierData);
    } else {
      result = await supplierStore.register(supplierData);
    }

    if (result.isSuccess) {
      const successMsg = isEditing
        ? 'Proveedor editado con éxito!'
        : 'Proveedor agregado con éxito!';

      toast.success(successMsg);
      emit('saved');
      close();
    }
  } catch (error: any) {
    const isEditing = !!localSupplier.value.idSupplier;
    const customMessage = isEditing
      ? 'Error en editar al proveedor'
      : 'Error en agregar al proveedor';

    handleApiError(error, customMessage);
  } finally {
    saving.value = false;
  }
};
</script>