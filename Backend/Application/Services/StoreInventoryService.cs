using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.StoreInventory;
using Application.Dtos.Response.StoreInventory;
using Application.Interfaces;
using Application.Mappers;
using Azure;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class StoreInventoryService : IStoreInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<StoreInventoryRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public StoreInventoryService(IUnitOfWork unitOfWork, IValidator<StoreInventoryRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<StoreInventoryResponseDto>>> ListInventory(int authenticatedStoreId, BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<StoreInventoryResponseDto>>();
            try
            {
                var inventory = _unitOfWork.StoreInventory.GetInventoryQueryable(authenticatedStoreId)
                    .Where(i => (i.Product.AuditDeleteUser == null && i.Product.AuditDeleteDate == null) || i.StockAvailable != 0 || i.StockInTransit != 0);

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            inventory = inventory.Where(x => x.Product.Code!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            inventory = inventory.Where(x => x.Product.Description!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            inventory = inventory.Where(x => x.Product.Material!.Contains(filters.TextFilter));
                            break;
                        case 4:
                            inventory = inventory.Where(x => x.Product.Color!.Contains(filters.TextFilter));
                            break;
                        case 5:
                            inventory = inventory.Where(x => x.Price.ToString().Contains(filters.TextFilter));
                            break;
                        case 6:
                            inventory = inventory.Where(x => x.Product.Brand!.BrandName!.Contains(filters.TextFilter));
                            break;
                        case 7:
                            inventory = inventory.Where(x => x.Product.Category!.CategoryName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    inventory = inventory.Where(x => x.Product.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                    inventory = inventory.Where(x => x.Product.AuditCreateDate >= startDate && x.Product.AuditCreateDate < endDate);
                }
                response.TotalRecords = await inventory.CountAsync();

                filters.Sort ??= "IdProduct";
                var items = await _orderingQuery.Ordering(filters, inventory, true).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(StoreInventoryMapp.StoreInventoryMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<StoreInventoryPivotResponseDto>> ListInventoryPivot(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<StoreInventoryPivotResponseDto>();
            try
            {
                var inventory = _unitOfWork.StoreInventory.GetAllInventoryQueryable();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            inventory = inventory.Where(x => x.Product.Code!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            inventory = inventory.Where(x => x.Product.Description!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            inventory = inventory.Where(x => x.Product.Material!.Contains(filters.TextFilter));
                            break;
                        case 4:
                            inventory = inventory.Where(x => x.Product.Color!.Contains(filters.TextFilter));
                            break;
                        case 5:
                            inventory = inventory.Where(x => x.Product.Brand!.BrandName!.Contains(filters.TextFilter));
                            break;
                        case 6:
                            inventory = inventory.Where(x => x.Product.Category!.CategoryName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    inventory = inventory.Where(x => x.Product.Status == stateValue);
                }


                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                    inventory = inventory.Where(x => x.Product.AuditCreateDate >= startDate && x.Product.AuditCreateDate < endDate);
                }

                var items = await inventory.ToListAsync();
                var stores = await _unitOfWork.Store.GetAllQueryable().ToListAsync();
                var pivot = StoreInventoryMapp.StoreInventoryPivotMapping(items, stores);

                response.TotalRecords = pivot.Rows.Count;

                var pageNumber = filters.NumberPage;
                var pageSize = filters.NumberRecordsPage;
                pivot.Rows = pivot.Rows
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                response.Data = pivot;
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<StoreInventoryKardexResponseDto>> ListKardexInventory(int authenticatedStoreId, int productId, BaseFiltersRequest filters)
        {
            var response = new BaseResponse<StoreInventoryKardexResponseDto>();
            try
            {
                // Obtener el inventario del producto (StoreInventoryEntity)
                var inventoryProduct = await _unitOfWork.StoreInventory
                    .GetInventoryQueryable(authenticatedStoreId)
                    .FirstOrDefaultAsync(i => i.IdProduct == productId);

                if (inventoryProduct is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                // Obtener todos los movimientos del producto
                var receipts = await _unitOfWork.GoodsReceiptDetails.GetGoodsReceiptDetailsByProductAsync(authenticatedStoreId, productId);
                var issues = await _unitOfWork.GoodsIssueDetails.GetGoodsIssueDetailsByProductAsync(authenticatedStoreId, productId);
                var transfers = await _unitOfWork.TransferDetails.GetTransferDetailsByProductAsync(authenticatedStoreId, productId);

                // Crear lista de movimientos usando mappers
                var movements = new List<StoreInventoryKardexMovementDto>();

                // Agregar SOLO Entradas COMPLETADAS
                movements.AddRange(
                    receipts
                        .Where(r => r.GoodsReceipt != null && r.GoodsReceipt.Status == (int)Movements.Completado)
                        .Select(StoreInventoryMapp.MapReceiptToKardexMovement)
                );

                // Agregar SOLO Salidas COMPLETADAS
                movements.AddRange(
                    issues
                        .Where(i => i.GoodsIssue != null && i.GoodsIssue.Status == (int)Movements.Completado)
                        .Select(StoreInventoryMapp.MapIssueToKardexMovement)
                );

                // Agregar SOLO Traspasos ENVIADOS o RECIBIDOS (no Cancelados ni Pendientes)
                movements.AddRange(
                    transfers
                        .Where(t => t.Transfer != null &&
                                   (t.Transfer.Status == (int)Transfers.Enviado ||
                                    t.Transfer.Status == (int)Transfers.Recibido))
                        .Select(t => StoreInventoryMapp.MapTransferToKardexMovement(t, authenticatedStoreId))
                );

                // Ordenar por fecha (Date ya es string en formato "dd/MM/yyyy HH:mm")
                movements = movements
                    .OrderBy(m => {
                        if (DateTime.TryParseExact(m.Date, "dd/MM/yyyy HH:mm",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out DateTime date))
                            return date;
                        return DateTime.MinValue;
                    })
                    .ToList();

                // Calcular stock acumulado ANTES de paginar (importante para mantener consistencia)
                int runningStock = 0;
                foreach (var movement in movements)
                {
                    runningStock += movement.Quantity;
                    movement.Stock = runningStock;
                }

                // Total de registros ANTES de paginar
                response.TotalRecords = movements.Count;

                // Aplicar paginación
                var pageNumber = filters.NumberPage;
                var pageSize = filters.NumberRecordsPage;
                var paginatedMovements = movements
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Usar mapper para crear respuesta final
                response.Data = StoreInventoryMapp.StoreInventoryKardexMapping(
                    inventoryProduct,
                    paginatedMovements,
                    runningStock);
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> UpdatePriceByProduct(int authenticatedUserId, int authenticatedStoreId, StoreInventoryRequestDto requestDto)
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

                var isValid = await _unitOfWork.Product.GetByIdAsync(requestDto.IdProduct);
                if (isValid is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;

                }
                var inventory = StoreInventoryMapp.StoreInventoryMapping(requestDto);
                inventory.IdStore = authenticatedStoreId;
                inventory.AuditUpdateUser = authenticatedUserId;
                inventory.AuditUpdateDate = DateTime.Now;

                response.Data = await _unitOfWork.StoreInventory.UpdatePriceByProductsAsync(inventory);

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
    }
}

