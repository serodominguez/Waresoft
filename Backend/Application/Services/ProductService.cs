using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Product;
using Application.Dtos.Response.Product;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ProductRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public ProductService(IUnitOfWork unitOfWork, IValidator<ProductRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<ProductResponseDto>>> ListProducts(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<ProductResponseDto>>();
            try
            {
                var products = _unitOfWork.Product.GetProductsQueryable();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            products = products.Where(x => x.Code!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            products = products.Where(x => x.Description!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            products = products.Where(x => x.Material!.Contains(filters.TextFilter));
                            break;
                        case 4:
                            products = products.Where(x => x.Color!.Contains(filters.TextFilter));
                            break;
                        case 5:
                            products = products.Where(x => x.Brand!.BrandName!.Contains(filters.TextFilter));
                            break;
                        case 6:
                            products = products.Where(x => x.Category!.CategoryName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    products = products.Where(x => x.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                    products = products.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }
                response.TotalRecords = await products.CountAsync();

                filters.Sort ??= "Id";
                var items = await _orderingQuery.Ordering(filters, products, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(ProductMapp.ProductsResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<ProductResponseDto>> ProductById(int productId)
        {
            var response = new BaseResponse<ProductResponseDto>();

            try
            {
                var product = await _unitOfWork.Product.GetByIdAsync(productId);

                if (product is not null)
                {
                    response.Data = ProductMapp.ProductsResponseDtoMapping(product);
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

        public async Task<BaseResponse<bool>> RegisterProduct(int authenticatedUserId, ProductRequestDto requestDto)
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

                var product = ProductMapp.ProductsMapping(requestDto);
                product.AuditCreateUser = authenticatedUserId;
                product.AuditCreateDate = DateTime.Now;
                product.Status = true;

                response.Data = await _unitOfWork.Product.RegisterAsync(product);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_SAVE;
                }
                else
                {
                    response.IsSuccess = false;
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
        public async Task<BaseResponse<bool>> EditProduct(int authenticatedUserId, int productId, ProductRequestDto requestDto)
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

                var isValid = await _unitOfWork.Product.GetByIdAsync(productId);
                if (isValid is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                var product = ProductMapp.ProductsMapping(requestDto);
                product.Id = productId;
                product.AuditUpdateUser = authenticatedUserId;
                product.AuditUpdateDate = DateTime.Now;

                response.Data = await _unitOfWork.Product.EditAsync(product);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
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

        public async Task<BaseResponse<bool>> EnableProduct(int authenticatedUserId, int productId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var product = await _unitOfWork.Product.GetByIdAsync(productId);

                if (product is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                product.AuditUpdateUser = authenticatedUserId;
                product.AuditUpdateDate = DateTime.Now;
                product.Status = true;

                response.Data = await _unitOfWork.Product.UpdateAsync(product);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_ACTIVATE;
                }
                else
                {
                    response.IsSuccess = false;
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

        public async Task<BaseResponse<bool>> DisableProduct(int authenticatedUserId, int productId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var product = await _unitOfWork.Product.GetByIdAsync(productId);

                if (product is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                product.AuditUpdateUser = authenticatedUserId;
                product.AuditUpdateDate = DateTime.Now;
                product.Status = false;

                response.Data = await _unitOfWork.Product.UpdateAsync(product);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_INACTIVATE;
                }
                else
                {
                    response.IsSuccess = false;
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

        public async Task<BaseResponse<bool>> RemoveProduct(int authenticatedUserId, int productId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var product = await _unitOfWork.Product.GetByIdAsync(productId);

                if (product is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                product.AuditDeleteUser = authenticatedUserId;
                product.AuditDeleteDate = DateTime.Now;
                product.Status = false;

                response.Data = await _unitOfWork.Product.RemoveAsync(product);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_DELETE;
                }
                else
                {
                    response.IsSuccess = false;
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
