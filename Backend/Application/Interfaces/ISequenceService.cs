using Application.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface ISequenceService
    {
        Task<BaseResponse<string>> ViewProductCode();
    }
}
