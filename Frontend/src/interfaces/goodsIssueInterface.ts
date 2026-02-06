export interface GoodsIssue {
  idIssue: number | null;
  code: string;
  type: string;
  storeName: string;
  idUser: number | null;
  userName: string;
  totalAmount: number;
  annotations: string;
  auditCreateDate: string;
  statusIssue: string;
}

export interface GoodsIssueDetail {
  idProduct: number;
  code: string;
  description: string;
  material: string;
  color: string;
  categoryName: string;
  brandName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface GoodsIssueRegister {
  type: string;
  totalAmount: number;
  annotations: string;
  idUser: number;
  idStore: number;
  goodsReceiptDetails: {
    item: number;
    idProduct: number;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
  }[];
}