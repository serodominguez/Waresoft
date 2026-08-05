import { describe, it, expect } from 'vitest';
import { formatDate, formatDateForApi } from '../date';

describe('formatDate', () => {

  it('retorna null cuando la fecha es null', () => {
    expect(formatDate(null)).toBeNull();
  });

  it('formatea una fecha al formato YYYY-MM-DD', () => {
    expect(formatDate(new Date(2025, 0, 15))).toBe('2025-01-15');
  });

  it('agrega cero a mes y día de un solo dígito', () => {
    expect(formatDate(new Date(2025, 2, 5))).toBe('2025-03-05');
  });

  it('formatea correctamente el último día del año', () => {
    expect(formatDate(new Date(2025, 11, 31))).toBe('2025-12-31');
  });

  it('formatea correctamente el primer día del año', () => {
    expect(formatDate(new Date(2025, 0, 1))).toBe('2025-01-01');
  });
});

describe('formatDateForApi', () => {

  it('retorna null cuando la fecha es null', () => {
    expect(formatDateForApi(null)).toBeNull();
  });

  it('retorna solo la parte YYYY-MM-DD de un string con hora', () => {
    expect(formatDateForApi('2025-06-15T10:30:00')).toBe('2025-06-15');
  });

  it('retorna el string tal cual si ya está en formato YYYY-MM-DD', () => {
    expect(formatDateForApi('2025-06-15')).toBe('2025-06-15');
  });

  it('retorna el string tal cual si no coincide con el formato esperado', () => {
    expect(formatDateForApi('15/06/2025')).toBe('15/06/2025');
  });

  it('retorna solo la fecha ignorando la zona horaria', () => {
    expect(formatDateForApi('2025-12-31T23:59:59.999Z')).toBe('2025-12-31');
  });
});