export interface Inventory {
  idStore: number | null;
  idProduct: number | null;
  code: string;
  description: string;
  material: string;
  color: string;
  unitMeasure: string,
  stockAvailable: number | null;
  stockInTransit: number | null;
  price: number | null;
  availability: string;
  brandName: string;
  categoryName: string;
}