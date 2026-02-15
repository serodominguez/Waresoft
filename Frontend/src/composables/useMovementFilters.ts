import { ref } from 'vue';
import { formatDate } from '@/utils/date';

type StateMap = Record<string, number>;

export function useMovementFilters(
  defaultFilter: string, 
  filterMap: Record<string, number>,
  stateMap: StateMap,
  defaultState: string = 'Todos'
) {
  const selectedFilter = ref(defaultFilter);
  const state = ref(defaultState);
  const startDate = ref<Date | null>(null);
  const endDate = ref<Date | null>(null);
  
  const getFilterParams = (search: string | null) => {
    // Obtener el valor del estado según el mapa proporcionado
    const stateFilterValue = stateMap[state.value] ?? stateMap['Todos'];
    
    return {
      textFilter: search?.trim() || null,
      numberFilter: filterMap[selectedFilter.value],
      stateFilter: stateFilterValue,
      startDate: formatDate(startDate.value),
      endDate: formatDate(endDate.value)
    };
  };
  
  return { selectedFilter, state, startDate, endDate, getFilterParams };
}