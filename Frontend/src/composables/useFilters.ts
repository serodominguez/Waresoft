import { ref } from 'vue'; 
import { formatDate } from '@/utils/date';

export function useFilters(defaultFilter: string, filterMap: Record<string, number>) {
  const selectedFilter = ref(defaultFilter);
  const state = ref('Activos');
  const startDate = ref<Date | null>(null);
  const endDate = ref<Date | null>(null);
  
  const getFilterParams = (search: string | null) => ({
    textFilter: search?.trim() || null,
    numberFilter: filterMap[selectedFilter.value],
    stateFilter: state.value === 'Activos' ? 1 : 0,
    startDate: formatDate(startDate.value),
    endDate: formatDate(endDate.value)
  });
  
  return { selectedFilter, state, startDate, endDate, getFilterParams };
}