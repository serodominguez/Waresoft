export interface InventoryPeriod {
  idPeriod: number;
  idStore: number;
  storeName: string;
  periodName: string;
  startDate: string;
  endDate: string;
  statusPeriod: string;
  openedByUser: number;
  openedDate: string;
  closedByUser: number | null;
  closedDate: string | null;
}

export interface InventoryPeriodDetail {
  idPeriod: number;
  idStore: number;
  storeName: string;
  periodName: string;
  startDate: string;
  endDate: string;
  status: string;
  openedByUser: number;
  openedDate: string;
  closedByUser: number | null;
  closedDate: string | null;
  totalProducts: number;
  totalSystemStock: number;
  totalPhysicalStock: number;
  totalDifference: number;
}

export interface InventoryPeriodOpeningResponse {
  idPeriod: number;
  idProduct: number;
  productCode: string;
  productDescription: string;
  unitMeasure: string;
  openingStock: number;
}

export interface InventoryPeriodClosingResponse {
  idPeriod: number;
  idProduct: number;
  productCode: string;
  productDescription: string;
  unitMeasure: string;
  systemStock: number;
  physicalStock: number | null;
  difference: number | null;
  closingStock: number;
}


export interface PhysicalCount {
  idProduct: number;
  physicalStock: number;
}

export interface InventoryPeriodOpenRequest {
  periodName: string;
  startDate: string | null;
  endDate: string | null;
}

export interface InventoryPeriodCloseRequest {
  idPeriod: number;
  physicalCounts: PhysicalCount[];
}