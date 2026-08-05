import { describe, it, expect, vi, beforeEach } from 'vitest';

// ✅ vi.hoisted garantiza que mockToast se inicialice ANTES del hoisting de vi.mock
const mockToast = vi.hoisted(() => ({
  error:   vi.fn(),
  warning: vi.fn(),
  success: vi.fn(),
}));

vi.mock('vue-toastification', () => ({
  useToast: () => mockToast,
}));

vi.stubGlobal('import', { meta: { env: { DEV: false } } });

import { ErrorHandler, handleApiError, handleSilentError } from '../errorHandler';
import { ErrorType } from '@/interfaces/errorInterface';

const makeAxiosError = (status: number, message?: string, errors?: any) => ({
  isAxiosError: true,
  response: {
    status,
    data: {
      isSuccess: false,
      message: message ?? '',
      errors,
    },
  },
  message: 'Request failed',
});

beforeEach(() => {
  vi.clearAllMocks();
});

describe('ErrorHandler.handle', () => {

  // ── Tipos de error ────────────────────────────────────────────────────────
  it('normaliza un Error estándar como UNKNOWN', () => {
    const result = ErrorHandler.handle(new Error('algo salió mal'), { showToast: false });

    expect(result.type).toBe(ErrorType.UNKNOWN);
    expect(result.message).toBe('algo salió mal');
  });

  it('normaliza un error de red como NETWORK', () => {
    const result = ErrorHandler.handle({ message: 'Network Error' }, { showToast: false });

    expect(result.type).toBe(ErrorType.NETWORK);
  });

  it('normaliza un token expirado como TOKEN_EXPIRED', () => {
    const result = ErrorHandler.handle(
      { isTokenExpired: true, message: 'Token expirado' },
      { showToast: false }
    );

    expect(result.type).toBe(ErrorType.TOKEN_EXPIRED);
  });

  it('normaliza un 401 como AUTHENTICATION', () => {
    const result = ErrorHandler.handle(makeAxiosError(401), { showToast: false });

    expect(result.type).toBe(ErrorType.AUTHENTICATION);
    expect(result.statusCode).toBe(401);
  });

  it('normaliza un 403 como AUTHORIZATION', () => {
    const result = ErrorHandler.handle(makeAxiosError(403), { showToast: false });

    expect(result.type).toBe(ErrorType.AUTHORIZATION);
  });

  it('normaliza un 404 como NOT_FOUND', () => {
    const result = ErrorHandler.handle(makeAxiosError(404), { showToast: false });

    expect(result.type).toBe(ErrorType.NOT_FOUND);
  });

  it('normaliza un 400 como VALIDATION', () => {
    const result = ErrorHandler.handle(makeAxiosError(400), { showToast: false });

    expect(result.type).toBe(ErrorType.VALIDATION);
  });

  it('normaliza un 500 como SERVER', () => {
    const result = ErrorHandler.handle(makeAxiosError(500), { showToast: false });

    expect(result.type).toBe(ErrorType.SERVER);
  });

  it('normaliza un valor desconocido como UNKNOWN', () => {
    const result = ErrorHandler.handle('error raro', { showToast: false });

    expect(result.type).toBe(ErrorType.UNKNOWN);
  });

  // ── Mensaje ───────────────────────────────────────────────────────────────
  it('usa el mensaje de la API si está disponible', () => {
    const result = ErrorHandler.handle(
      makeAxiosError(400, 'Campo requerido'),
      { showToast: false }
    );

    expect(result.message).toBe('Campo requerido');
  });

  it('formatea errores de validación como array', () => {
    const result = ErrorHandler.handle(
      makeAxiosError(400, '', ['Error A', 'Error B']),
      { showToast: false }
    );

    expect(result.message).toBe('Error A, Error B');
  });

  it('formatea errores de validación como objeto', () => {
    const result = ErrorHandler.handle(
      makeAxiosError(400, '', { nombre: ['requerido'], email: ['inválido'] }),
      { showToast: false }
    );

    expect(result.message).toContain('nombre: requerido');
    expect(result.message).toContain('email: inválido');
  });

  it('usa el ERROR_MESSAGES del status si no hay mensaje en la API', () => {
    const result = ErrorHandler.handle(makeAxiosError(404, ''), { showToast: false });

    expect(result.message).toBe('El recurso solicitado no fue encontrado');
  });

  it('sobrescribe el mensaje con customMessage', () => {
    const result = ErrorHandler.handle(
      new Error('original'),
      { showToast: false, customMessage: 'Mensaje personalizado' }
    );

    expect(result.message).toBe('Mensaje personalizado');
  });

  // ── Toast ─────────────────────────────────────────────────────────────────
  it('muestra toast de error por defecto', () => {
    ErrorHandler.handle(new Error('error'));

    expect(mockToast.error).toHaveBeenCalled();
  });

  it('muestra toast de warning para errores de autenticación', () => {
    ErrorHandler.handle(makeAxiosError(401));

    expect(mockToast.warning).toHaveBeenCalled();
  });

  it('muestra toast de warning para errores de autorización', () => {
    ErrorHandler.handle(makeAxiosError(403));

    expect(mockToast.warning).toHaveBeenCalled();
  });

  it('no muestra toast cuando showToast es false', () => {
    ErrorHandler.handle(new Error('error'), { showToast: false });

    expect(mockToast.error).not.toHaveBeenCalled();
    expect(mockToast.warning).not.toHaveBeenCalled();
  });

  // ── Callbacks ─────────────────────────────────────────────────────────────
  it('llama onError con el AppError normalizado', () => {
    const onError = vi.fn();
    ErrorHandler.handle(new Error('error'), { showToast: false, onError });

    expect(onError).toHaveBeenCalledWith(expect.objectContaining({
      type: ErrorType.UNKNOWN,
      message: 'error',
    }));
  });

  it('throwError relanza el error después de manejarlo', () => {
    expect(() =>
      ErrorHandler.handle(new Error('crítico'), { showToast: false, throwError: true })
    ).toThrow();
  });
});

describe('ErrorHandler.handleSilent', () => {

  it('no muestra toast', () => {
    ErrorHandler.handleSilent(new Error('silencioso'));

    expect(mockToast.error).not.toHaveBeenCalled();
  });

  it('retorna AppError normalizado', () => {
    const result = ErrorHandler.handleSilent(new Error('silencioso'));

    expect(result.type).toBe(ErrorType.UNKNOWN);
    expect(result.message).toBe('silencioso');
  });
});

describe('ErrorHandler.handleCritical', () => {

  it('relanza el error siempre', () => {
    expect(() =>
      ErrorHandler.handleCritical(new Error('crítico'))
    ).toThrow();
  });

  it('muestra toast antes de relanzar', () => {
    try {
      ErrorHandler.handleCritical(new Error('crítico'));
    } catch {}

    expect(mockToast.error).toHaveBeenCalled();
  });
});

describe('exports funcionales', () => {

  it('handleApiError retorna AppError', () => {
    const result = handleApiError(new Error('error api'));

    expect(result).toHaveProperty('type');
    expect(result).toHaveProperty('message');
  });

  it('handleSilentError no muestra toast', () => {
    handleSilentError(new Error('silencioso'));

    expect(mockToast.error).not.toHaveBeenCalled();
  });
});