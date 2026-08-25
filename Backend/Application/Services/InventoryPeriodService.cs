using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.InventoryPeriod;
using Application.Dtos.Response.InventoryPeriod;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class InventoryPeriodService : IInventoryPeriodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<InventoryPeriodOpenRequestDto> _openValidator;
        private readonly IValidator<InventoryPeriodCloseRequestDto> _closeValidator;
        private readonly IOrderingQuery _orderingQuery;

        public InventoryPeriodService(IUnitOfWork unitOfWork, IValidator<InventoryPeriodOpenRequestDto> openValidator, IValidator<InventoryPeriodCloseRequestDto> closeValidator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _openValidator = openValidator;
            _closeValidator = closeValidator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<InventoryPeriodResponseDto>>> ListPeriods(int storeId, BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<InventoryPeriodResponseDto>>();
            try
            {
                var periods = _unitOfWork.InventoryPeriodQuery
                    .GetPeriodListQueryable(storeId);

                if (!string.IsNullOrEmpty(filters.TextFilter))
                {
                    periods = periods.Where(x => x.PeriodName!.Contains(filters.TextFilter));
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                    periods = periods.Where(x => x.StartDate >= startDate && x.StartDate < endDate);
                }

                response.TotalRecords = await periods.CountAsync();

                filters.Sort ??= "IdPeriod";
                var items = await _orderingQuery.Ordering(filters, periods, true).ToListAsync();

                response.IsSuccess = true;
                response.Data = items.Select(InventoryPeriodMapp.InventoryPeriodResponseMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION;
                throw;
            }

            return response;
        }

        public async Task<BaseResponse<InventoryPeriodDetailResponseDto>> GetPeriodDetail(int periodId)
        {
            var response = new BaseResponse<InventoryPeriodDetailResponseDto>();
            try
            {
                var period = await _unitOfWork.InventoryPeriodQuery.GetPeriodDetailAsync(periodId);

                if (period is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                response.IsSuccess = true;
                response.Data = InventoryPeriodMapp.InventoryPeriodDetailResponseMapping(period);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION;
                throw;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<InventoryPeriodClosingResponseDto>>> GetClosingByPeriod(int periodId)
        {
            var response = new BaseResponse<IEnumerable<InventoryPeriodClosingResponseDto>>();
            try
            {
                var closing = await _unitOfWork.InventoryPeriodQuery.GetClosingByPeriodAsync(periodId);

                response.IsSuccess = true;
                response.TotalRecords = closing.Count;
                response.Data = closing.Select(InventoryPeriodMapp.InventoryPeriodClosingResponseMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION;
                throw;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<InventoryPeriodOpeningResponseDto>>> GetOpeningByPeriod(int periodId)
        {
            var response = new BaseResponse<IEnumerable<InventoryPeriodOpeningResponseDto>>();
            try
            {
                var opening = await _unitOfWork.InventoryPeriodQuery.GetOpeningByPeriodAsync(periodId);

                response.IsSuccess = true;
                response.TotalRecords = opening.Count;
                response.Data = opening.Select(InventoryPeriodMapp.InventoryPeriodOpeningResponseMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION;
                throw;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<InventoryPeriodClosingResponseDto>>> GetSystemStockCalculated(int periodId)
        {
            var response = new BaseResponse<IEnumerable<InventoryPeriodClosingResponseDto>>();
            try
            {
                var systemStock = await _unitOfWork.InventoryPeriodQuery.CalculateSystemStockAsync(periodId);

                response.IsSuccess = true;
                response.TotalRecords = systemStock.Count;
                response.Data = systemStock.Select(InventoryPeriodMapp.InventoryPeriodClosingResponseMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION;
                throw;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> OpenPeriod(int authenticatedUserId, int authenticatedStoreId, InventoryPeriodOpenRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            var validationResult = await _openValidator.ValidateAsync(requestDto);

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
                // Verificar que no exista un período abierto para el almacén
                var existingOpenPeriod = await _unitOfWork.InventoryPeriodQuery
                    .GetPeriodListQueryable(authenticatedStoreId)
                    .AnyAsync(p => p.Status == "OPEN");

                if (existingOpenPeriod)
                {
                    response.IsSuccess = false;
                    response.Message = "Ya existe un período abierto para este almacén";
                    return response;
                }

                var entity = InventoryPeriodMapp.InventoryPeriodOpenMapping(requestDto, authenticatedUserId, authenticatedStoreId);
                await _unitOfWork.InventoryPeriodCommand.OpenPeriodAsync(entity);

                // Verificar si existe un período cerrado anterior
                var lastClosedPeriod = await _unitOfWork.InventoryPeriodQuery
                    .GetPeriodListQueryable(authenticatedStoreId)
                    .Where(p => p.Status == "CLOSED")
                    .OrderByDescending(p => p.IdPeriod)
                    .FirstOrDefaultAsync();

                if (lastClosedPeriod is null)
                {
                    // Primer período — tomar desde StoresInventory
                    var currentStock = await _unitOfWork.StoreInventoryCommand
                        .GetStocksByStoreAsQueryable(authenticatedStoreId)
                        .ToListAsync();

                    var opening = currentStock.Select(s => new InventoryPeriodOpeningEntity
                    {
                        IdPeriod = entity.IdPeriod,
                        IdProduct = s.IdProduct,
                        OpeningStock = s.StockAvailable
                    }).ToList();

                    await _unitOfWork.InventoryPeriodCommand.SaveOpeningAsync(opening);
                }
                else
                {
                    // Períodos posteriores — tomar desde InventoryPeriodClosing del último cerrado
                    var closingStock = await _unitOfWork.InventoryPeriodQuery
                        .GetClosingByPeriodAsync(lastClosedPeriod.IdPeriod);

                    var opening = closingStock.Select(c => new InventoryPeriodOpeningEntity
                    {
                        IdPeriod = entity.IdPeriod,
                        IdProduct = c.IdProduct,
                        OpeningStock = c.ClosingStock
                    }).ToList();

                    await _unitOfWork.InventoryPeriodCommand.SaveOpeningAsync(opening);
                }

                transaction.Commit();
                response.IsSuccess = true;
                response.Data = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception)
            {
                transaction.Rollback();
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION;
                throw;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> ClosePeriod(int authenticatedUserId, int authenticatedStoreId, InventoryPeriodCloseRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            var validationResult = await _closeValidator.ValidateAsync(requestDto);

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
                // Verificar que el período exista y esté abierto
                var period = await _unitOfWork.InventoryPeriodQuery
                    .GetPeriodListQueryable(authenticatedStoreId)
                    .FirstOrDefaultAsync(p => p.IdPeriod == requestDto.IdPeriod && p.Status == "OPEN");

                if (period is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                // 1. Calcular SystemStock desde movimientos del período
                var systemStocks = await _unitOfWork.InventoryPeriodQuery
                    .CalculateSystemStockAsync(requestDto.IdPeriod);

                // 2. Construir InventoryPeriodClosing
                var closingEntities = systemStocks.Select(s =>
                {
                    var physicalCount = requestDto.PhysicalCounts
                        .FirstOrDefault(p => p.IdProduct == s.IdProduct);

                    var physicalStock = physicalCount?.PhysicalStock;
                    var closingStock = physicalStock ?? s.SystemStock;

                    return new InventoryPeriodClosingEntity
                    {
                        IdPeriod = requestDto.IdPeriod,
                        IdProduct = s.IdProduct,
                        SystemStock = s.SystemStock,
                        PhysicalStock = physicalStock,
                        Difference = physicalStock.HasValue
                            ? physicalStock.Value - s.SystemStock
                            : null,
                        ClosingStock = closingStock
                    };
                }).ToList();

                // 3. Grabar cierre
                await _unitOfWork.InventoryPeriodCommand.SaveClosingAsync(closingEntities);

                // 4. Sincronizar cache StoresInventory
                await _unitOfWork.InventoryPeriodCommand.RegisterAdjustmentAsync(
                    authenticatedStoreId,
                    requestDto.IdPeriod,
                    closingEntities);

                // 5. Cerrar período
                await _unitOfWork.InventoryPeriodCommand.ClosePeriodAsync(
                    requestDto.IdPeriod,
                    authenticatedUserId);

                transaction.Commit();
                response.IsSuccess = true;
                response.Data = true;
                response.Message = ReplyMessage.MESSAGE_UPDATE;
            }
            catch (Exception)
            {
                transaction.Rollback();
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION;
                throw;
            }

            return response;
        }
    }
}
