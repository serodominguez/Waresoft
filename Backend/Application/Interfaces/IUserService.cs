using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.User;
using Application.Dtos.Response.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<IEnumerable<UserResponseDto>>> ListUsers(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<UserSelectResponseDto>>> ListSelectUsers();
        Task<BaseResponse<UserResponseDto>> UserById(int userId);
        Task<BaseResponse<bool>> RegisterUser(int authenticatedUserId, UserRequestDto requestDto);
        Task<BaseResponse<bool>> EditUser(int authenticatedUserId, int userId, UserRequestDto requestDto);
        Task<BaseResponse<bool>> EnableUser(int authenticatedUserId, int userId);
        Task<BaseResponse<bool>> DisableUser(int authenticatedUserId, int userId);
        Task<BaseResponse<bool>> RemoveUser(int authenticatedUserId, int userId);
    }
}
