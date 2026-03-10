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
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ProductRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;
        private readonly IFileStorageImageService _fileStorageImageService;

        public ProductService(IUnitOfWork unitOfWork, IValidator<ProductRequestDto> validator, IOrderingQuery orderingQuery, IFileStorageImageService fileStorageImageService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
            _fileStorageImageService = fileStorageImageService;
        }

        public async Task<BaseResponse<IEnumerable<ProductResponseDto>>> ListProducts(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<ProductResponseDto>>();

            try
            {
                var products = _unitOfWork.Product.GetProductsQueryable()
                    .AsNoTracking();

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
                var product = await _unitOfWork.Product.GetByIdAsQueryable(productId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

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

                if (requestDto.Image is not null)
                    product.Image = await _fileStorageImageService.SaveFile(ContainerConstants.PRODUCTS, requestDto.Image);

                product.AuditCreateUser = authenticatedUserId;
                product.AuditCreateDate = DateTime.Now;
                product.Replenishment = 1;
                product.Status = true;

                await _unitOfWork.Product.AddAsync(product);

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

                var product = await _unitOfWork.Product.GetByIdAsQueryable(productId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (product is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                product.Code = requestDto.Code.NormalizeString();
                product.Description = requestDto.Description.NormalizeString();
                product.Material = requestDto.Material.NormalizeString();
                product.Color = requestDto.Color.NormalizeString();
                product.UnitMeasure = requestDto.UnitMeasure.NormalizeString();
                product.IdBrand = requestDto.IdBrand;
                product.IdCategory = requestDto.IdCategory;
                product.AuditUpdateUser = authenticatedUserId;
                product.AuditUpdateDate = DateTime.Now;

                // ✅ Manejo correcto de imágenes
                if (requestDto.Image is not null)
                {
                    // Si hay nueva imagen, EditFile elimina la vieja y guarda la nueva
                    product.Image = await _fileStorageImageService.EditFile(
                        ContainerConstants.PRODUCTS,
                        requestDto.Image,
                        product.Image!
                    );
                }
                else if (requestDto.RemoveImage && !string.IsNullOrEmpty(product.Image))
                {
                    // Si NO hay nueva imagen pero se marcó para eliminar
                    await _fileStorageImageService.RemoveFile(product.Image!, ContainerConstants.PRODUCTS);
                    product.Image = null;
                }
                // Si NO hay nueva imagen Y NO se marcó para eliminar, mantener la actual

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

        public async Task<BaseResponse<bool>> EnableProduct(int authenticatedUserId, int productId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var product = await _unitOfWork.Product.GetByIdAsQueryable(productId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (product is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                product.AuditUpdateUser = authenticatedUserId;
                product.AuditUpdateDate = DateTime.Now;
                product.Replenishment = 1;
                product.Status = true;

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

        public async Task<BaseResponse<bool>> DisableProduct(int authenticatedUserId, int productId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var product = await _unitOfWork.Product.GetByIdAsQueryable(productId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (product is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                product.AuditUpdateUser = authenticatedUserId;
                product.AuditUpdateDate = DateTime.Now;
                product.Replenishment = 2;
                product.Status = false;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_INACTIVATE;
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

        public async Task<BaseResponse<bool>> RemoveProduct(int authenticatedUserId, int productId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var product = await _unitOfWork.Product.GetByIdAsQueryable(productId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (product is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                if (!string.IsNullOrEmpty(product.Image))
                    await _fileStorageImageService.RemoveFile(product.Image, ContainerConstants.PRODUCTS);

                product.AuditDeleteUser = authenticatedUserId;
                product.AuditDeleteDate = DateTime.Now;
                product.Image = null;
                product.Replenishment = 3;
                product.Status = false;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_DELETE;
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
