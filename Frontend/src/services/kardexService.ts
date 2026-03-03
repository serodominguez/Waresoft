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

  // Sobrecargas de tipos
  async fetchKardex(productId: number, params?: FilterParams, download?: false): Promise<BaseResponse<KardexDetail>>;
  async fetchKardex(productId: number, params: FilterParams, download: true): Promise<Blob>;

  // Implementación
  async fetchKardex(
    productId: number,
    params: FilterParams = {},
    download: boolean = false
  ): Promise<BaseResponse<KardexDetail> | Blob> {
    const queryParams = this.buildParams({
      pageNumber: params.pageNumber || 1,
      pageSize: params.pageSize || 10,
      order: params.order || 'asc',
      sort: params.sort || 'Id',
      stateFilter: params.stateFilter,
      textFilter: params.textFilter,
      numberFilter: params.numberFilter,
      startDate: params.startDate,
      endDate: params.endDate,
    });

    // Agrega el productId como parámetro adicional
    queryParams.productId = productId;

    if (download) {
      const response = await axios.get(`api/${this.endpoint}/Kardex`, {
        params: queryParams,
        responseType: 'blob',
      });
      return response.data as Blob;
    }

    const response = await axios.get<BaseResponse<KardexDetail>>(
      `api/${this.endpoint}/Kardex`,
      { params: queryParams }
    );
    return response.data;
  }
}

export const kardexService = new KardexService();