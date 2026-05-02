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
    params: FilterParams = {},
    signal?: AbortSignal
  ): Promise<BaseResponse<KardexDetail>> {
    const queryParams = {
      ...this.buildParams(params),
      productId,
    };

    const response = await axios.get<BaseResponse<KardexDetail>>(
      `api/${this.endpoint}/Kardex`,
      { params: queryParams, signal }
    );
    return response.data;
  }

  private buildDownloadLink(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', filename);
    document.body.appendChild(link);
    link.click();
    link.parentNode?.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  async downloadKardexExcel(productId: number, params: FilterParams = {}): Promise<void> {
    const queryParams = {
      ...this.buildParams(params),
      productId,
      Download: true,
    };

    const response = await axios.get(`api/${this.endpoint}/Kardex`, {
      params: queryParams,
      responseType: 'blob',
    });

    const date = new Date().toISOString().split('T')[0];
    const blob = new Blob([response.data], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    });
    this.buildDownloadLink(blob, `${this.downloadFileName}_${date}.xlsx`);
  }

  async downloadKardexPdf(productId: number, params: FilterParams = {}): Promise<void> {
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

    const date = new Date().toISOString().split('T')[0];
    const blob = new Blob([response.data], { type: 'application/pdf' });
    this.buildDownloadLink(blob, `${this.downloadFileName}_${date}.pdf`);
  }
}

export const kardexService = new KardexService();