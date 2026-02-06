// Permisos CRUD para componentes de lista
export interface ListPermissions {
  canCreate: boolean;
  canRead: boolean;
  canEdit: boolean;
  canDelete: boolean;
  canDownload: boolean;
}

// Estado de filtros para componentes de lista
export interface ListState {
  drawer: boolean;
  selectedFilter: string;
  state: string;
  startDate: Date | null;
  endDate: Date | null;
}

// Props base para componentes de lista
export interface BaseListProps<T> extends ListPermissions, ListState {
  items: T[];
  loading: boolean;
  totalItems: number;
  downloadingExcel?: boolean;
  downloadingPdf?: boolean;
  itemsPerPage?: number;
}

