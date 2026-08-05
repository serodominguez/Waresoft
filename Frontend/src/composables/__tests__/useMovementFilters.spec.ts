import { describe, it, expect, beforeEach } from 'vitest';
import { useMovementFilters } from '../useMovementFilters';

const filterMap = {
  'Código':    1,
  'Tienda':    2,
  'Proveedor': 3,
};

const stateMap = {
  'Todos':      0,
  'Completado': 1,
  'Anulado':    2,
};

describe('useMovementFilters', () => {

  it('inicializa con los valores por defecto', () => {
    const { selectedFilter, state, startDate, endDate } = useMovementFilters(
      'Código', filterMap, stateMap, 'Completado'
    );

    expect(selectedFilter.value).toBe('Código');
    expect(state.value).toBe('Completado');
    expect(startDate.value).toBeNull();
    expect(endDate.value).toBeNull();
  });

  it('getFilterParams retorna textFilter null cuando search es null', () => {
    const { getFilterParams } = useMovementFilters('Código', filterMap, stateMap, 'Completado');

    const params = getFilterParams(null);

    expect(params.textFilter).toBeNull();
  });

  it('getFilterParams retorna textFilter null cuando search es string vacío', () => {
    const { getFilterParams } = useMovementFilters('Código', filterMap, stateMap, 'Completado');

    const params = getFilterParams('   ');

    expect(params.textFilter).toBeNull();
  });

  it('getFilterParams retorna textFilter con el valor trimado', () => {
    const { getFilterParams } = useMovementFilters('Código', filterMap, stateMap, 'Completado');

    const params = getFilterParams('  ABC123  ');

    expect(params.textFilter).toBe('ABC123');
  });

  it('getFilterParams retorna el numberFilter correcto según el filtro seleccionado', () => {
    const { selectedFilter, getFilterParams } = useMovementFilters('Código', filterMap, stateMap, 'Completado');

    selectedFilter.value = 'Proveedor';
    const params = getFilterParams(null);

    expect(params.numberFilter).toBe(3);
  });

  it('getFilterParams retorna el stateFilter correcto según el estado', () => {
    const { state, getFilterParams } = useMovementFilters('Código', filterMap, stateMap, 'Completado');

    state.value = 'Anulado';
    const params = getFilterParams(null);

    expect(params.stateFilter).toBe(2);
  });

  it('getFilterParams retorna stateFilter de Todos cuando el estado no existe en el mapa', () => {
    const { state, getFilterParams } = useMovementFilters('Código', filterMap, stateMap, 'Completado');

    state.value = 'EstadoInexistente';
    const params = getFilterParams(null);

    expect(params.stateFilter).toBe(stateMap['Todos']);
  });

  it('getFilterParams formatea las fechas correctamente', () => {
    const { startDate, endDate, getFilterParams } = useMovementFilters('Código', filterMap, stateMap, 'Completado');

    startDate.value = new Date(2025, 0, 15); // 15 enero 2025
    endDate.value   = new Date(2025, 11, 31); // 31 diciembre 2025
    const params = getFilterParams(null);

    expect(params.startDate).toBe('2025-01-15');
    expect(params.endDate).toBe('2025-12-31');
  });

  it('getFilterParams retorna fechas null cuando no se establecen', () => {
    const { getFilterParams } = useMovementFilters('Código', filterMap, stateMap, 'Completado');

    const params = getFilterParams(null);

    expect(params.startDate).toBeNull();
    expect(params.endDate).toBeNull();
  });
});