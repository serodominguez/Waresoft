export interface Permission {
  idPermission : number;
  idRole: number;
  idModule: number;
  moduleName: string;
  idAction: number;
  actionName: string;
  status: boolean;
}

export interface PermissionResponse {
  isSuccess: boolean;
  data: Permission[];
  totalRecords: number | null;
  message: string;
  errors: string[] | null;
}

export interface PermissionsByModule {
  module: string;
  permissions: {
    crear: boolean;
    leer: boolean;
    editar: boolean;
    eliminar: boolean;
    descargar: boolean;
  };
}
