export interface GoodsReceipt {
  idReceipt: number | null;
  code: string;
  type: string;
  storeName: string;
  idSupplier: number | null;
  companyName: string;
  documentDate: string;
  documentType: string;
  documentNumber: string;
  totalAmount: number;
  annotations: string;
  auditCreateDate: string;
  statusReceipt: string;
}

export interface GoodsReceiptDetail {
  idProduct: number;
  code: string;
  description: string;
  material: string;
  color: string;
  categoryName: string;
  brandName: string;
  quantity: number;
  unitCost: number;
  totalCost: number;
}

export interface GoodsReceiptRegister {
  type: string;
  documentDate: string | null;
  documentType: string;
  documentNumber: string;
  totalAmount: number;
  annotations: string;
  idSupplier: number | null;
  idStore: number;
  goodsReceiptDetails: {
    item: number;
    idProduct: number;
    quantity: number;
    unitCost: number;
    totalCost: number;
  }[];
}