/**
 * Representa el producto que retorna CommonProductIn
 * (usado en entradas y productos generales)
 */
export interface ProductInSelection {
  idProduct: number | null;
  code: string;
  description: string;
  material: string;
  color: string;
  categoryName: string;
  brandName: string;
}

/**
 * Representa el producto que retorna CommonProductOut
 * (usado en salidas y traspasos — incluye stock y precio)
 */
export interface ProductOutSelection extends ProductInSelection {
  price: number | null;
  stockAvailable: number | null;
  auditCreateDate: string;
  
}