// ── inventoryService.ts ──────────────────────────────────────────────────────
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

  async fetchPivot(params: FilterParams = {}): Promise<BaseResponse<InventoryPivot>> {
    const queryParams = {
      ...this.buildParams(params),
    };
    const response = await axios.get<BaseResponse<InventoryPivot>>(
      `api/${this.endpoint}/Pivot`,
      { params: queryParams }
    );
    return response.data;
  }

  async downloadPivotExcel(params: FilterParams = {}): Promise<void> {
    try {
      const queryParams = {
        ...this.buildParams(params),
        Download: true,
      };

      const response = await axios.get(`api/${this.endpoint}/Pivot`, {
        params: queryParams,
        responseType: 'blob',
      });

      const blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;

      const date = new Date().toISOString().split('T')[0];
      link.setAttribute('download', `ConsolidadoExistencias_${date}.xlsx`);

      document.body.appendChild(link);
      link.click();

      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Error al descargar Excel Pivot:', error);
      throw error;
    }
  }

  async updatePrice(inventory: Inventory): Promise<BaseResponse<Inventory>> {
    const response = await axios.put<BaseResponse<Inventory>>(
      `api/${this.endpoint}/Edit`,
      inventory
    );
    return response.data;
  }

  async inventorySheet(params: FilterParams = {}, storeName?: string): Promise<void> {
    try {
      const queryParams = {
        ...this.buildParams(params),
        Download: true,
        DownloadType: 'pdf',
      };

      const response = await axios.get(`api/${this.endpoint}/ExportPdf`, {
        params: queryParams,
        responseType: 'blob',
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