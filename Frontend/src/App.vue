<template>
  <v-app>
    <Appbar />
    <v-main class="ma-4">
      <router-view />
    </v-main>
    <Footerbar />
  </v-app>
</template>

<script setup lang="ts">
import Appbar from './components/Dashboard/Appbar.vue'
import Footerbar from './components/Dashboard/Footerbar.vue'
</script>
