import { describe, it, expect } from 'vitest';
import { useFilters } from '../useFilters';

const filterMap = {
  'Nombre': 1,
  'Código': 2,
  'Email': 3,
};

describe('useFilters', () => {

  it('inicializa con los valores por defecto', () => {
    const { selectedFilter, state, startDate, endDate } = useFilters('Nombre', filterMap);

    expect(selectedFilter.value).toBe('Nombre');
    expect(state.value).toBe('Activos');
    expect(startDate.value).toBeNull();
    expect(endDate.value).toBeNull();
  });

  it('getFilterParams retorna stateFilter 1 cuando state es Activos', () => {
    const { getFilterParams } = useFilters('Nombre', filterMap);

    const params = getFilterParams(null);

    expect(params.stateFilter).toBe(1);
  });

  it('getFilterParams retorna stateFilter 0 cuando state no es Activos', () => {
    const { state, getFilterParams } = useFilters('Nombre', filterMap);

    state.value = 'Inactivos';
    const params = getFilterParams(null);

    expect(params.stateFilter).toBe(0);
  });

  it('getFilterParams retorna textFilter null cuando search es null', () => {
    const { getFilterParams } = useFilters('Nombre', filterMap);

    const params = getFilterParams(null);

    expect(params.textFilter).toBeNull();
  });

  it('getFilterParams retorna textFilter null cuando search es string vacío', () => {
    const { getFilterParams } = useFilters('Nombre', filterMap);

    const params = getFilterParams('   ');

    expect(params.textFilter).toBeNull();
  });

  it('getFilterParams retorna textFilter con el valor trimado', () => {
    const { getFilterParams } = useFilters('Nombre', filterMap);

    const params = getFilterParams('  Juan  ');

    expect(params.textFilter).toBe('Juan');
  });

  it('getFilterParams retorna el numberFilter correcto según el filtro seleccionado', () => {
    const { selectedFilter, getFilterParams } = useFilters('Nombre', filterMap);

    selectedFilter.value = 'Email';
    const params = getFilterParams(null);

    expect(params.numberFilter).toBe(3);
  });

  it('getFilterParams formatea las fechas correctamente', () => {
    const { startDate, endDate, getFilterParams } = useFilters('Nombre', filterMap);

    startDate.value = new Date(2025, 2, 5);  // 5 marzo 2025
    endDate.value = new Date(2025, 5, 20); // 20 junio 2025
    const params = getFilterParams(null);

    expect(params.startDate).toBe('2025-03-05');
    expect(params.endDate).toBe('2025-06-20');
  });

  it('getFilterParams retorna fechas null cuando no se establecen', () => {
    const { getFilterParams } = useFilters('Nombre', filterMap);

    const params = getFilterParams(null);

    expect(params.startDate).toBeNull();
    expect(params.endDate).toBeNull();
  });
});