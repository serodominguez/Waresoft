import axios, { AxiosError, AxiosResponse, InternalAxiosRequestConfig } from 'axios';
import { jwtDecode } from 'jwt-decode';
import { useAuthStore } from '@/stores/auth';
import router from '@/router';
import { ErrorHandler } from '@/helpers/errorHandler';

// Interface para el payload del JWT
interface JwtPayload {
  exp: number;
  [key: string]: any;
}

// Función para verificar si el token expiró
const isTokenExpired = (token: string): boolean => {
  try {
    const decoded = jwtDecode<JwtPayload>(token);
    return decoded.exp < Date.now() / 1000;
  } catch {
    return true;
  }
};

export function setupAxiosInterceptors() {
  // Request Interceptor
  axios.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
      // Obtener token del localStorage
      const token = localStorage.getItem("token");

      if (token) {
        // Verificar si el token expiró antes de enviar la petición
        if (isTokenExpired(token)) {
          // Cancelar la petición actual con error tipado
          return Promise.reject({
            message: "Token expirado",
            isTokenExpired: true,
          });
        }

        // Token válido → agregarlo a los headers
        if (config.headers) {
          config.headers.Authorization = `Bearer ${token}`;
        }
      }

      return config;
    },
    (error: AxiosError) => {
      return Promise.reject(error);
    }
  );

  // Response Interceptor
  axios.interceptors.response.use(
    (response: AxiosResponse) => {
      return response;
    },
    async (error: AxiosError | any) => {
      // Token expirado desde el request interceptor
      if (error.isTokenExpired) {
        const currentRoute = router.currentRoute.value.name;

        // Evitar loops infinitos
        if (currentRoute !== "login") {
          // Mostrar toast cuando expire el token
          ErrorHandler.handle(error, {
            showToast: true,
            customMessage: "Tu sesión ha expirado. Por favor, inicia sesión nuevamente",
          });
          
          // Limpiar sesión y redirigir
          const authStore = useAuthStore();
          await authStore.logout();
        }
        return Promise.reject(error);
      }

      const statusCode = error.response?.status;

      // 401 - Token expirado o inválido (respuesta del servidor)
      if (statusCode === 401) {
        const currentRoute = router.currentRoute.value.name;

        // Evitar loops infinitos
        if (currentRoute !== "login") {
          ErrorHandler.handle(error, {
            showToast: true,
            customMessage: "Tu sesión ha expirado. Por favor, inicia sesión nuevamente",
          });

          // Limpiar sesión y redirigir
          const authStore = useAuthStore();
          await authStore.logout();
        }
        return Promise.reject(error);
      }

      // 403 - Sin permisos
      if (statusCode === 403) {
        ErrorHandler.handle(error, {
          showToast: true,
          customMessage: "No tienes permisos para realizar esta acción",
        });
        return Promise.reject(error);
      }

      // 404 - Recurso no encontrado
      if (statusCode === 404) {
        ErrorHandler.handleSilent(error);
        return Promise.reject(error);
      }

      // 500+ - Errores del servidor
      if (statusCode && statusCode >= 500) {
        ErrorHandler.handle(error, {
          showToast: true,
          customMessage: "Error del servidor. Intenta nuevamente en unos momentos",
        });
        return Promise.reject(error);
      }

      // Errores de red
      if (error.message === "Network Error" || !error.response) {
        ErrorHandler.handle(error, {
          showToast: true,
          customMessage: "Sin conexión a internet. Verifica tu conexión",
        });
        return Promise.reject(error);
      }

      // Timeout
      if (error.code === "ECONNABORTED") {
        ErrorHandler.handle(error, {
          showToast: true,
          customMessage: "La solicitud tardó demasiado. Intenta nuevamente",
        });
        return Promise.reject(error);
      }

      // Otros errores - dejar que el componente decida
      return Promise.reject(error);
    }
  );
}

// Configuración de timeout global
export function configureAxiosDefaults() {
  // Base URL
  axios.defaults.baseURL = process.env.VUE_APP_API_URL || 'https://localhost:7145/';

  // Timeout (30 segundos)
  axios.defaults.timeout = 30000;

  // Headers por defecto
  axios.defaults.headers.common['Content-Type'] = 'application/json';
  axios.defaults.headers.common['Accept'] = 'application/json';

  // Configurar para enviar cookies
  axios.defaults.withCredentials = false;
}