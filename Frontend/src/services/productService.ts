import axios from 'axios';
import { BaseService } from './baseService';
import { Product, ProductStats } from '@/interfaces/productInterface';
import { BaseResponse } from '@/interfaces/baseInterface';

class ProductService extends BaseService<Product> {
  constructor() {
    super({
      endpoint: 'Product',
      downloadFileName: 'Productos',
    });
  }

  async getStats(): Promise<ProductStats> {
    const response = await axios.get<{ data: ProductStats }>(`api/Product/Stats`);
    return response.data.data;
  }

  async registerProduct(data: FormData): Promise<BaseResponse<Product>> {
    const response = await axios.post<BaseResponse<Product>>(`api/Product/Register`, data);
    return response.data;
  }

  async editProduct(id: number, data: FormData): Promise<BaseResponse<Product>> {
    const response = await axios.put<BaseResponse<Product>>(`api/Product/Edit/${id}`, data);
    return response.data;
  }

  async generateProductCode(): Promise<BaseResponse<string>> {
    const response = await axios.get<BaseResponse<string>>(`api/Sequence/Product-Code`);
    return response.data;
  }

  async generateBarcodePdf(idProduct: number, quantity: number): Promise<Blob> {
      const response = await axios.post(`api/Product/BarcodePdf`, { idProduct, quantity },{ responseType: 'blob' });
  return response.data;
  }
}

export const productService = new ProductService();