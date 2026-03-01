<template>
  <nav>
    <v-app-bar class="app-bar-custom" dark app>
      <v-app-bar-nav-icon v-if="currentUser" @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title v-if="currentUser">
        <span class="font-weight-light"></span>
        <span style="font-size: 70%"><strong>{{ currentUser.storeType+": "+currentUser.storeName }} </strong></span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <span v-if="currentUser" style="font-size: 90%; margin-right: 10px;"><strong> Usuario: {{ currentUser.userName}}</strong></span>
      <v-btn v-if="currentUser" @click="logout" icon="logout"></v-btn>
    </v-app-bar>
  </nav>
  <NavigationDrawer v-model="drawer" />
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useAuthStore } from '@/stores/auth';
import NavigationDrawer from './NavigationDrawer.vue';

// Inicialización del store
const authStore = useAuthStore();

// Estado reactivo
const drawer = ref(false);

// Computed property para el usuario actual
const currentUser = computed(() => authStore.currentUser);

// Método para cerrar sesión
const logout = (): void => {
  authStore.logout();
};
</script>
<style scoped>
.app-bar-custom {
  background-color: rgb(26, 32, 44);
  color: white; 
}
</style>