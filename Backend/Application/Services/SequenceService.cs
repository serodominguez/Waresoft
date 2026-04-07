using Application.Commons.Bases.Response;
using Application.Interfaces;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class SequenceService : ISequenceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SequenceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<string>> ViewProductCode()
        {
            var response = new BaseResponse<string>();

            try
            {
                var code = await _unitOfWork.Sequence.ViewProductCodeAsync();

                response.IsSuccess = true;
                response.Data = code;
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
