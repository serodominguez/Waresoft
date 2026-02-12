export interface Transfer {
  idTransfer: number | null;
  code: string;
  sendDate: string;
  receiveDate: string;
  totalAmount: number;
  annotations: string;
  idStoreOrigin: number | null;
  storeOrigin: string;
  idStoreDestination: number | null;
  storeDestination: string;
  sendUser: string;
  receiveUser: string,
  statusTransfer: string;
}

export interface TransferDetail {
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

export interface TransferRegister {
  totalAmount: number;
  annotations: string;
  idStoreOrigin: number;
  idStoreDestination: number;
  transferDetails: {
    item: number;
    idProduct: number;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
  }[];
}