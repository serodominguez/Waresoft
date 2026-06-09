export interface Customer {
  idCustomer: number | null;
  names: string;
  lastNames: string;
  identificationNumber: string;
  phoneNumber: string;
  auditCreateDate: string;
  statusCustomer: string;
}

export interface CustomerStats {
  totalActive: number;
  percentageChange: number;
  isPositive: boolean;
}