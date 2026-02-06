
//Parámetros de filtrado comunes para todas las entidades
export interface FilterParams {
  pageNumber?: number;
  pageSize?: number;
  order?: string;
  sort?: string;
  textFilter?: string | null;
  numberFilter?: number | null;
  stateFilter?: number;
  startDate?: string | null;
  endDate?: string | null;
}

//Respuesta estándar del API
export interface BaseResponse<T = any> {
  isSuccess: boolean;
  data: T;
  totalRecords: number;
  message: string;
  errors: any;
}

//Estado base para módulos de Vuex
export interface BaseState<T> {
  items: T[];
  selectedItem: T | null;
  totalItems: number;
  loading: boolean;
  error: string | null;
  lastFilterParams?: FilterParams;
}

//Opciones de configuración para el servicio base
export interface ServiceConfig {
  endpoint: string;
  downloadFileName: string;
  selectEndpoint?: string; // Por si el endpoint de selección es diferente
  customEndpoints?: {
    create?: string;
    update?: string;
    enable?: string;
    disable?: string;
    remove?: string;
  };
}