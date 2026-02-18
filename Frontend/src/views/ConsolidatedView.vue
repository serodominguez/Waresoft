<template>
  <div>
    <StockList
      :stores="stores"
      :rows="rows"
      :loading="loading"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useInventoryStore } from '@/stores/inventoryStore';
import { useRoute } from 'vue-router';
import StockList from '@/components/Inventory/StockList.vue';

const inventoryStore = useInventoryStore();
const route = useRoute();

const loading = computed(() => inventoryStore.loading);
const stores = computed(() => inventoryStore.inventoryPivot?.stores ?? []);
const rows = computed(() => inventoryStore.inventoryPivot?.rows ?? []);

onMounted(async () => {
  const productId = Number(route.params.productId);
  await inventoryStore.fetchInventoryPivot(productId);
});
</script>