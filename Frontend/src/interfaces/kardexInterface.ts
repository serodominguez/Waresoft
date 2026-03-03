export interface KardexMovement {
  idProduct: number;
  quantity: number;
  code: string;
  date: string;
  movementType: 'Entrada' | 'Salida' | 'Traspaso';
  type: string;
  state: string;
  accumulatedStock: number;
}

export interface KardexDetail {
  idProduct: number;
  code: string;
  description: string;
  material: string;
  color: string;
  unitMeasure: string;
  currentStock: number;
  calculatedStock: number;
  stockDifference: number;
  movements: KardexMovement[];
}

export interface KardexResponse {
  isSuccess: boolean;
  data: KardexDetail;
  totalRecords: number;
  message: string;
  errors: string | null;
}