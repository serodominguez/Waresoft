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

// Clase principal para manejo centralizado de errores
export class ErrorHandler {
  private static toast = useToast();

  // Determina el tipo de error basado en el código de estado
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

  // Normaliza un error de Axios a AppError
  private static normalizeAxiosError(error: AxiosError): AppError {
    const statusCode = error.response?.status;
    const type = this.getErrorType(statusCode);
    
    // Intentar obtener mensaje del servidor
    const apiResponse = error.response?.data as ApiErrorResponse | undefined;
    let message = '';

    if (apiResponse?.message) {
      message = apiResponse.message;
    } else if (apiResponse?.errors) {
      // Manejar errores de validación
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

  // Formatea errores de validación
  private static formatValidationErrors(
    errors: string[] | Record<string, string[]>
  ): string {
    if (Array.isArray(errors)) {
      return errors.join(', ');
    }

    // Si es un objeto con campos
    const messages: string[] = [];
    Object.entries(errors).forEach(([field, fieldErrors]) => {
      messages.push(`${field}: ${fieldErrors.join(', ')}`);
    });

    return messages.join('\n');
  }

  // Normaliza cualquier tipo de error a AppError
  private static normalizeError(error: any): AppError {
    // NUEVO: Error de token expirado (del interceptor)
    if (error.isTokenExpired === true) {
      return {
        type: ErrorType.TOKEN_EXPIRED,
        message: ERROR_TYPE_MESSAGES[ErrorType.TOKEN_EXPIRED],
        originalError: error,
      };
    }

    // Error de Axios
    if (error.isAxiosError) {
      return this.normalizeAxiosError(error as AxiosError);
    }

    // Error de red
    if (error.message === 'Network Error') {
      return {
        type: ErrorType.NETWORK,
        message: ERROR_TYPE_MESSAGES[ErrorType.NETWORK],
        originalError: error,
      };
    }

    // AppError ya normalizado
    if (error.type && error.message) {
      return error as AppError;
    }

    // Error genérico de JavaScript
    return {
      type: ErrorType.UNKNOWN,
      message: error.message || ERROR_TYPE_MESSAGES[ErrorType.UNKNOWN],
      originalError: error,
    };
  }

  // Maneja un error de forma centralizada
  public static handle(
    error: any,
    config: ErrorHandlerConfig = {}
  ): AppError {
    const {
      showToast = true,
      logToConsole = process.env.NODE_ENV !== 'production', // Solo en desarrollo
      throwError = false,
      customMessage,
      onError,
    } = config;

    // Normalizar error
    const appError = this.normalizeError(error);

    // Usar mensaje personalizado si se proporciona
    if (customMessage) {
      appError.message = customMessage;
    }

    // Loggear en consola (solo en desarrollo)
    if (logToConsole) {
      console.error('[ErrorHandler]', {
        type: appError.type,
        message: appError.message,
        statusCode: appError.statusCode,
        details: appError.details,
        originalError: appError.originalError,
      });
    }

    // Mostrar toast
    if (showToast) {
      this.showErrorToast(appError);
    }

    // Callback personalizado
    if (onError) {
      onError(appError);
    }

    // Re-lanzar error si se solicita
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
      case ErrorType.TOKEN_EXPIRED: // NUEVO
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

  // Determina el timeout del toast según el tipo de error
  private static getToastTimeout(type: ErrorType): number {
    switch (type) {
      case ErrorType.NETWORK:
        return 3000;
      case ErrorType.VALIDATION:
        return 5000;
      case ErrorType.TOKEN_EXPIRED: // NUEVO
      case ErrorType.AUTHENTICATION:
        return 4000;
      default:
        return 3000;
    }
  }

  // Método helper para errores de API
  public static handleApiError(
    error: any,
    customMessage?: string
  ): AppError {
    return this.handle(error, {
      showToast: true,
      logToConsole: true,
      customMessage,
    });
  }

  // Método helper para errores silenciosos (sin toast)
  public static handleSilent(error: any): AppError {
    return this.handle(error, {
      showToast: false,
      logToConsole: true,
    });
  }

  // Método helper para errores críticos (con re-throw)
  public static handleCritical(error: any, customMessage?: string): never {
    this.handle(error, {
      showToast: true,
      logToConsole: true,
      throwError: true,
      customMessage,
    });
    throw error; // TypeScript safety
  }
}

// Exportar función helper global
export const handleError = (error: any, config?: ErrorHandlerConfig) =>
  ErrorHandler.handle(error, config);

export const handleApiError = (error: any, customMessage?: string) =>
  ErrorHandler.handleApiError(error, customMessage);

export const handleSilentError = (error: any) =>
  ErrorHandler.handleSilent(error);

export const handleCriticalError = (error: any, customMessage?: string) =>
  ErrorHandler.handleCritical(error, customMessage);