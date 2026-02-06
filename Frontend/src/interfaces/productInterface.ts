export interface Product {
  idProduct: number | null;
  code: string;
  description: string;
  material: string;
  color: string;
  unitMeasure: string;
  idBrand: number | null;
  brandName: string,
  idCategory: number | null;
  categoryName: string,
  auditCreateDate: string;
  statusProduct: string;
}