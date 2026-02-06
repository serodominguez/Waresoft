import { AxiosError } from 'axios';


export enum ErrorType {
  NETWORK = 'NETWORK',           // Sin conexión
  AUTHENTICATION = 'AUTH',       // 401
  AUTHORIZATION = 'FORBIDDEN',   // 403
  NOT_FOUND = 'NOT_FOUND',      // 404
  VALIDATION = 'VALIDATION',     // 400
  SERVER = 'SERVER',             // 500
  TIMEOUT = 'TIMEOUT',           // Timeout
  TOKEN_EXPIRED = 'TOKEN_EXPIRED', // Token
  UNKNOWN = 'UNKNOWN',           // Otros
}


export interface AppError {
  type: ErrorType;
  message: string;
  statusCode?: number;
  details?: any;
  originalError?: any;
}

export interface TokenExpiredError {
  message: string;
  isTokenExpired: true;
}

export interface ApiErrorResponse {
  isSuccess: false;
  message: string;
  errors?: string[] | Record<string, string[]>;
  statusCode?: number;
}

// Configuración de manejo de errores
export interface ErrorHandlerConfig {
  showToast?: boolean;           // Mostrar toast automáticamente
  logToConsole?: boolean;         // Loggear en consola
  throwError?: boolean;           // Re-lanzar el error después de manejar
  customMessage?: string;         // Mensaje personalizado
  onError?: (error: AppError) => void; // Callback personalizado
}

// Mapeo de códigos HTTP a mensajes amigables
export const ERROR_MESSAGES: Record<number, string> = {
  400: 'Los datos enviados no son válidos',
  401: 'Tu sesión ha expirado. Por favor, inicia sesión nuevamente',
  403: 'No tienes permisos para realizar esta acción',
  404: 'El recurso solicitado no fue encontrado',
  408: 'La solicitud tardó demasiado tiempo',
  409: 'El recurso ya existe o hay un conflicto',
  422: 'Los datos no pudieron ser procesados',
  429: 'Demasiadas solicitudes. Intenta más tarde',
  500: 'Error interno del servidor',
  502: 'El servidor no está disponible temporalmente',
  503: 'Servicio no disponible. Intenta más tarde',
  504: 'El servidor tardó demasiado en responder',
};

// Mensajes por tipo de error
export const ERROR_TYPE_MESSAGES: Record<ErrorType, string> = {
  [ErrorType.NETWORK]: 'No hay conexión a internet',
  [ErrorType.AUTHENTICATION]: 'Tu sesión ha expirado',
  [ErrorType.AUTHORIZATION]: 'No tienes permisos para esta acción',
  [ErrorType.NOT_FOUND]: 'Recurso no encontrado',
  [ErrorType.VALIDATION]: 'Los datos ingresados no son válidos',
  [ErrorType.SERVER]: 'Error del servidor',
  [ErrorType.TIMEOUT]: 'La solicitud tardó demasiado',
  [ErrorType.TOKEN_EXPIRED]: 'Tu sesión ha expirado. Por favor, inicia sesión nuevamente',
  [ErrorType.UNKNOWN]: 'Ha ocurrido un error inesperado',
};