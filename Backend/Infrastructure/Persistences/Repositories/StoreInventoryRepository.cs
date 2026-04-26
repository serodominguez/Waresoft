using Dapper;
using Domain.Entities;
using Domain.Models;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Persistences.Repositories
{
    public class StoreInventoryRepository : IStoreInventoryRepository
    {
        private readonly DbContextSystem _context;

        public StoreInventoryRepository(DbContextSystem context)
        {
            _context = context;
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

            string productFilters = " AND p.AUDIT_DELETE_USER IS NULL AND p.AUDIT_DELETE_DATE IS NULL";

            if (numberFilter.HasValue && !string.IsNullOrEmpty(textFilter))
            {
                string? column = numberFilter switch
                {
                    1 => "p.CODE",
                    2 => "p.DESCRIPTION",
                    3 => "p.MATERIAL",
                    4 => "p.COLOR",
                    5 => "si.PRICE",
                    6 => "b.BRAND_NAME",
                    7 => "c.CATEGORY_NAME",
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
                productFilters += " AND p.STATUS = @State";
                parameters.Add("State", stateFilter.Value ? 1 : 0, DbType.Int16);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                productFilters += " AND p.AUDIT_CREATE_DATE >= @Start AND p.AUDIT_CREATE_DATE < @End";
                parameters.Add("Start", startDate.Value.Date);
                parameters.Add("End", endDate.Value.Date.AddDays(1));
            }

            parameters.Add("Offset", (pageNumber - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var sql = $@"
                            SELECT COUNT(*) 
                            FROM STORES_INVENTORY si 
                            INNER JOIN PRODUCTS p ON si.PK_PRODUCT = p.PK_PRODUCT
                            LEFT JOIN BRANDS b ON p.PK_BRAND = b.PK_BRAND
                            LEFT JOIN CATEGORIES c ON p.PK_CATEGORY = c.PK_CATEGORY
                            WHERE si.PK_STORE = @StoreId {productFilters};

                        -- B. OBTENER IDs DE LA PÁGINA ACTUAL
                            DROP TABLE IF EXISTS #PagedIds;
        
                            SELECT si.PK_PRODUCT INTO #PagedIds
                            FROM STORES_INVENTORY si
                            INNER JOIN PRODUCTS p ON si.PK_PRODUCT = p.PK_PRODUCT
                            LEFT JOIN BRANDS b ON p.PK_BRAND = b.PK_BRAND
                            LEFT JOIN CATEGORIES c ON p.PK_CATEGORY = c.PK_CATEGORY
                            WHERE si.PK_STORE = @StoreId {productFilters}
                            ORDER BY p.PK_PRODUCT DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                        -- C. SELECT FINAL con CTE de movimientos
                            ;WITH TotalMovementCTE AS (
                            SELECT IdProduct, SUM(Quantity) AS Total 
                            FROM (

                        -- Entradas por recepción
                            SELECT d.PK_PRODUCT AS IdProduct, d.QUANTITY AS Quantity 
                            FROM GOODS_RECEIPT_DETAILS d 
                            INNER JOIN GOODS_RECEIPT r ON r.PK_RECEIPT = d.PK_RECEIPT 
                            WHERE r.PK_STORE = @StoreId 
                            AND r.ACTIVE = 1 
                            AND r.STATUS = 1
                            AND d.PK_PRODUCT IN (SELECT PK_PRODUCT FROM #PagedIds)
                            UNION ALL
                
                        -- Salidas por despacho
                            SELECT d.PK_PRODUCT, -d.QUANTITY 
                            FROM GOODS_ISSUE_DETAILS d 
                            INNER JOIN GOODS_ISSUE i ON i.PK_ISSUE = d.PK_ISSUE 
                            WHERE i.PK_STORE = @StoreId 
                            AND i.ACTIVE = 1 
                            AND i.STATUS = 1
                            AND d.PK_PRODUCT IN (SELECT PK_PRODUCT FROM #PagedIds)
                            UNION ALL
                
                        -- Entradas por transferencia (destino)
                            SELECT d.PK_PRODUCT, d.QUANTITY 
                            FROM TRANSFERS_DETAILS d 
                            INNER JOIN TRANSFERS t ON t.PK_TRANSFER = d.PK_TRANSFER 
                            WHERE t.PK_STORE_DESTINATION = @StoreId 
                            AND t.ACTIVE = 1 
                            AND t.STATUS != 0
                            AND d.PK_PRODUCT IN (SELECT PK_PRODUCT FROM #PagedIds)
                            UNION ALL
                
                        -- Salidas por transferencia (origen)
                            SELECT d.PK_PRODUCT, -d.QUANTITY 
                            FROM TRANSFERS_DETAILS d 
                            INNER JOIN TRANSFERS t ON t.PK_TRANSFER = d.PK_TRANSFER 
                            WHERE t.PK_STORE_ORIGIN = @StoreId 
                            AND t.ACTIVE = 1 
                            AND t.STATUS != 0
                            AND d.PK_PRODUCT IN (SELECT PK_PRODUCT FROM #PagedIds)) t GROUP BY IdProduct)
        
                        -- D. SELECT FINAL
                            SELECT  si.PK_STORE AS IdStore, si.PK_PRODUCT AS IdProduct,
                                    si.STOCK_AVAILABLE AS StockAvailable, si.STOCK_IN_TRANSIT AS StockInTransit, si.PRICE AS Price,
                                    p.REPLENISHMENT AS Replenishment, p.CODE AS Code, p.DESCRIPTION AS Description,
                                    p.MATERIAL AS Material, p.COLOR AS Color, p.UNIT_MEASURE AS UnitMeasure,
                                    b.BRAND_NAME AS BrandName, c.CATEGORY_NAME AS CategoryName, p.AUDIT_CREATE_DATE AS AuditCreateDate,
                            COALESCE(cs.Total, 0) AS CalculatedStock
                            FROM STORES_INVENTORY si
                            INNER JOIN #PagedIds pi ON si.PK_PRODUCT = pi.PK_PRODUCT
                            INNER JOIN PRODUCTS p ON si.PK_PRODUCT = p.PK_PRODUCT
                            LEFT JOIN BRANDS b ON p.PK_BRAND = b.PK_BRAND
                            LEFT JOIN CATEGORIES c ON p.PK_CATEGORY = c.PK_CATEGORY
                            LEFT JOIN TotalMovementCTE cs ON si.PK_PRODUCT = cs.IdProduct
                            WHERE si.PK_STORE = @StoreId
                            ORDER BY p.PK_PRODUCT DESC;

                            DROP TABLE IF EXISTS #PagedIds;";

            var connection = _context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(sql, parameters);
            var total = await multi.ReadFirstAsync<int>();
            var data = (await multi.ReadAsync<StoreInventoryModel>()).ToList();

            return (data, total);
        }

        public async Task<List<KardexMovementModel>> GetKardexByProductAsync(int storeId, int productId, DateTime? startDate, DateTime? endDate)
        {
            var sql = @"WITH AllMovements AS (

                                -- ENTRADAS (GoodsReceipt)
                                            SELECT @ProductId AS IdProduct, d.QUANTITY AS Quantity, 
                                                    r.PK_RECEIPT AS IdMovement, r.CODE AS Code, 
                                                    r.AUDIT_CREATE_DATE AS Date, 'Entrada' AS MovementType, r.TYPE AS Type,
                                            CAST(r.STATUS AS VARCHAR(10)) AS State
                                            FROM GOODS_RECEIPT_DETAILS d
                                            INNER JOIN GOODS_RECEIPT r ON r.PK_RECEIPT = d.PK_RECEIPT
                                            WHERE d.PK_PRODUCT = @ProductId 
                                            AND r.PK_STORE = @StoreId 
                                            AND r.ACTIVE = 1 
                                            AND r.STATUS = 1
                                            UNION ALL

                                -- SALIDAS (GoodsIssue)
                                            SELECT @ProductId AS IdProduct, -d.QUANTITY AS Quantity, 
                                                    i.PK_ISSUE AS IdMovement, i.CODE AS Code, 
                                                    i.AUDIT_CREATE_DATE AS Date, 'Salida' AS MovementType, i.TYPE AS Type,
                                            CAST(i.STATUS AS VARCHAR(10)) AS State
                                            FROM GOODS_ISSUE_DETAILS d
                                            INNER JOIN GOODS_ISSUE i ON i.PK_ISSUE = d.PK_ISSUE
                                            WHERE d.PK_PRODUCT = @ProductId 
                                            AND i.PK_STORE = @StoreId 
                                            AND i.ACTIVE = 1 
                                            AND i.STATUS = 1
                                            UNION ALL

                                -- TRASPASOS RECIBIDOS (Destino)
                                            SELECT @ProductId AS IdProduct, d.QUANTITY AS Quantity, 
                                                    t.PK_TRANSFER AS IdMovement, t.CODE AS Code, 
                                                    t.AUDIT_CREATE_DATE AS Date, 'Traspaso' AS MovementType, 'Entrada' AS Type,
                                            CAST(t.STATUS AS VARCHAR(10)) AS State
                                            FROM TRANSFERS_DETAILS d
                                            INNER JOIN TRANSFERS t ON t.PK_TRANSFER = d.PK_TRANSFER
                                            WHERE d.PK_PRODUCT = @ProductId 
                                            AND t.PK_STORE_DESTINATION = @StoreId 
                                            AND t.ACTIVE = 1 
                                            AND t.STATUS != 0
                                            UNION ALL

                                -- TRASPASOS ENVIADOS (Origen)
                                            SELECT @ProductId AS IdProduct, -d.QUANTITY AS Quantity, 
                                                    t.PK_TRANSFER AS IdMovement, t.CODE AS Code, 
                                                    t.AUDIT_CREATE_DATE AS Date, 'Traspaso' AS MovementType, 'Salida' AS Type,
                                            CAST(t.STATUS AS VARCHAR(10)) AS State
                                            FROM TRANSFERS_DETAILS d
                                            INNER JOIN TRANSFERS t ON t.PK_TRANSFER = d.PK_TRANSFER
                                            WHERE d.PK_PRODUCT = @ProductId 
                                            AND t.PK_STORE_ORIGIN = @StoreId 
                                            AND t.ACTIVE = 1 
                                            AND t.STATUS != 0), MovementsWithAccumulated AS (
                                            SELECT IdProduct, Quantity, IdMovement, Code, Date, MovementType, Type, State,
                                            SUM(Quantity) OVER (ORDER BY Date, IdMovement ROWS UNBOUNDED PRECEDING) AS AccumulatedStock
                                            FROM AllMovements)
                                            SELECT IdProduct, Quantity, IdMovement, Code, Date, MovementType, Type, State, AccumulatedStock
                                            FROM MovementsWithAccumulated
                                            WHERE (@StartDate IS NULL OR Date >= @StartDate)
                                            AND (@EndDate IS NULL OR Date <= @EndDate)
                                            ORDER BY Date ASC, IdMovement ASC";

            using var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryAsync<KardexMovementModel>(sql, new { ProductId = productId, StoreId = storeId, StartDate = startDate, EndDate = endDate });
            return result.ToList();
        }

        public async Task<(List<InventoryPivotModel> Data, int TotalRecords)> GetInventoryPivotAsync(int? numberFilter, string? textFilter, bool? stateFilter,DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            var parameters = new DynamicParameters();

            string filters = " AND p.AUDIT_DELETE_USER IS NULL AND p.AUDIT_DELETE_DATE IS NULL";

            if (numberFilter.HasValue && !string.IsNullOrEmpty(textFilter))
            {
                string? column = numberFilter switch
                {
                    1 => "p.CODE",
                    2 => "p.DESCRIPTION",
                    3 => "p.MATERIAL",
                    4 => "p.COLOR",
                    5 => "b.BRAND_NAME",
                    6 => "c.CATEGORY_NAME",
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
                filters += " AND p.STATUS = @State";
                parameters.Add("State", stateFilter.Value ? 1 : 0, DbType.Int16);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                filters += " AND p.AUDIT_CREATE_DATE >= @Start AND p.AUDIT_CREATE_DATE < @End";
                parameters.Add("Start", startDate.Value.Date);
                parameters.Add("End", endDate.Value.Date.AddDays(1));
            }

            parameters.Add("Offset", (pageNumber - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var sql = $@"
                        -- A. CONTAR PRODUCTOS ÚNICOS
                            SELECT COUNT(DISTINCT p.PK_PRODUCT) 
                            FROM PRODUCTS p 
                            INNER JOIN STORES_INVENTORY si ON p.PK_PRODUCT = si.PK_PRODUCT
                            LEFT JOIN CATEGORIES c ON p.PK_CATEGORY = c.PK_CATEGORY
                            LEFT JOIN BRANDS b ON p.PK_BRAND = b.PK_BRAND
                            WHERE 1=1 {filters};

                        -- B. OBTENER IDs DE LA PÁGINA ACTUAL
                            DROP TABLE IF EXISTS #PagedProductIds;

                            SELECT DISTINCT p.PK_PRODUCT 
                            INTO #PagedProductIds
                            FROM PRODUCTS p
                            INNER JOIN STORES_INVENTORY si ON p.PK_PRODUCT = si.PK_PRODUCT
                            LEFT JOIN CATEGORIES c ON p.PK_CATEGORY = c.PK_CATEGORY
                            LEFT JOIN BRANDS b ON p.PK_BRAND = b.PK_BRAND
                            WHERE 1=1 {filters}
                            ORDER BY p.PK_PRODUCT DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                        -- C. DATA FINAL
                            SELECT  p.IMAGE AS Image, p.CODE AS Code,p.DESCRIPTION AS Description, 
                                    p.MATERIAL AS Material, p.COLOR AS Color,b.BRAND_NAME AS BrandName, 
                                    c.CATEGORY_NAME AS CategoryName, p.AUDIT_CREATE_DATE AS AuditCreateDate,
                            STRING_AGG(CONCAT(si.PK_STORE, ':', si.STOCK_AVAILABLE), ',') AS StoreStocks
                            FROM PRODUCTS p
                            INNER JOIN #PagedProductIds ppi ON p.PK_PRODUCT = ppi.PK_PRODUCT
                            INNER JOIN STORES_INVENTORY si ON p.PK_PRODUCT = si.PK_PRODUCT
                            LEFT JOIN CATEGORIES c ON p.PK_CATEGORY = c.PK_CATEGORY
                            LEFT JOIN BRANDS b ON p.PK_BRAND = b.PK_BRAND
                            GROUP BY 
                                    p.PK_PRODUCT, p.IMAGE, p.CODE, p.DESCRIPTION,
                                    p.MATERIAL, p.COLOR, b.BRAND_NAME, c.CATEGORY_NAME, p.AUDIT_CREATE_DATE
                            ORDER BY p.PK_PRODUCT DESC;

                            DROP TABLE IF EXISTS #PagedProductIds;";

            var connection = _context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

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
