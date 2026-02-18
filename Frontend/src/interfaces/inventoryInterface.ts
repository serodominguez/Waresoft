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
  replenishment: string;
  brandName: string;
  categoryName: string;
}

export interface InventoryPivotRow {
  codigo: string;
  color: string;
  marca: string;
  categoria: string;
  stockByStore: Record<string, number>;
}

export interface InventoryPivot {
  stores: string[];
  rows: InventoryPivotRow[];
}