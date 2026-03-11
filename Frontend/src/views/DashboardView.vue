<template>
  <Dashboard />
</template>

<script lang="ts">
import { defineComponent } from 'vue';

// Components
import Dashboard from '../components/Dashboard/Dashboard.vue';

export default defineComponent({
  name: 'DashboardView',

  components: {
    Dashboard,
  },
});
</script>
