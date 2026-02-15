<template>
  <v-container fluid class="fill-height">
    <v-row justify="center">
      <v-col cols="12" sm="8" md="6" lg="5" xl="4">
        <v-card :loading="loading">
          <v-toolbar class="tool-bar-custom" dark>
            <v-toolbar-title>Inicio de sesión</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-text-field v-model="user" color="indigo" label="Usuario" variant="underlined" @keyup="uppercase"
              :disabled="loading" required></v-text-field>
            <v-text-field v-model="password" color="indigo" label="Contraseña" variant="underlined"
              :append-inner-icon="show ? 'visibility' : 'visibility_off'" :type="show ? 'text' : 'password'"
              @click:append-inner="show = !show" @keyup.enter="login()" :disabled="loading" required></v-text-field>
            <v-alert v-if="errorMessage" type="error">{{ errorMessage }}</v-alert>
          </v-card-text>
          <v-card-actions class="px-3 pb-3">
            <v-spacer></v-spacer>
            <v-btn @click="login" color="indigo" elevation="4" :loading="loading" :disabled="loading">
              Ingresar
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import axios, { AxiosError } from 'axios';
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

interface ApiResponse {
  isSuccess: boolean;
  data: string;
  message: string;
  errors: any;
}

interface ErrorResponse {
  message: string;
}

const router = useRouter();
const authStore = useAuthStore();

const show = ref(false);
const user = ref('');
const password = ref('');
const errorMessage = ref('');
const loading = ref(false);

const uppercase = () => {
  user.value = user.value.toUpperCase();
};

const validateFields = (): boolean => {
  errorMessage.value = '';

  errorMessage.value = '';

  if (!user.value.trim()) {
    errorMessage.value = 'El usuario es requerido';
    return false;
  }

  if (!password.value.trim()) {
    errorMessage.value = 'La contraseña es requerida';
    return false;
  }

  return true;
};

const login = async () => {
  // PASO 1: Validar campos
  if (!validateFields()) {
    return; // Si hay errores, se detiene aquí
  }

  // PASO 2: Activar loading
  loading.value = true; // ← Activa loading
  errorMessage.value = ''; // Limpia errores anteriores

  try {
    // PASO 3: Solicitar token al backend
    const response = await axios.post<ApiResponse>('api/Authorization/Generate/Token', {
      userName: user.value,
      password: password.value,
    });

    const token = response.data.data;

    // PASO 4: Verificar respuesta exitosa
    if (token && response.data.isSuccess) {
      // PASO 5: Guardar token Y cargar permisos (espera a que termine)
      await authStore.saveToken(token);

      // PASO 6: Todo listo, navegar al home
      router.push({ name: "home" });
      // NO desactiva loading aquí porque ya cambió de página

    } else {
      // Si no hay token válido
      errorMessage.value = response.data.message || 'Token no recibido. Inténtalo de nuevo.';
      loading.value = false; // ← Desactiva loading (hubo error)
    }
  } catch (error) {
    // PASO 7: Manejo de errores
    const axiosError = error as AxiosError<ErrorResponse>;
    if (axiosError.response?.data?.message) {
      errorMessage.value = axiosError.response.data.message;
    } else {
      errorMessage.value = 'Error de autenticación. Verifica tus credenciales.';
    }
    loading.value = false; // ← Desactiva loading (hubo error)
  }
};
</script>

<style scoped>
.tool-bar-custom {
  background-color: rgb(26, 32, 44);
  color: white;
}
</style>