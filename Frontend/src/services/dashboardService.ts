import axios from 'axios';
import { BaseResponse } from '@/interfaces/baseInterface';
import { 
  DashboardGoodsIssueStats, 
  DashboardInventoryStats,
  DashboardMovementStats,
  DashboardProductReplenishment, 
  DashboardProductStats,
  DashboardTransferByStore, 
  DashboardTransferPending,
  DashboardTransferStatus } from '@/interfaces/dashboardInterface';

class DashboardService {
  async getGoodsIssueStats(): Promise<DashboardGoodsIssueStats> {
    const response = await axios.get<BaseResponse<DashboardGoodsIssueStats>>('api/Dashboard/GoodsIssueStats');
    return response.data.data;
  }

  async getInventoryStats(): Promise<DashboardInventoryStats> {
    const response = await axios.get<BaseResponse<DashboardInventoryStats>>('api/Dashboard/InventoryStats');
    return response.data.data;
  }

  async getMovementsStats(): Promise<DashboardMovementStats[]> {
    const response = await axios.get<BaseResponse<DashboardMovementStats[]>>('api/Dashboard/MovementsStats');
    return response.data.data;
  }

  async getProductReplenishment(): Promise<DashboardProductReplenishment> {
    const response = await axios.get<BaseResponse<DashboardProductReplenishment>>('api/Dashboard/ProductReplenishment');
    return response.data.data;
  }

  async getProductStats(): Promise<DashboardProductStats> {
    const response = await axios.get<BaseResponse<DashboardProductStats>>('api/Dashboard/ProductStats');
    return response.data.data;
  }

  async getTransfersByStore(): Promise<DashboardTransferByStore[]> {
    const response = await axios.get<BaseResponse<DashboardTransferByStore[]>>('api/Dashboard/TransfersByStore');
    return response.data.data;
  }

  async getTransferPending(): Promise<DashboardTransferPending> {
    const response = await axios.get<BaseResponse<DashboardTransferPending>>('api/Dashboard/TransferPending');
    return response.data.data;
  }

  async getTransferStatus(): Promise<DashboardTransferStatus[]> {
    const response = await axios.get<BaseResponse<DashboardTransferStatus[]>>('api/Dashboard/TransferStatus');
    return response.data.data;
  }
}

export const dashboardService = new DashboardService();