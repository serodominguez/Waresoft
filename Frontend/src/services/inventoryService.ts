import axios from 'axios';
import { BaseService } from './baseService';
import { Inventory, InventoryPivot } from '@/interfaces/inventoryInterface';
import { FilterParams, BaseResponse } from '@/interfaces/baseInterface';

class InventoryService extends BaseService<Inventory> {
  constructor() {
    super({
      endpoint: 'StoreInventory',
      downloadFileName: 'Inventario',
    });
  }

  // Sobrecargas de tipos
  async fetchAll(params?: FilterParams, download?: false): Promise<BaseResponse<Inventory[]>>;
  async fetchAll(params: FilterParams, download: true): Promise<Blob>;

  // Implementación
  async fetchAll(
    params: FilterParams = {},
    download: boolean = false
  ): Promise<BaseResponse<Inventory[]> | Blob> {
    const customParams = {
      ...params,
      sort: params.sort || 'IdProduct' // Por defecto ordena por IdProduct
    };
    return super.fetchAll(customParams, download as any);
  }

async fetchPivot(params: FilterParams = {}): Promise<BaseResponse<InventoryPivot>> {
  // Usa el mismo buildParams de la clase base
  const queryParams = this.buildParams({
    pageNumber: params.pageNumber || 1,
    pageSize: params.pageSize || 10,
    order: params.order || 'asc',
    sort: params.sort || 'IdProduct',
    stateFilter: params.stateFilter ?? 1,
    textFilter: params.textFilter,
    numberFilter: params.numberFilter,
    startDate: params.startDate,
    endDate: params.endDate
  });

  const response = await axios.get<BaseResponse<InventoryPivot>>(
    `api/${this.endpoint}/Pivot`,
    { params: queryParams }
  );
  return response.data;
}

  // Método específico para editar precio (solo envía el JSON sin ID en la URL)
  async updatePrice(inventory: Inventory): Promise<BaseResponse<Inventory>> {
    const response = await axios.put<BaseResponse<Inventory>>(
      `api/${this.endpoint}/Edit`,
      inventory
    );
    return response.data;
  }

  // Descarga planilla de inventario
  async inventorySheet(params: FilterParams = {}, storeName?: string): Promise<void> {
    try {
      const queryParams: any = {
        NumberPage: params.pageNumber || 1,
        NumberRecordsPage: params.pageSize || 10,
        Order: params.order || 'desc',
        Sort: params.sort || 'IdProduct',
        StateFilter: params.stateFilter ?? 1,
        Download: true,
        DownloadType: 'pdf'
      };

      if (params.textFilter && params.numberFilter) {
        queryParams.TextFilter = params.textFilter;
        queryParams.NumberFilter = params.numberFilter;
      }

      if (params.startDate) {
        queryParams.StartDate = params.startDate;
      }

      if (params.endDate) {
        queryParams.EndDate = params.endDate;
      }

      const response = await axios.get(`api/${this.endpoint}/ExportPdf`, {
        params: queryParams,
        responseType: 'blob'
      });

      const blob = new Blob([response.data], { type: 'application/pdf' });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;

      const date = new Date().toISOString().split('T')[0];
      const storePrefix = storeName ? `${storeName}_` : '';
      link.setAttribute('download', `${storePrefix}${this.downloadFileName}_${date}.pdf`);

      document.body.appendChild(link);
      link.click();

      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Error al descargar PDF:', error);
      throw error;
    }
  }
}

export const inventoryService = new InventoryService();