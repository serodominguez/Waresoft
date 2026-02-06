import { BaseService } from './baseService';
import { Brand } from '@/interfaces/brandInterface';

/**
 * Servicio para gestión de marcas
 * Hereda toda la funcionalidad CRUD de BaseService
 */
class BrandService extends BaseService<Brand> {
  constructor() {
    super({
      endpoint: 'Brand',
      downloadFileName: 'Marcas',
    });
  }

  // Aquí puedes agregar métodos específicos de Brand si los necesitas
  // Por ejemplo, si Brand tiene un endpoint especial:
  
  // async getBrandsByCategory(categoryId: number) {
  //   const response = await axios.get(`api/Brand/ByCategory/${categoryId}`);
  //   return response.data;
  // }
}

// Exportar instancia única (Singleton)
export const brandService = new BrandService();