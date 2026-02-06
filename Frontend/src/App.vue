<template>
  <v-app>
    <Appbar />
    <v-main class="ma-4">
      <router-view />
    </v-main>
    <Footerbar />
  </v-app>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import Appbar from './components/Dashboard/Appbar.vue';
import Footerbar from './components/Dashboard/Footerbar.vue';

export default defineComponent({
  name: 'App',
  components: {
    Appbar,
    Footerbar,
  },
});
</script>

