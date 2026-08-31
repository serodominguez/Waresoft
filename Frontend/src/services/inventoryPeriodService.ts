import axios from 'axios';
import { BaseService } from './baseService';
import {
  InventoryPeriod,
  InventoryPeriodDetail,
  InventoryPeriodOpeningResponse,
  InventoryPeriodClosingResponse,
  InventoryPeriodOpenRequest,
  InventoryPeriodCloseRequest,
} from '@/interfaces/inventoryPeriodInterface';
import { FilterParams, BaseResponse } from '@/interfaces/baseInterface';

class InventoryPeriodService extends BaseService<InventoryPeriod> {
  constructor() {
    super({
      endpoint: 'InventoryPeriod',
      downloadFileName: 'PeriodosInventario',
    });
  }

  // GET api/InventoryPeriod — storeId viene del token en el backend
  async fetchPeriods(params: FilterParams = {}): Promise<BaseResponse<InventoryPeriod[]>> {
    const queryParams = this.buildParams(params);
    const response = await axios.get<BaseResponse<InventoryPeriod[]>>(
      `api/${this.endpoint}`,
      { params: queryParams }
    );
    return response.data;
  }

  // GET api/InventoryPeriod/{periodId}
  async fetchDetail(periodId: number): Promise<BaseResponse<InventoryPeriodDetail>> {
    const response = await axios.get<BaseResponse<InventoryPeriodDetail>>(
      `api/${this.endpoint}/${periodId}`
    );
    return response.data;
  }

  // GET api/InventoryPeriod/{periodId}/Opening
  async fetchOpening(periodId: number): Promise<BaseResponse<InventoryPeriodOpeningResponse[]>> {
    const response = await axios.get<BaseResponse<InventoryPeriodOpeningResponse[]>>(
      `api/${this.endpoint}/${periodId}/Opening`
    );
    return response.data;
  }

  // GET api/InventoryPeriod/{periodId}/Closing
  async fetchClosing(periodId: number): Promise<BaseResponse<InventoryPeriodClosingResponse[]>> {
    const response = await axios.get<BaseResponse<InventoryPeriodClosingResponse[]>>(
      `api/${this.endpoint}/${periodId}/Closing`
    );
    return response.data;
  }

  // GET api/InventoryPeriod/{periodId}/SystemStock
  async fetchSystemStock(periodId: number): Promise<BaseResponse<InventoryPeriodClosingResponse[]>> {
    const response = await axios.get<BaseResponse<InventoryPeriodClosingResponse[]>>(
      `api/${this.endpoint}/${periodId}/SystemStock`
    );
    return response.data;
  }

  // POST api/InventoryPeriod/Open
  async openPeriod(data: InventoryPeriodOpenRequest): Promise<BaseResponse<boolean>> {
    const response = await axios.post<BaseResponse<boolean>>(
      `api/${this.endpoint}/Open`,
      data
    );
    return response.data;
  }

  // PUT api/InventoryPeriod/Close
  async closePeriod(data: InventoryPeriodCloseRequest): Promise<BaseResponse<boolean>> {
    const response = await axios.put<BaseResponse<boolean>>(
      `api/${this.endpoint}/Close`,
      data
    );
    return response.data;
  }
}

export const inventoryPeriodService = new InventoryPeriodService();