import axios, { AxiosRequestConfig } from 'axios';
import { FilterParams, BaseResponse, ServiceConfig } from '@/interfaces/baseInterface';

/**
 * Servicio base genérico para operaciones CRUD
 * @template T - Tipo de la entidad (Brand, Category, etc.)
 */

interface QueryParams {
  NumberPage: number;
  NumberRecordsPage: number;
  Order: string;
  Sort: string;
  StateFilter: number;
  TextFilter?: string;
  NumberFilter?: number;
  StartDate?: string;
  EndDate?: string;
  Download?: boolean;
  DownloadType?: 'pdf' | 'excel';
}

export class BaseService<T> {
  protected endpoint: string;
  protected downloadFileName: string;
  protected selectEndpoint: string;
  protected customEndpoints: ServiceConfig['customEndpoints'];

  constructor(config: ServiceConfig) {
    this.endpoint = config.endpoint;
    this.downloadFileName = config.downloadFileName;
    this.selectEndpoint = config.selectEndpoint || `${config.endpoint}/Select`;
    this.customEndpoints = config.customEndpoints || {};
  }

  private triggerDownload(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', filename);
    document.body.appendChild(link);
    link.click();
    link.parentNode?.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  // Construye los parámetros de consulta
  protected buildParams(params: FilterParams = {}): QueryParams {
    const queryParams: QueryParams = {
      NumberPage: params.pageNumber || 1,
      NumberRecordsPage: params.pageSize || 10,
      Order: params.order || 'desc',
      Sort: params.sort || 'Id',
      StateFilter: params.stateFilter ?? 1,
    };

    if (params.textFilter && params.numberFilter) {
      queryParams.TextFilter = params.textFilter;
      queryParams.NumberFilter = params.numberFilter;
    }
    if (params.startDate) queryParams.StartDate = params.startDate;
    if (params.endDate) queryParams.EndDate = params.endDate;

    return queryParams;
  }

  // Sobrecarga de tipos
  async fetchAll(params?: FilterParams, download?: false): Promise<BaseResponse<T[]>>;
  async fetchAll(params: FilterParams, download: true): Promise<Blob>;

  // Implementación (mantener como está)
  async fetchAll(
    params: FilterParams = {},
    download: boolean = false
  ): Promise<BaseResponse<T[]> | Blob> {
    const queryParams = {
      ...this.buildParams(params),
      Download: download,
    };

    const config: AxiosRequestConfig = {
      params: queryParams,
    };

    if (download) {
      config.responseType = 'blob';
      const response = await axios.get(`api/${this.endpoint}`, config);
      return response.data as Blob;
    }

    const response = await axios.get<BaseResponse<T[]>>(`api/${this.endpoint}`, config);
    return response.data;
  }

  // Descarga Excel de items
  async downloadExcel(params: FilterParams = {}): Promise<void> {
    const blob = await this.fetchAll(params, true);
    const date = new Date().toISOString().split('T')[0];
    this.triggerDownload(blob, `${this.downloadFileName}_${date}.xlsx`);
  }

  // Descarga PDF de items
  async downloadPdf(params: FilterParams = {}): Promise<void> {
    const queryParams = { ...this.buildParams(params), Download: true, DownloadType: 'pdf' };
    const response = await axios.get(`api/${this.endpoint}`, { params: queryParams, responseType: 'blob' });
    const blob = new Blob([response.data], { type: 'application/pdf' });
    const date = new Date().toISOString().split('T')[0];
    this.triggerDownload(blob, `${this.downloadFileName}_${date}.pdf`);
  }
  // Obtiene lista para selects (sin paginación)
  async select(): Promise<BaseResponse<T[]>> {
    const response = await axios.get<BaseResponse<T[]>>(`api/${this.selectEndpoint}`);
    return response.data;
  }

  // Obtiene un item por ID
  async fetchById(id: number): Promise<BaseResponse<T>> {
    const response = await axios.get<BaseResponse<T>>(`api/${this.endpoint}/${id}`);
    return response.data;
  }

  // Crea un nuevo item
  async create(data: T): Promise<BaseResponse<T>> {
    const endpoint = this.customEndpoints?.create || `${this.endpoint}/Register`;
    const response = await axios.post<BaseResponse<T>>(`api/${endpoint}`, data);
    return response.data;
  }

  // Actualiza un item existente
  async update(id: number, data: T): Promise<BaseResponse<T>> {
    const endpoint = this.customEndpoints?.update || `${this.endpoint}/Edit/${id}`;
    const response = await axios.put<BaseResponse<T>>(`api/${endpoint}`, data);
    return response.data;
  }

  // Habilita un item
  async enable(id: number): Promise<BaseResponse<void>> {
    const endpoint = this.customEndpoints?.enable || `${this.endpoint}/Enable/${id}`;
    const response = await axios.put<BaseResponse<void>>(`api/${endpoint}`, {});
    return response.data;
  }

  // Deshabilita un item
  async disable(id: number): Promise<BaseResponse<void>> {
    const endpoint = this.customEndpoints?.disable || `${this.endpoint}/Disable/${id}`;
    const response = await axios.put<BaseResponse<void>>(`api/${endpoint}`, {});
    return response.data;
  }

  // Elimina un item (soft delete)
  async remove(id: number): Promise<BaseResponse<void>> {
    const endpoint = this.customEndpoints?.remove || `${this.endpoint}/Remove/${id}`;
    const response = await axios.put<BaseResponse<void>>(`api/${endpoint}`, {});
    return response.data;
  }
}