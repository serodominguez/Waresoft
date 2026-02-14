<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localCustomer.idCustomer ? 'Editar Cliente' : 'Agregar Cliente' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="formRef" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCustomer.names"
                  :rules="[rules.required, rules.onlyLetters]" counter="25" :maxlength="25" label="Nombre del cliente"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCustomer.lastNames"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" label="Apellidos" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCustomer.identificationNumber"
                  counter="8" :maxlength="8" label="Carnet" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCustomer.phoneNumber" counter="8"
                  :rules="[rules.onlyNumbers]" :maxlength="8" label="Teléfono" />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveCustomer" :disabled="!valid"
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
import { useCustomerStore } from '@/stores/customerStore';
import { Customer } from '@/interfaces/customerInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

interface Props {
  modelValue: boolean;
  customer?: Customer | null;
}

const props = withDefaults(defineProps<Props>(), {
  customer: () => ({
    idCustomer: null,
    names: '',
    lastNames: '',
    identificationNumber: '',
    phoneNumber: null,
    auditCreateDate: '',
    statusCustomer: ''
  })
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [customer: Customer];
}>();

const customerStore = useCustomerStore();
const toast = useToast();

const formRef = ref<FormRef | null>(null);

const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const localCustomer = ref<Customer>({ ...props.customer } as Customer);

const rules = {
  required: (value: string) => !!value || 'Este campo es requerido.',
  onlyLetters: (value: string) =>
    !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) ||
    'Solo se permiten letras.',
  onlyNumbers: (value: string) =>
    !value || /^[0-9]+$/.test(value) ||
    'Solo se permiten números.'
};

watch(() => props.modelValue, (newValue: boolean) => {
  isOpen.value = newValue;
});

watch(isOpen, (newValue: boolean) => {
  emit('update:modelValue', newValue);
});

watch(() => props.customer, (newCustomer) => {
  if (newCustomer) {
    localCustomer.value = { ...newCustomer } as Customer;
  }
}, { deep: true });

const close = () => {
  isOpen.value = false;
};

const saveCustomer = async () => {
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
    const isEditing = !!localCustomer.value.idCustomer;
    let result;

    if (isEditing && localCustomer.value.idCustomer !== null) {
      result = await customerStore.editCustomer(
        localCustomer.value.idCustomer,
        { ...localCustomer.value }
      );
    } else {
      result = await customerStore.registerCustomer({ ...localCustomer.value });
    }

    if (result.isSuccess) {
      const successMsg = isEditing
        ? 'Cliente editado con éxito!'
        : 'Cliente agregado con éxito!';

      toast.success(successMsg);
      emit('saved', { ...localCustomer.value });
      close();
    }

  } catch (error: any) {
    const isEditing = !!localCustomer.value.idCustomer;
    const customMessage = isEditing
      ? 'Error en editar al cliente'
      : 'Error en agregar al cliente';

    handleApiError(error, customMessage);
  } finally {
    saving.value = false;
  }
};
</script>