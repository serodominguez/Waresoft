export interface User {
  idUser: number | null;
  userName: string;
  password: string;
  passwordHash: string;
  names: string;
  lastNames: string;
  identificationNumber: string;
  phoneNumber: number | null;
  idRole: number | null;
  roleName: string,
  idStore: number | null;
  storeName: string,
  auditCreateDate: string;
  statusUser: string;
  updatePassword: boolean;
}