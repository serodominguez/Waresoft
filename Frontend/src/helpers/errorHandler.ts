import { AxiosError } from 'axios';
import { useToast } from 'vue-toastification';
import {
  AppError,
  ErrorType,
  ApiErrorResponse,
  ErrorHandlerConfig,
  ERROR_MESSAGES,
  ERROR_TYPE_MESSAGES,
} from '@/interfaces/errorInterface';

// ─── Type Guards ────────────────────────────────────────────────────────────

interface TokenExpiredErrorShape {
  isTokenExpired: true;
  message: string;
}

function isTokenExpiredError(error: unknown): error is TokenExpiredErrorShape {
  return (
    typeof error === 'object' &&
    error !== null &&
    'isTokenExpired' in error &&
    (error as Record<string, unknown>).isTokenExpired === true
  );
}

function isAxiosError(error: unknown): error is AxiosError<ApiErrorResponse> {
  return (
    typeof error === 'object' &&
    error !== null &&
    'isAxiosError' in error &&
    (error as Record<string, unknown>).isAxiosError === true
  );
}

function isNetworkError(error: unknown): boolean {
  return (
    typeof error === 'object' &&
    error !== null &&
    'message' in error &&
    typeof (error as Record<string, unknown>).message === 'string' &&
    (error as Record<string, unknown>).message === 'Network Error'
  );
}

function isAppError(error: unknown): error is AppError {
  return (
    typeof error === 'object' &&
    error !== null &&
    'type' in error &&
    'message' in error &&
    typeof (error as Record<string, unknown>).message === 'string'
  );
}

// ─── Clase principal ─────────────────────────────────────────────────────────

export class ErrorHandler {
  private static toast = useToast();

  // Determina el tipo de error basado en el código de estado HTTP
  private static getErrorType(statusCode?: number): ErrorType {
    if (!statusCode) return ErrorType.NETWORK;

    if (statusCode === 401) return ErrorType.AUTHENTICATION;
    if (statusCode === 403) return ErrorType.AUTHORIZATION;
    if (statusCode === 404) return ErrorType.NOT_FOUND;
    if (statusCode >= 400 && statusCode < 500) return ErrorType.VALIDATION;
    if (statusCode >= 500) return ErrorType.SERVER;
    if (statusCode === 408 || statusCode === 504) return ErrorType.TIMEOUT;

    return ErrorType.UNKNOWN;
  }

  // Normaliza un AxiosError tipado a AppError
  private static normalizeAxiosError(
    error: AxiosError<ApiErrorResponse>
  ): AppError {
    const statusCode = error.response?.status;
    const type = this.getErrorType(statusCode);

    const apiResponse = error.response?.data;
    let message = '';

    if (apiResponse?.message) {
      message = apiResponse.message;
    } else if (apiResponse?.errors) {
      message = this.formatValidationErrors(apiResponse.errors);
    } else if (statusCode && ERROR_MESSAGES[statusCode]) {
      message = ERROR_MESSAGES[statusCode];
    } else {
      message = ERROR_TYPE_MESSAGES[type];
    }

    return {
      type,
      message,
      statusCode,
      details: apiResponse?.errors,
      originalError: error,
    };
  }

  // Formatea errores de validación del servidor
  private static formatValidationErrors(
    errors: string[] | Record<string, string[]>
  ): string {
    if (Array.isArray(errors)) {
      return errors.join(', ');
    }

    const messages: string[] = [];
    Object.entries(errors).forEach(([field, fieldErrors]) => {
      messages.push(`${field}: ${fieldErrors.join(', ')}`);
    });

    return messages.join('\n');
  }

  // Normaliza cualquier valor desconocido a AppError
  private static normalizeError(error: unknown): AppError {

    // Error de token expirado (lanzado desde el interceptor de Axios)
    if (isTokenExpiredError(error)) {
      return {
        type: ErrorType.TOKEN_EXPIRED,
        message: ERROR_TYPE_MESSAGES[ErrorType.TOKEN_EXPIRED],
        originalError: error,
      };
    }

    // Error de Axios (HTTP)
    if (isAxiosError(error)) {
      return this.normalizeAxiosError(error);
    }

    // Error de red sin respuesta del servidor
    if (isNetworkError(error)) {
      return {
        type: ErrorType.NETWORK,
        message: ERROR_TYPE_MESSAGES[ErrorType.NETWORK],
        originalError: error,
      };
    }

    // AppError ya normalizado previamente
    if (isAppError(error)) {
      return error;
    }

    // Error estándar de JavaScript
    if (error instanceof Error) {
      return {
        type: ErrorType.UNKNOWN,
        message: error.message,
        originalError: error,
      };
    }

    // Cualquier otro valor lanzado (string, número, objeto, etc.)
    return {
      type: ErrorType.UNKNOWN,
      message: ERROR_TYPE_MESSAGES[ErrorType.UNKNOWN],
      originalError: error,
    };
  }

  // Maneja un error de forma centralizada
  public static handle(
    error: unknown,
    config: ErrorHandlerConfig = {}
  ): AppError {
    const {
      showToast = true,
      logToConsole = process.env.NODE_ENV !== 'production',
      throwError = false,
      customMessage,
      onError,
    } = config;

    const appError = this.normalizeError(error);

    // Sobrescribir mensaje si se proporciona uno personalizado
    if (customMessage) {
      appError.message = customMessage;
    }

    // Loguear en consola solo en desarrollo
    if (logToConsole) {
      console.error('[ErrorHandler]', {
        type:          appError.type,
        message:       appError.message,
        statusCode:    appError.statusCode,
        details:       appError.details,
        originalError: appError.originalError,
      });
    }

    if (showToast) {
      this.showErrorToast(appError);
    }

    if (onError) {
      onError(appError);
    }

    if (throwError) {
      throw appError;
    }

    return appError;
  }

  // Muestra un toast según el tipo de error
  private static showErrorToast(error: AppError): void {
    const options = {
      timeout: this.getToastTimeout(error.type),
    };

    switch (error.type) {
      case ErrorType.TOKEN_EXPIRED:
      case ErrorType.AUTHENTICATION:
        this.toast.warning(error.message, options);
        break;

      case ErrorType.AUTHORIZATION:
        this.toast.warning(error.message, options);
        break;

      case ErrorType.VALIDATION:
        this.toast.error(error.message, { ...options, timeout: 5000 });
        break;

      case ErrorType.NETWORK:
        this.toast.error(error.message, { ...options, timeout: 3000 });
        break;

      default:
        this.toast.error(error.message, options);
    }
  }

  // Determina la duración del toast según el tipo de error
  private static getToastTimeout(type: ErrorType): number {
    switch (type) {
      case ErrorType.NETWORK:
        return 3000;
      case ErrorType.VALIDATION:
        return 5000;
      case ErrorType.TOKEN_EXPIRED:
      case ErrorType.AUTHENTICATION:
        return 4000;
      default:
        return 3000;
    }
  }

  // Helper para errores de API con mensaje personalizado opcional
  public static handleApiError(
    error: unknown,
    customMessage?: string
  ): AppError {
    return this.handle(error, {
      showToast:    true,
      logToConsole: true,
      customMessage,
    });
  }

  // Helper para errores silenciosos (sin toast, solo log)
  public static handleSilent(error: unknown): AppError {
    return this.handle(error, {
      showToast:    false,
      logToConsole: true,
    });
  }

  // Helper para errores críticos (muestra toast y relanza el error)
  public static handleCritical(error: unknown, customMessage?: string): never {
    this.handle(error, {
      showToast:    true,
      logToConsole: true,
      throwError:   true,
      customMessage,
    });
    throw error;
  }
}

// ─── Exports funcionales ─────────────────────────────────────────────────────

export const handleError = (
  error: unknown,
  config?: ErrorHandlerConfig
): AppError => ErrorHandler.handle(error, config);

export const handleApiError = (
  error: unknown,
  customMessage?: string
): AppError => ErrorHandler.handleApiError(error, customMessage);

export const handleSilentError = (error: unknown): AppError =>
  ErrorHandler.handleSilent(error);

export const handleCriticalError = (
  error: unknown,
  customMessage?: string
): never => ErrorHandler.handleCritical(error, customMessage);