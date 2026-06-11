export interface DashboardGoodsIssueStats {
  totalIssues: number;
  differenceVsLast7Days: number;
  isPositive: boolean;
}

export interface DashboardInventoryStats {
  belowMinimum: number;
  differenceVsLastMonth: number;
  isPositive: boolean;
}

export interface DashboardMovementStats {
  month: string;
  receipts: number;
  issues: number;
}

export interface DashboardProductReplenishment {
  available: number;
  notAvailable: number;
  discontinued: number;
}

export interface DashboardProductStats {
  totalActive: number;
  newThisMonth: number;
}

export interface DashboardTransferByStore {
  storeName: string;
  totalTransfers: number;
}

export interface DashboardTransferPending {
  totalPending: number;
  differenceVsYesterday: number;
  isPendingPositive: boolean;
}

export interface DashboardTransferStatus {
  month: string;
  sent: number;
  pending: number;
  received: number;
}

