import { FilterParams, BaseResponse } from '@/interfaces/baseInterface';

export interface BaseService<T> {
  fetchAll(params: FilterParams): Promise<BaseResponse<T[]>>;
  select(): Promise<BaseResponse<T[]>>;
  fetchById(id: number): Promise<BaseResponse<T>>;
  create(item: T): Promise<BaseResponse<T>>;
  update(id: number, item: T): Promise<BaseResponse<T>>;
  enable(id: number): Promise<BaseResponse<void>>;
  disable(id: number): Promise<BaseResponse<void>>;
  remove(id: number): Promise<BaseResponse<void>>;
  downloadExcel(params?: FilterParams): Promise<void>;
  downloadPdf(params?: FilterParams): Promise<void>;
}