<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localUser.idUser ? 'Editar Usuario' : 'Agregar Usuario' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="formRef" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.userName"
                  :rules="[rules.required, rules.onlyLetters]" counter="20" :maxlength="20" @keyup="uppercase"
                  label="Usuario" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.passwordHash" type="password"
                  :rules="[rules.required]" label="Contraseña" clearable required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.names"
                  :rules="[rules.required, rules.onlyLetters]" counter="30" :maxlength="30" label="Nombres" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.lastNames"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" label="Apellidos"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.identificationNumber" counter="8"
                  :maxlength="8" label="Carnet" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.phoneNumber" counter="8"
                  :rules="[rules.onlyNumbers]" :maxlength="8" label="Teléfono" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-autocomplete color="indigo" variant="underlined" :items="rolesArray" v-model="localUser.idRole"
                  item-title="roleName" item-value="idRole" :rules="[rules.required]"
                  no-data-text="No hay datos disponibles" label="Rol" required :loading="loadingRoles" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-autocomplete color="indigo" variant="underlined" :items="storesArray" v-model="localUser.idStore"
                  item-title="storeName" item-value="idStore" :rules="[rules.required]"
                  no-data-text="No hay datos disponibles" label="Tienda" required :loading="loadingStores" />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveUser" :disabled="!valid"
            :loading="saving">Guardar</v-btn>
          <v-btn color="red" dark class="mb-2" elevation="4" @click="close">Cancelar</v-btn>
        </v-card-actions>
      </v-col>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import { storeToRefs } from 'pinia';
import { useToast } from 'vue-toastification';
import { useUserStore } from '@/stores/userStore';
import { useRoleStore } from '@/stores/roleStore';
import { useStoreStore } from '@/stores/storeStore';
import { User } from '@/interfaces/userInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

interface Props {
  modelValue: boolean;
  user?: User | null;
}

const props = withDefaults(defineProps<Props>(), {
  user: () => ({
    idUser: null,
    userName: '',
    password: '',
    passwordHash: '',
    names: '',
    lastNames: '',
    identificationNumber: '',
    phoneNumber: null,
    idRole: null,
    roleName: '',
    idStore: null,
    storeName: '',
    auditCreateDate: '',
    statusUser: '',
    updatePassword: false
  })
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [user: User];
}>();

const userStore = useUserStore();
const roleStore = useRoleStore();
const storeStore = useStoreStore();
const toast = useToast();

const { roles, loading: loadingRoles } = storeToRefs(roleStore);
const { stores, loading: loadingStores } = storeToRefs(storeStore);

const formRef = ref<FormRef | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const localUser = ref<User>({ ...props.user } as User);
const oldPassword = ref('');

const rules = {
  required: (value: string) => !!value || 'Este campo es requerido.',
  onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.',
  onlyNumbers: (value: string) => !value || /^[0-9]+$/.test(value) || 'Solo se permiten números.',
};

const rolesArray = computed(() => Array.isArray(roles.value) ? roles.value : []);
const storesArray = computed(() => Array.isArray(stores.value) ? stores.value : []);

watch(() => props.modelValue, (newValue: boolean) => {
  isOpen.value = newValue;
  if (newValue) {
    roleStore.selectRole();
    storeStore.selectStore();
  }
});

watch(isOpen, (newValue: boolean) => {
  emit('update:modelValue', newValue);
});

watch(() => props.user, (newUser) => {
  if (newUser) {
    localUser.value = { ...newUser } as User;
    if (newUser.idUser) {
      oldPassword.value = newUser.passwordHash;
    } else {
      oldPassword.value = '';
    }
  }
}, { deep: true });

const uppercase = () => {
  localUser.value.userName = localUser.value.userName.toUpperCase();
};

const close = () => {
  isOpen.value = false;
};

const saveUser = async () => {
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
    const isEditing = !!localUser.value.idUser;
    let result;

    if (isEditing && localUser.value.idUser !== null) {
      if (localUser.value.passwordHash !== oldPassword.value) {
        localUser.value.updatePassword = true;
        localUser.value.password = localUser.value.passwordHash;
      } else {
        localUser.value.updatePassword = false;
        localUser.value.password = localUser.value.passwordHash;
      }
      result = await userStore.editUser(
        localUser.value.idUser,
        { ...localUser.value }
      );
    } else {
      localUser.value.password = localUser.value.passwordHash;
      result = await userStore.registerUser({ ...localUser.value });
    }

    if (result.isSuccess) {
      const successMsg = isEditing
        ? 'Usuario actualizado con éxito!'
        : 'Usuario registrado con éxito!';

      toast.success(successMsg);
      emit('saved', { ...localUser.value });
      close();
    }

  } catch (error: any) {
    const isEditing = !!localUser.value.idUser;
    const customMessage = isEditing
      ? 'Error en actualizar el usuario'
      : 'Error en guardar el usuario';

    handleApiError(error, customMessage);
  } finally {
    saving.value = false;
  }
};
</script>