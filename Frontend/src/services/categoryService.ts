import { BaseService } from './baseService';
import { Category } from '@/interfaces/categoryInterface';

class CategoryService extends BaseService<Category> {
  constructor() {
    super({
      endpoint: 'Category',
      downloadFileName: 'Categorías',
    });
  }
}

export const categoryService = new CategoryService();