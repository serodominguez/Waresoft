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

export interface KardexMovement {
  idProduct: number;
  quantity: number;
  idMovement: number;
  code: string;
  date: string;
  movementType: 'Entrada' | 'Salida' | 'Traspaso';
  type: string;
  state: string;
  accumulatedStock: number;
}

export interface KardexResponse {
  isSuccess: boolean;
  data: KardexDetail;
  totalRecords: number;
  message: string;
  errors: string | null;
}