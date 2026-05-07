using Dapper;
using Domain.Entities;
using Domain.Models;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Persistences.Repositories
{
    public class StoreInventoryRepository : IStoreInventoryRepository
    {
        private readonly DbContextSystem _context;
        private readonly string _connectionString;

        public StoreInventoryRepository(DbContextSystem context)
        {
            _context = context;
            _connectionString = context.Database.GetConnectionString()!;
        }

        public IQueryable<StoreInventoryEntity> GetInventoryQueryable(int storeId)
        {
            return _context.StoreInventory
                    .Where(i => i.IdStore == storeId)
                    .Select(i => new StoreInventoryEntity
                    {
                        IdStore = i.IdStore,
                        IdProduct = i.IdProduct,
                        StockAvailable = i.StockAvailable,
                        StockInTransit = i.StockInTransit,
                        Price = i.Price,
                        Store = new StoreEntity
                        {
                            StoreName = i.Store.StoreName
                        },
                        Product = new ProductEntity
                        {
                            Replenishment = i.Product.Replenishment,
                            Code = i.Product.Code,
                            Description = i.Product.Description,
                            Material = i.Product.Material,
                            Color = i.Product.Color,
                            UnitMeasure = i.Product.UnitMeasure,
                            Brand = new BrandEntity
                            {
                                BrandName = i.Product.Brand.BrandName
                            },
                            Category = new CategoryEntity
                            {
                                CategoryName = i.Product.Category.CategoryName
                            },
                            Status = i.Product.Status,
                            AuditCreateDate = i.Product.AuditCreateDate
                        }
                    });
        }

        public async Task<(List<StoreInventoryModel> Data, int TotalRecords)> GetInventoryListAsync(int storeId, int? numberFilter, string? textFilter, bool? stateFilter, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            var parameters = new DynamicParameters();
            parameters.Add("StoreId", storeId);

            string productFilters = " AND p.AuditDeleteUser IS NULL AND p.AuditDeleteDate IS NULL";

            if (numberFilter.HasValue && !string.IsNullOrEmpty(textFilter))
            {
                string? column = numberFilter switch
                {
                    1 => "p.Code",
                    2 => "p.Description",
                    3 => "p.Material",
                    4 => "p.Color",
                    5 => "si.Price",
                    6 => "b.BrandName",
                    7 => "c.CategoryName",
                    _ => null
                };

                if (column != null)
                {
                    if (numberFilter == 5)
                    {
                        if (decimal.TryParse(textFilter, out var priceValue))
                        {
                            productFilters += $" AND {column} = @Text";
                            parameters.Add("Text", priceValue);
                        }
                    }
                    else
                    {
                        productFilters += $" AND {column} LIKE @Text";
                        parameters.Add("Text", $"%{textFilter}%");
                    }
                }
            }

            if (stateFilter.HasValue)
            {
                productFilters += " AND p.Status = @State";
                parameters.Add("State", stateFilter.Value ? 1 : 0, DbType.Int16);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                productFilters += " AND p.AuditCreateDate >= @Start AND p.AuditCreateDate < @End";
                parameters.Add("Start", startDate.Value.Date);
                parameters.Add("End", endDate.Value.Date.AddDays(1));
            }

            parameters.Add("Offset", (pageNumber - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var sql = $@"
                            SELECT COUNT(*) 
                            FROM STORES_INVENTORY si 
                            INNER JOIN PRODUCTS p ON si.IdProduct = p.IdProduct
                            LEFT JOIN BRANDS b ON p.IdBrand = b.IdBrand
                            LEFT JOIN CATEGORIES c ON p.IdCategory = c.IdCategory
                            WHERE si.IdStore = @StoreId {productFilters};

                        -- B. OBTENER IDs DE LA PÁGINA ACTUAL
                            DROP TABLE IF EXISTS #PagedIds;
        
                            SELECT si.IdProduct INTO #PagedIds
                            FROM STORES_INVENTORY si
                            INNER JOIN PRODUCTS p ON si.IdProduct = p.IdProduct
                            LEFT JOIN BRANDS b ON p.IdBrand = b.IdBrand
                            LEFT JOIN CATEGORIES c ON p.IdCategory = c.IdCategory
                            WHERE si.IdStore = @StoreId {productFilters}
                            ORDER BY p.IdProduct DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                        -- C. SELECT FINAL con CTE de movimientos
                            ;WITH TotalMovementCTE AS (
                            SELECT IdProduct, SUM(Quantity) AS Total 
                            FROM (

                        -- Entradas por recepción
                            SELECT d.IdProduct, d.Quantity 
                            FROM GOODS_RECEIPT_DETAILS d 
                            INNER JOIN GOODS_RECEIPT r ON r.IdReceipt = d.IdReceipt 
                            WHERE r.IdStore = @StoreId 
                            AND r.Active = 1 
                            AND r.Status = 1
                            AND d.IdProduct IN (SELECT IdProduct FROM #PagedIds)
                            UNION ALL
                
                        -- Salidas por despacho
                            SELECT d.IdProduct, -d.Quantity
                            FROM GOODS_ISSUE_DETAILS d 
                            INNER JOIN GOODS_ISSUE i ON i.IdIssue = d.IdIssue 
                            WHERE i.IdStore = @StoreId 
                            AND i.Active = 1 
                            AND i.Status = 1
                            AND d.IdProduct IN (SELECT IdProduct FROM #PagedIds)
                            UNION ALL
                
                        -- Entradas por transferencia (destino)
                            SELECT d.IdProduct, d.Quantity 
                            FROM TRANSFERS_DETAILS d 
                            INNER JOIN TRANSFERS t ON t.IdTransfer = d.IdTransfer 
                            WHERE t.IdStoreDestination = @StoreId 
                            AND t.Active = 1 
                            AND t.Status != 0
                            AND d.IdProduct IN (SELECT IdProduct FROM #PagedIds)
                            UNION ALL
                
                        -- Salidas por transferencia (origen)
                            SELECT d.IdProduct, -d.Quantity 
                            FROM TRANSFERS_DETAILS d 
                            INNER JOIN TRANSFERS t ON t.IdTransfer = d.IdTransfer 
                            WHERE t.IdStoreOrigin = @StoreId 
                            AND t.Active = 1 
                            AND t.Status != 0
                            AND d.IdProduct IN (SELECT IdProduct FROM #PagedIds)) t GROUP BY IdProduct)
        
                        -- D. SELECT FINAL
                            SELECT  si.IdStore, si.IdProduct,
                                    si.StockAvailable, si.StockInTransit, si.Price,
                                    p.Replenishment, p.Code, p.Description,
                                    p.Material, p.Color, p.UnitMeasure,
                                    b.BrandName, c.CategoryName, p.AuditCreateDate,
                            COALESCE(cs.Total, 0) AS CalculatedStock
                            FROM STORES_INVENTORY si
                            INNER JOIN #PagedIds pi ON si.IdProduct = pi.IdProduct
                            INNER JOIN PRODUCTS p ON si.IdProduct = p.IdProduct
                            LEFT JOIN BRANDS b ON p.IdBrand = b.IdBrand
                            LEFT JOIN CATEGORIES c ON p.IdCategory = c.IdCategory
                            LEFT JOIN TotalMovementCTE cs ON si.IdProduct = cs.IdProduct
                            WHERE si.IdStore = @StoreId
                            ORDER BY p.IdProduct DESC;

                            DROP TABLE IF EXISTS #PagedIds;";

            using var connection = new SqlConnection(_connectionString);
            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var total = await multi.ReadFirstAsync<int>();
            var data = (await multi.ReadAsync<StoreInventoryModel>()).ToList();

            return (data, total);
        }

        public async Task<List<KardexMovementModel>> GetKardexByProductAsync(int storeId, int productId, DateTime? startDate, DateTime? endDate)
        {
            const string sql = @"WITH AllMovements AS (

                                -- ENTRADAS (GoodsReceipt)
                                            SELECT @ProductId AS IdProduct, d.Quantity, 
                                                    r.IdReceipt AS IdMovement, r.Code, 
                                                    r.AuditCreateDate AS Date, 'Entrada' AS MovementType, r.Type,
                                            CAST(r.Status AS VARCHAR(10)) AS State
                                            FROM GOODS_RECEIPT_DETAILS d
                                            INNER JOIN GOODS_RECEIPT r ON r.IdReceipt = d.IdReceipt
                                            WHERE d.IdProduct = @ProductId 
                                            AND r.IdStore = @StoreId 
                                            AND r.Active = 1 
                                            AND r.Status = 1
                                            UNION ALL

                                -- SALIDAS (GoodsIssue)
                                            SELECT @ProductId AS IdProduct, -d.Quantity, 
                                                    i.IdIssue AS IdMovement, i.Code, 
                                                    i.AuditCreateDate AS Date, 'Salida' AS MovementType, i.Type,
                                            CAST(i.Status AS VARCHAR(10)) AS State
                                            FROM GOODS_ISSUE_DETAILS d
                                            INNER JOIN GOODS_ISSUE i ON i.IdIssue = d.IdIssue
                                            WHERE d.IdProduct = @ProductId 
                                            AND i.IdStore = @StoreId 
                                            AND i.Active = 1 
                                            AND i.Status = 1
                                            UNION ALL

                                -- TRASPASOS RECIBIDOS (Destino)
                                            SELECT @ProductId AS IdProduct, d.Quantity, 
                                                    t.IdTransfer AS IdMovement, t.Code, 
                                                    t.AuditCreateDate AS Date, 'Traspaso' AS MovementType, 'Entrada' AS Type,
                                            CAST(t.Status AS VARCHAR(10)) AS State
                                            FROM TRANSFERS_DETAILS d
                                            INNER JOIN TRANSFERS t ON t.IdTransfer = d.IdTransfer
                                            WHERE d.IdProduct = @ProductId 
                                            AND t.IdStoreDestination = @StoreId 
                                            AND t.Active = 1 
                                            AND t.Status != 0
                                            UNION ALL

                                -- TRASPASOS ENVIADOS (Origen)
                                            SELECT @ProductId AS IdProduct, -d.Quantity, 
                                                    t.IdTransfer AS IdMovement, t.Code, 
                                                    t.AuditCreateDate AS Date, 'Traspaso' AS MovementType, 'Salida' AS Type,
                                            CAST(t.Status AS VARCHAR(10)) AS State
                                            FROM TRANSFERS_DETAILS d
                                            INNER JOIN TRANSFERS t ON t.IdTransfer = d.IdTransfer
                                            WHERE d.IdProduct = @ProductId 
                                            AND t.IdStoreOrigin = @StoreId 
                                            AND t.Active = 1 
                                            AND t.Status != 0), MovementsWithAccumulated AS (
                                            SELECT IdProduct, Quantity, IdMovement, Code, Date, MovementType, Type, State,
                                            SUM(Quantity) OVER (ORDER BY Date, IdMovement ROWS UNBOUNDED PRECEDING) AS AccumulatedStock
                                            FROM AllMovements)
                                            SELECT IdProduct, Quantity, IdMovement, Code, Date, MovementType, Type, State, AccumulatedStock
                                            FROM MovementsWithAccumulated
                                            WHERE (@StartDate IS NULL OR Date >= @StartDate)
                                            AND (@EndDate IS NULL OR Date <= @EndDate)
                                            ORDER BY Date ASC, IdMovement ASC";

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<KardexMovementModel>(sql, new
            {
                ProductId = productId,
                StoreId = storeId,
                StartDate = startDate,
                EndDate = endDate
            });

            return result.ToList();
        }

        public async Task<(List<InventoryPivotModel> Data, int TotalRecords)> GetInventoryPivotAsync(int? numberFilter, string? textFilter, bool? stateFilter,DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            var parameters = new DynamicParameters();

            string filters = " AND p.AuditDeleteUser IS NULL AND p.AuditDeleteDate IS NULL";

            if (numberFilter.HasValue && !string.IsNullOrEmpty(textFilter))
            {
                string? column = numberFilter switch
                {
                    1 => "p.Code",
                    2 => "p.Description",
                    3 => "p.Material",
                    4 => "p.Color",
                    5 => "b.BrandName",
                    6 => "c.CategoryName",
                    _ => null
                };

                if (column != null)
                {
                    filters += $" AND {column} LIKE @Text";
                    parameters.Add("Text", $"%{textFilter}%");
                }
            }

            if (stateFilter.HasValue)
            {
                filters += " AND p.Status = @State";
                parameters.Add("State", stateFilter.Value ? 1 : 0, DbType.Int16);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                filters += " AND p.AuditCreateDate >= @Start AND p.AuditCreateDate < @End";
                parameters.Add("Start", startDate.Value.Date);
                parameters.Add("End", endDate.Value.Date.AddDays(1));
            }

            parameters.Add("Offset", (pageNumber - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var sql = $@"
                        -- A. CONTAR PRODUCTOS ÚNICOS
                            SELECT COUNT(DISTINCT p.IdProduct) 
                            FROM PRODUCTS p 
                            INNER JOIN STORES_INVENTORY si ON p.IdProduct = si.IdProduct
                            LEFT JOIN CATEGORIES c ON p.IdCategory = c.IdCategory
                            LEFT JOIN BRANDS b ON p.IdBrand = b.IdBrand
                            WHERE 1=1 {filters};

                        -- B. OBTENER IDs DE LA PÁGINA ACTUAL
                            DROP TABLE IF EXISTS #PagedProductIds;

                            SELECT DISTINCT p.IdProduct 
                            INTO #PagedProductIds
                            FROM PRODUCTS p
                            INNER JOIN STORES_INVENTORY si ON p.IdProduct = si.IdProduct
                            LEFT JOIN CATEGORIES c ON p.IdCategory = c.IdCategory
                            LEFT JOIN BRANDS b ON p.IdBrand = b.IdBrand
                            WHERE 1=1 {filters}
                            ORDER BY p.IdProduct DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                        -- C. DATA FINAL
                            SELECT  p.Image, p.Code,p.Description, 
                                    p.Material, p.Color,b.BrandName, 
                                    c.CategoryName, p.AuditCreateDate,
                            STRING_AGG(CONCAT(si.IdStore, ':', si.StockAvailable), ',') AS StoreStocks
                            FROM PRODUCTS p
                            INNER JOIN #PagedProductIds ppi ON p.IdProduct = ppi.IdProduct
                            INNER JOIN STORES_INVENTORY si ON p.IdProduct = si.IdProduct
                            LEFT JOIN CATEGORIES c ON p.IdCategory = c.IdCategory
                            LEFT JOIN BRANDS b ON p.IdBrand = b.IdBrand
                            GROUP BY 
                                    p.IdProduct, p.Image, p.Code, p.Description,
                                    p.Material, p.Color, b.BrandName, c.CategoryName, p.AuditCreateDate
                            ORDER BY p.IdProduct DESC;

                            DROP TABLE IF EXISTS #PagedProductIds;";

            using var connection = new SqlConnection(_connectionString);
            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var total = await multi.ReadFirstAsync<int>();
            var data = (await multi.ReadAsync<InventoryPivotModel>()).ToList();

            return (data, total);
        }

        public IQueryable<StoreInventoryEntity> GetStocksByStoreAsQueryable(int storeId)
        {
            return _context.StoreInventory
                .Where(s => s.IdStore == storeId);
        }

        public IQueryable<StoreInventoryEntity> GetStockByIdAsQueryable(int productId, int storeId)
        {
            return _context.StoreInventory
                .Where(x => x.IdProduct == productId && x.IdStore == storeId);
        }

        public async Task AddStoreInventoryAsync(StoreInventoryEntity entity)
        {
            await _context.AddAsync(entity);
        }

    }
}
