import { describe, it, expect } from 'vitest';
import { normalize, titleCase } from '../string';

describe('normalize', () => {

  it('convierte a minúsculas', () => {
    expect(normalize('HOLA')).toBe('hola');
  });

  it('elimina tildes', () => {
    expect(normalize('café')).toBe('cafe');
  });

  it('elimina tildes y convierte a minúsculas', () => {
    expect(normalize('Ácido Útil Índice')).toBe('acido util indice');
  });

  it('maneja la ñ correctamente', () => {
    expect(normalize('niño')).toBe('nino');
  });

  it('retorna string vacío si se pasa string vacío', () => {
    expect(normalize('')).toBe('');
  });

  it('no altera texto sin tildes ni mayúsculas', () => {
    expect(normalize('hola mundo')).toBe('hola mundo');
  });
});

describe('titleCase', () => {

  it('capitaliza la primera letra de cada palabra', () => {
    expect(titleCase('hola mundo')).toBe('Hola Mundo');
  });

  it('convierte texto en mayúsculas a title case', () => {
    expect(titleCase('JUAN PEREZ')).toBe('Juan Perez');
  });

  it('retorna string vacío si se pasa string vacío', () => {
    expect(titleCase('')).toBe('');
  });

  it('maneja una sola palabra', () => {
    expect(titleCase('sergio')).toBe('Sergio');
  });

  it('maneja múltiples espacios entre palabras', () => {
    expect(titleCase('juan  perez')).toBe('Juan  Perez');
  });

  it('retorna string vacío si se pasa null o undefined', () => {
    expect(titleCase(null as any)).toBe('');
    expect(titleCase(undefined as any)).toBe('');
  });
});