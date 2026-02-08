using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.GoodsIssue;
using Application.Dtos.Response.GoodsIssue;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class GoodsIssueService : IGoodsIssueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<GoodsIssueRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public GoodsIssueService(IUnitOfWork unitOfWork, IValidator<GoodsIssueRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<GoodsIssueResponseDto>>> ListGoodsIssueByStore(int authenticatedStoreId, BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<GoodsIssueResponseDto>>();
            try
            {
                var issues = _unitOfWork.GoodsIssue.GetGoodsIssueQueryableByStore(authenticatedStoreId);

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            issues = issues.Where(x => x.Code!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            issues = issues.Where(x => x.Store.StoreName!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            issues = issues.Where(x => x.User.UserName!.Contains(filters.TextFilter) || x.User.LastNames!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    issues = issues.Where(x => x.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                    issues = issues.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }
                response.TotalRecords = await issues.CountAsync();

                filters.Sort ??= "IdIssue";
                var items = await _orderingQuery.Ordering(filters, issues, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(GoodsIssueMapp.GoodsIssueResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<GoodsIssueWithDetailsResponseDto>> GoodsIssueById(int issueId)
        {
            var response = new BaseResponse<GoodsIssueWithDetailsResponseDto>();

            try
            {
                var issue = await _unitOfWork.GoodsIssue.GetGoodsIssueByIdAsync(issueId);

                if (issue is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                string? userName = null;
                if (issue!.AuditCreateUser.HasValue)
                {
                    var user = await _unitOfWork.User.GetByIdAsync(issue.AuditCreateUser.Value);
                    userName = user?.Names+' '+ user?.LastNames;
                }

                var details = await _unitOfWork.GoodsIssueDetails.GetGoodsIssueDetailsAsync(issue!.IdIssue);

                issue.GoodsIssueDetails = details.ToList();

                response.IsSuccess = true;
                response.Data = GoodsIssueMapp.GoodsIssueWithDetailsResponseDtoMapping(issue, userName);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterGoodsIssue(int authenticatedUserId, GoodsIssueRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            var validationResult = await _validator.ValidateAsync(requestDto);

            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                response.Errors = validationResult.Errors;
                return response;
            }

            using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                var entity = GoodsIssueMapp.GoodsIssueMapping(requestDto);
                entity.Code = await _unitOfWork.GoodsIssue.GenerateCodeAsync();
                entity.AuditCreateUser = authenticatedUserId;
                entity.AuditCreateDate = DateTime.Now;
                entity.Status = true;
                await _unitOfWork.GoodsIssue.RegisterGoodsIssueAsync(entity);

                foreach (var item in entity.GoodsIssueDetails)
                {
                    var currentStock = await _unitOfWork.StoreInventory.GetStockByIdAsync(item.IdProduct, requestDto.IdStore);
                    if (currentStock is null)
                    {
                        transaction.Rollback();
                        response.IsSuccess = false;
                        response.Message = ReplyMessage.MESSAGE_NOT_FOUND + item.IdProduct;
                        return response;
                    }
                    else
                    {
                        currentStock.StockAvailable -= item.Quantity;
                        await _unitOfWork.StoreInventory.UpdateStockByProductsAsync(currentStock);
                    }
                }

                transaction.Commit();
                response.IsSuccess = true;
                response.Data = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> CancelGoodsIssue(int authenticatedUserId, int issueId)
        {
            var response = new BaseResponse<bool>();

            using var transaction = _unitOfWork.BeginTransaction();

            try
            {

                var issue = await _unitOfWork.GoodsIssue.GetGoodsIssueByIdAsync(issueId);
                if (issue is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                var details = await _unitOfWork.GoodsIssueDetails.GetGoodsIssueDetailsAsync(issue!.IdIssue);

                issue.AuditDeleteUser = authenticatedUserId;
                issue.AuditDeleteDate = DateTime.Now;
                issue.Status = false;

                response.Data = await _unitOfWork.GoodsIssue.CancelGoodsIssueAsync(issue);

                foreach (var item in details)
                {
                    var currentStock = await _unitOfWork.StoreInventory.GetStockByIdAsync(item.IdProduct, issue.IdStore);
                    if (currentStock is null)
                    {
                        transaction.Rollback();
                        response.IsSuccess = false;
                        response.Message = ReplyMessage.MESSAGE_NOT_FOUND + item.IdProduct;
                        return response;
                    }
                    else
                    {
                        currentStock.StockAvailable += item.Quantity;
                        await _unitOfWork.StoreInventory.UpdateStockByProductsAsync(currentStock);
                    }
                }

                transaction.Commit();
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }
    }
}
