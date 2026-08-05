import { describe, it, expect } from 'vitest';
import { formatCurrency } from '../currency';

describe('formatCurrency', () => {

  it('formatea un número entero sin decimales', () => {
    expect(formatCurrency(1000)).toBe('1,000');
  });

  it('formatea un número grande con separadores de miles', () => {
    expect(formatCurrency(1000000)).toBe('1,000,000');
  });

  it('formatea cero correctamente', () => {
    expect(formatCurrency(0)).toBe('0');
  });

  it('trunca los decimales sin redondear al alza incorrectamente', () => {
    expect(formatCurrency(1500.99)).toBe('1,501');
  });

  it('formatea números pequeños sin separador', () => {
    expect(formatCurrency(999)).toBe('999');
  });

  it('formatea números negativos', () => {
    expect(formatCurrency(-1500)).toBe('-1,500');
  });
});