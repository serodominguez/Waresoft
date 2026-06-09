import { defineStore } from 'pinia';
import { ref } from 'vue';
import { createBaseStore } from '@/stores/baseStore';
import { Customer, CustomerStats } from '@/interfaces/customerInterface';
import { customerService } from '@/services/customerService';

export const useCustomerStore = createBaseStore<Customer>('customer', customerService);

export const useCustomerStatsStore = defineStore('customerStats', () => {
  const stats = ref<CustomerStats | null>(null);
  const loading = ref<boolean>(false);

  async function fetchStats() {
    loading.value = true;
    try {
      stats.value = await customerService.getStats();
    } finally {
      loading.value = false;
    }
  }

  return {
    stats,
    loading,
    fetchStats,
  };
});