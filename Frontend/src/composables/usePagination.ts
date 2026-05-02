import { ref } from 'vue';
import { FilterParams } from '@/interfaces/baseInterface';

export function usePagination(fetchFn: (params: Partial<FilterParams>) => void | Promise<void>) {
  const currentPage = ref(1);
  const itemsPerPage = ref(10);

  const updateItemsPerPage = (val: number) => {
    itemsPerPage.value = val;
    currentPage.value = 1;
    fetchFn({ pageNumber: 1, pageSize: val });
  };

  const changePage = (page: number) => {
    currentPage.value = page;
    fetchFn({ pageNumber: page, pageSize: itemsPerPage.value });
  };

  return { currentPage, itemsPerPage, updateItemsPerPage, changePage };
}