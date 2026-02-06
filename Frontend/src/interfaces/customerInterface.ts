export interface Customer {
  idCustomer: number | null;
  names: string;
  lastNames: string;
  identificationNumber: string;
  phoneNumber: number | null;
  auditCreateDate: string;
  statusCustomer: string;
}