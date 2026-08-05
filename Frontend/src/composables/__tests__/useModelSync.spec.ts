import { describe, it, expect, vi } from 'vitest';
import { useModelSync, useFiltersSync } from '../useModelSync';

describe('useModelSync', () => {

  it('get retorna el valor de la prop', () => {
    const props = { modelValue: true };
    const emit = vi.fn();

    const model = useModelSync<boolean>(props, emit, 'modelValue');

    expect(model.value).toBe(true);
  });

  it('set emite el evento update con el nuevo valor', () => {
    const props = { modelValue: false };
    const emit = vi.fn();

    const model = useModelSync<boolean>(props, emit, 'modelValue');
    model.value = true;

    expect(emit).toHaveBeenCalledWith('update:modelValue', true);
  });

  it('get retorna el valor actualizado cuando la prop cambia', () => {
    const props = { nombre: 'Juan' };
    const emit = vi.fn();

    const model = useModelSync<string>(props, emit, 'nombre');
    props.nombre = 'Pedro';

    expect(model.value).toBe('Pedro');
  });

  it('set emite el nombre de evento correcto para cualquier prop', () => {
    const props = { selectedFilter: 'Código' };
    const emit = vi.fn();

    const model = useModelSync<string>(props, emit, 'selectedFilter');
    model.value = 'Nombre';

    expect(emit).toHaveBeenCalledWith('update:selectedFilter', 'Nombre');
  });
});

describe('useFiltersSync', () => {

  const makeProps = (overrides = {}) => ({
    modelValue: false,
    selectedFilter: 'Código',
    state: 'Activos',
    startDate: null,
    endDate: null,
    filters: ['Código', 'Nombre', 'Email'],
    ...overrides
  });

  it('inicializa los modelos con los valores de las props', () => {
    const props = makeProps({ modelValue: true, state: 'Inactivos' });
    const emit = vi.fn();

    const { drawerModel, stateModel } = useFiltersSync(props, emit);

    expect(drawerModel.value).toBe(true);
    expect(stateModel.value).toBe('Inactivos');
  });

  it('clearFilters resetea selectedFilter al primer filtro de la lista', () => {
    const props = makeProps({ selectedFilter: 'Email' });
    const emit = vi.fn();

    const { clearFilters } = useFiltersSync(props, emit);
    clearFilters();

    expect(emit).toHaveBeenCalledWith('update:selectedFilter', 'Código');
  });

  it('clearFilters resetea state al valor por defecto Activos', () => {
    const props = makeProps({ state: 'Inactivos' });
    const emit = vi.fn();

    const { clearFilters } = useFiltersSync(props, emit);
    clearFilters();

    expect(emit).toHaveBeenCalledWith('update:state', 'Activos');
  });

  it('clearFilters resetea state al stateDefault personalizado', () => {
    const props = makeProps();
    const emit = vi.fn();

    const { clearFilters } = useFiltersSync(props, emit, 'Completado');
    clearFilters();

    expect(emit).toHaveBeenCalledWith('update:state', 'Completado');
  });

  it('clearFilters resetea startDate y endDate a null', () => {
    const props = makeProps({
      startDate: new Date(2025, 0, 1),
      endDate: new Date(2025, 11, 31),
    });
    const emit = vi.fn();

    const { clearFilters } = useFiltersSync(props, emit);
    clearFilters();

    expect(emit).toHaveBeenCalledWith('update:startDate', null);
    expect(emit).toHaveBeenCalledWith('update:endDate', null);
  });

  it('clearFilters no emite selectedFilter si filters está vacío', () => {
    const props = makeProps({ filters: [] });
    const emit = vi.fn();

    const { clearFilters } = useFiltersSync(props, emit);
    clearFilters();

    expect(emit).not.toHaveBeenCalledWith('update:selectedFilter', expect.anything());
  });
});