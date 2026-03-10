using Application.Commons.Bases.Response;
using Application.Dtos.Request.User;
using Application.Interfaces;
using Application.Security;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurity _security;
        private readonly IValidator<TokenRequestDto> _validator;

        public AuthorizationService(IUnitOfWork unitOfWork, ISecurity security, IValidator<TokenRequestDto> validator)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            _validator = validator;
        }

        public async Task<BaseResponse<string>> GenerateToken(TokenRequestDto requestDto)
        {
            var response = new BaseResponse<string>();

            var validationResult = await _validator.ValidateAsync(requestDto);

            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                response.Errors = validationResult.Errors;
                return response;
            }

            var user = await _unitOfWork.User.GetUsersQueryable()
                            .Where(u => u.UserName == requestDto.UserName && u.Status == true)
                            .FirstOrDefaultAsync();

            if (user is not null)
            {
                if (!_security.VerifyPasswordHash(requestDto.Password!, user.PasswordHash!, user.PasswordSalt!))
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_INCORRECT_PASSWORD;
                }
                else
                {
                    response.IsSuccess = true;
                    response.Data = _security.GenerateToken(user);
                    response.Message = ReplyMessage.MESSAGE_TOKEN;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_INCORRECT_USER;
            }

            return response;
        }
    }
}
