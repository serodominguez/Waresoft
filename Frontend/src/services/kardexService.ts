import axios from 'axios';
import { BaseService } from './baseService';
import { KardexDetail } from '@/interfaces/kardexInterface';
import { FilterParams, BaseResponse } from '@/interfaces/baseInterface';

class KardexService extends BaseService<KardexDetail> {
  constructor() {
    super({
      endpoint: 'StoreInventory',
      downloadFileName: 'Kardex',
    });
  }

  async fetchKardex(
    productId: number,
    params: FilterParams = {}
  ): Promise<BaseResponse<KardexDetail>> {
    const queryParams = {
      ...this.buildParams(params),
      productId,
    };

    const response = await axios.get<BaseResponse<KardexDetail>>(
      `api/${this.endpoint}/Kardex`,
      { params: queryParams }
    );
    return response.data;
  }

  async downloadKardexExcel(productId: number, params: FilterParams = {}): Promise<void> {
    try {
      const queryParams = {
        ...this.buildParams(params),
        productId,
        Download: true,
      };

      const response = await axios.get(`api/${this.endpoint}/Kardex`, {
        params: queryParams,
        responseType: 'blob',
      });

      const blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;

      const date = new Date().toISOString().split('T')[0];
      link.setAttribute('download', `${this.downloadFileName}_${date}.xlsx`);

      document.body.appendChild(link);
      link.click();

      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  }

  async downloadKardexPdf(productId: number, params: FilterParams = {}): Promise<void> {
    try {
      const queryParams = {
        ...this.buildParams(params),
        productId,
        Download: true,
        DownloadType: 'pdf',
      };

      const response = await axios.get(`api/${this.endpoint}/Kardex`, {
        params: queryParams,
        responseType: 'blob',
      });

      const blob = new Blob([response.data], { type: 'application/pdf' });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;

      const date = new Date().toISOString().split('T')[0];
      link.setAttribute('download', `${this.downloadFileName}_${date}.pdf`);

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

export const kardexService = new KardexService();