export interface Inventory {
  idStore: number | null;
  idProduct: number | null;
  code: string;
  description: string;
  material: string;
  color: string;
  unitMeasure: string,
  stockAvailable: number | null;
  calculatedStock: number | null;
  stockDifference: number | null;
  stockInTransit: number | null;
  price: number | null;
  replenishment: string;
  brandName: string;
  categoryName: string;
  auditCreateDate: string;
}

export interface InventoryPivotRow {
  image: string;
  code: string;
  color: string;
  brandName: string;
  categoryName: string;
  auditCreateDate: string;
  stockByStore: Record<string, number>;
}

export interface InventoryPivot {
  stores: string[];
  rows: InventoryPivotRow[];
}