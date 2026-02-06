using Application.Commons.Bases.Response;
using Application.Dtos.Request.User;

namespace Application.Interfaces
{
    public interface IAuthorizationService
    {
        Task<BaseResponse<string>> GenerateToken(TokenRequestDto requestDto);
    }
}
