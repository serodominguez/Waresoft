using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Brand;
using Application.Dtos.Response.Brand;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<BrandRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public BrandService(IUnitOfWork unitOfWork, IValidator<BrandRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<BrandResponseDto>>> ListBrands(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<BrandResponseDto>>();

            try
            {
                var brands = _unitOfWork.Brand.GetAllQueryable();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            brands = brands.Where(x => x.BrandName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    brands = brands.Where(x => x.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                    brands = brands.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }

                response.TotalRecords = await brands.CountAsync();

                filters.Sort ??= "Id";
                var items = await _orderingQuery.Ordering(filters, brands, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(BrandMapp.BrandsResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<BrandSelectResponseDto>>> ListSelectBrands()
        {
            var response = new BaseResponse<IEnumerable<BrandSelectResponseDto>>();

            try
            {
                var brands = (await _unitOfWork.Brand.GetSelectAsync());

                if (brands is not null && brands.Any())
                {
                    response.Data = brands.Select(BrandMapp.BrandsSelectResponseDtoMapping);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<BrandResponseDto>> BrandById(int brandId)
        {
            var response = new BaseResponse<BrandResponseDto>();

            try
            {
                var brand = await _unitOfWork.Brand.GetByIdAsync(brandId);

                if (brand is not null)
                {
                    response.Data = BrandMapp.BrandsResponseDtoMapping(brand);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterBrand(int authenticatedUserId, BrandRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var validationResult = await _validator.ValidateAsync(requestDto);

                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    response.Errors = validationResult.Errors;
                    return response;
                }

                var brand = BrandMapp.BrandsMapping(requestDto);
                brand.AuditCreateUser = authenticatedUserId;
                brand.AuditCreateDate = DateTime.Now;
                brand.Status = true;

                await _unitOfWork.Brand.AddAsync(brand);

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_SAVE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditBrand(int authenticatedUserId, int brandId, BrandRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var validationResult = await _validator.ValidateAsync(requestDto);

                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    response.Errors = validationResult.Errors;
                    return response;
                }

                var brand = await _unitOfWork.Brand.GetByIdForUpdateAsync(brandId);

                if (brand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                brand.BrandName = requestDto.BrandName.NormalizeString();
                brand.AuditUpdateUser = authenticatedUserId;
                brand.AuditUpdateDate = DateTime.Now;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EnableBrand(int authenticatedUserId, int brandId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var brand = await _unitOfWork.Brand.GetByIdForUpdateAsync(brandId);

                if (brand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                brand.AuditUpdateUser = authenticatedUserId;
                brand.AuditUpdateDate = DateTime.Now;
                brand.Status = true;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_ACTIVATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DisableBrand(int authenticatedUserId, int brandId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var brand = await _unitOfWork.Brand.GetByIdForUpdateAsync(brandId);

                if (brand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                brand.AuditUpdateUser = authenticatedUserId;
                brand.AuditUpdateDate = DateTime.Now;
                brand.Status = false;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_ACTIVATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveBrand(int authenticatedUserId, int brandId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var brand = await _unitOfWork.Brand.GetByIdForUpdateAsync(brandId);

                if (brand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                brand.AuditDeleteUser = authenticatedUserId;
                brand.AuditDeleteDate = DateTime.Now;
                brand.Status = false;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_ACTIVATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
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
