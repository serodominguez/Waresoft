import { BaseService } from './baseService';
import { Product } from '@/interfaces/productInterface';

class ProductService extends BaseService<Product> {
  constructor() {
    super({
      endpoint: 'Product',
      downloadFileName: 'Productos',
    });
  }
}

export const productService = new ProductService();