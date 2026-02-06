import { computed, WritableComputedRef } from 'vue';

/**
 * Composable para sincronizar props con v-model
 * Simplifica la creación de computed properties bidireccionales
 * 
 * @param props - Props del componente
 * @param emit - Función emit del componente
 * @param propName - Nombre de la prop a sincronizar
 * @returns Computed property bidireccional
 */
export function useModelSync<T>(
  props: any,
  emit: any,
  propName: string
): WritableComputedRef<T> {
  return computed({
    get: () => props[propName],
    set: (value: T) => {
      const eventName = `update:${propName}`;
      emit(eventName, value);
    }
  });
}

/**
 * Composable específico para filtros
 * Agrupa toda la lógica de sincronización de filtros
 */
export function useFiltersSync(props: any, emit: any) {
  const drawerModel = useModelSync<boolean>(props, emit, 'modelValue');
  const selectedFilterModel = useModelSync<string>(props, emit, 'selectedFilter');
  const stateModel = useModelSync<string>(props, emit, 'state');
  const startDateModel = useModelSync<Date | null>(props, emit, 'startDate');
  const endDateModel = useModelSync<Date | null>(props, emit, 'endDate');

  const clearFilters = () => {
    if (props.filters.length > 0) {
      selectedFilterModel.value = props.filters[0];
    }
    stateModel.value = 'Activos';
    startDateModel.value = null;
    endDateModel.value = null;
  };

  return {
    drawerModel,
    selectedFilterModel,
    stateModel,
    startDateModel,
    endDateModel,
    clearFilters
  };
}