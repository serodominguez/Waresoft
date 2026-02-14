<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localStore.idStore ? 'Editar Establecimiento' : 'Agregar Establecimiento' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="formRef" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.storeName"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" label="Establecimiento"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.manager"
                  :rules="[rules.required, rules.onlyLetters]" counter="30" :maxlength="30" label="Encargado"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.address" :rules="[rules.required]"
                  counter="60" :maxlength="60" label="Dirección" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.phoneNumber" counter="8"
                  :rules="[rules.onlyNumbers]" :maxlength="8" label="Teléfono" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.city" counter="15"
                  :rules="[rules.required, rules.onlyLetters]" :maxlength="15" label="Ciudad" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.email" counter="50"
                  :rules="[rules.email]" :maxlength="50" label="Correo" />
              </v-col>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-select color="indigo" variant="underlined" :rules="[rules.required]" v-model="localStore.type"
                  :items="types" label="Tipo" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveStore" :disabled="!valid"
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
import { useStoreStore } from '@/stores/storeStore';
import { Store } from '@/interfaces/storeInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

interface Props {
  modelValue: boolean;
  store?: Store | null;
}

const props = withDefaults(defineProps<Props>(), {
  store: () => ({
    idStore: null,
    storeName: '',
    manager: '',
    address: '',
    phoneNumber: null,
    city: '',
    email: '',
    type: '',
    auditCreateDate: '',
    statusStore: ''
  })
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [store: Store];
}>();

const storeStore = useStoreStore();
const toast = useToast();

const formRef = ref<FormRef | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const localStore = ref<Store>({ ...props.store } as Store);
const types = ref(['Casa Matriz', 'Sucursal', 'Almacén']);

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

watch(() => props.store, (newStore) => {
  if (newStore) {
    localStore.value = { ...newStore } as Store;
  }
}, { deep: true });

const close = () => {
  isOpen.value = false;
};

const saveStore = async () => {
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
    const isEditing = !!localStore.value.idStore;
    let result;

    if (isEditing && localStore.value.idStore !== null) {
      result = await storeStore.editStore(
        localStore.value.idStore,
        { ...localStore.value }
      );
    } else {
      result = await storeStore.registerStore({ ...localStore.value });
    }

    if (result.isSuccess) {
      const successMsg = isEditing
        ? 'Establecimiento editado con éxito!'
        : 'Establecimiento agregado con éxito!';

      toast.success(successMsg);
      emit('saved', { ...localStore.value });
      close();
    }

  } catch (error: any) {
    const isEditing = !!localStore.value.idStore;
    const customMessage = isEditing
      ? 'Error en editar el establecimiento'
      : 'Error en agregar el establecimiento';

    handleApiError(error, customMessage);
  } finally {
    saving.value = false;
  }
};
</script>