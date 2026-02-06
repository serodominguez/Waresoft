using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.GoodsReceipt;
using Application.Dtos.Response.GoodsReceipt;

namespace Application.Interfaces
{
    public interface IGoodsReceiptService
    {
        Task<BaseResponse<IEnumerable<GoodsReceiptResponseDto>>> ListGoodsReceiptByStore(int authenticatedStoreId, BaseFiltersRequest filters);
        Task<BaseResponse<GoodsReceiptWithDetailsResponseDto>> GoodsReceiptById(int receiptId);
        Task<BaseResponse<bool>> RegisterGoodsReceipt(int authenticatedUserId, GoodsReceiptRequestDto requestDto);
        Task<BaseResponse<bool>> CancelGoodsReceipt(int authenticatedUserId, int receiptId);
    }
}

