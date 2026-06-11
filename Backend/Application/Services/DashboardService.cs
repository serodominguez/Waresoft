using Application.Commons.Bases.Response;
using Application.Dtos.Response.Dashboard;
using Application.Interfaces;
using Application.Mappers;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<DashboardGoodsIssueStatsResponseDto>> GetGoodsIssueStats(int authenticatedStoreId, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<DashboardGoodsIssueStatsResponseDto>();

            try
            {
                var stats = await _unitOfWork.DashboardQuery.GetGoodsIssueStatsLast14DaysAsync(authenticatedStoreId, cancellationToken);
                response.IsSuccess = true;
                response.Data = DashboardMapp.GoodsIssueStatsResponseDtoMapping(stats);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<DashboardStoreInventoryStatsResponseDto>> GetInventoryStats(int authenticatedStoreId, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<DashboardStoreInventoryStatsResponseDto>();

            try
            {
                var stats = await _unitOfWork.DashboardQuery.GetInventoryStatsCurrentMonthAsync(authenticatedStoreId, cancellationToken);
                response.IsSuccess = true;
                response.Data = DashboardMapp.InventoryStatsResponseDtoMapping(stats);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<DashboardMovementResponseDto>>> GetMovementsStats(int authenticatedStoreId, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<DashboardMovementResponseDto>>();

            try
            {
                var movements = await _unitOfWork.DashboardQuery.GetMovementsLast6MonthsAsync(authenticatedStoreId, cancellationToken);
                response.IsSuccess = true;
                response.Data = movements.Select(DashboardMapp.MovementStatsResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<DashboardProductReplenishmentResponseDto>> GetProductReplenishment(int authenticatedStoreId, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<DashboardProductReplenishmentResponseDto>();

            try
            {
                var status = await _unitOfWork.DashboardQuery.GetProductReplenishmentAsync(authenticatedStoreId, cancellationToken);
                response.IsSuccess = true;
                response.Data = DashboardMapp.ProductReplenishmentResponseDtoMapping(status);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<DashboardProductStatsResponseDto>> GetProductStats(CancellationToken cancellationToken)
        {
            var response = new BaseResponse<DashboardProductStatsResponseDto>();

            try
            {
                var stats = await _unitOfWork.DashboardQuery.GetProductStatsCurrentMonthAsync(cancellationToken);
                response.IsSuccess = true;
                response.Data = DashboardMapp.ProductStatsResponseDtoMapping(stats);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<DashboardTransferByStoreResponseDto>>> GetTransfersByStore(int authenticatedStoreId, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<DashboardTransferByStoreResponseDto>>();
            try
            {
                var transfers = await _unitOfWork.DashboardQuery.GetTransfersByStoreLast6MonthsAsync(authenticatedStoreId, cancellationToken);
                response.IsSuccess = true;
                response.Data = transfers.Select(DashboardMapp.TransferByStoreResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<DashboardTransferStatsResponseDto>> GetTransferPending(int authenticatedStoreId, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<DashboardTransferStatsResponseDto>();

            try
            {
                var stats = await _unitOfWork.DashboardQuery.GetTransferPendingAsync(authenticatedStoreId, cancellationToken);
                response.IsSuccess = true;
                response.Data = DashboardMapp.TransferPendingResponseDtoMapping(stats);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<DashboardTransferStatusResponseDto>>> GetTransferStatus(int authenticatedStoreId, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<DashboardTransferStatusResponseDto>>();

            try
            {
                var transfers = await _unitOfWork.DashboardQuery.GetTransferStatusLast6MonthsAsync(authenticatedStoreId, cancellationToken);
                response.IsSuccess = true;
                response.Data = transfers.Select(DashboardMapp.TransferStatusResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }
    }
}
