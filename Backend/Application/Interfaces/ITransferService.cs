using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Transfer;
using Application.Dtos.Response.Transfer;

namespace Application.Interfaces
{
    public interface ITransferService
    {
        Task<BaseResponse<IEnumerable<TransferResponseDto>>> ListTransferByStore(int authenticatedStoreId, BaseFiltersRequest filters);
        Task<BaseResponse<TransferWithDetailsResponseDto>> TransferById(int authenticatedStoreId, int transferId);
        Task<BaseResponse<bool>> SendTransfer(int authenticatedUserId, TransferRequestDto requestDto);
        Task<BaseResponse<bool>> ReceiveTransfer(int authenticatedUserId, int transferId);
        Task<BaseResponse<bool>> CancelTransfer(int authenticatedUserId, int transferId);
    }
}
