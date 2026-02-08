namespace Utilities.Static
{
    public class ExcelColumnNames
    {
        public static List<ExcelTableColumn> GetColumns(IEnumerable<(string ColumnName, string PropertyName)> columnsProperties)
        {
            var columns = new List<ExcelTableColumn>();

            foreach (var (ColumnName, PropertyName) in columnsProperties)
            {
                var column = new ExcelTableColumn()
                {
                    Label = ColumnName,
                    PropertyName = PropertyName
                };

                columns.Add(column);
            }

            return columns;
        }

        #region ColumnsBrand
        public static List<(string ColumnName, string PropertyName)> GetColumnsBrands()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("MARCA", "BrandName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusBrand")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsCategory
        public static List<(string ColumnName, string PropertyName)> GetColumnsCategories()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("CATEGORÍA", "CategoryName"),
                ("DESCRIPCIÓN", "Description"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusCategory")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsCustomer
        public static List<(string ColumnName, string PropertyName)> GetColumnsCustomers()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("NOMBRE", "Names"),
                ("APELLIDOS", "LastNames"),
                ("CARNET", "IdentificationNumber"),
                ("TELÉFONO", "PhoneNumber"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusCustomer")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsGoodsIssue
        public static List<(string ColumnName, string PropertyName)> GetColumnsGoodsIssue()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("CÓDIGO", "Code"),
                ("TIPO", "Type"),
                ("PERSONAL", "UserName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusIssue")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsGoodsReceipt
        public static List<(string ColumnName, string PropertyName)> GetColumnsGoodsReceipt()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("CÓDIGO", "Code"),
                ("TIPO", "Type"),
                ("PROVEEDOR", "CompanyName"),
                ("FECHA DEL DOCUMENTO", "DocumentDate"),
                ("TIPO DE DOCUMENTO", "DocumentType"),
                ("NÚMERO DE DOCUMENTO", "DocumentNumber"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusReceipt")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsStoreInventory
        public static List<(string ColumnName, string PropertyName)> GetColumnsInventories()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("CÓDIGO", "Code"),
                ("DESCRIPCIÓN", "Description"),
                ("MATERIAL", "Material"),
                ("COLOR", "Color"),
                ("UNIDAD DE MEDIDA", "UnitMeasure"),
                ("CATEGORÍA", "CategoryName"),
                ("MARCA", "BrandName"),
                ("CANT. DISPONIBLE", "StockAvailable"),
                ("CANT. EN TRANSITO", "StockInTransit"),
                ("PRECIO", "Price")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsModule
        public static List<(string ColumnName, string PropertyName)> GetColumnsModules()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("MÓDULO", "ModuleName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusModule")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsProduct
        public static List<(string ColumnName, string PropertyName)> GetColumnsProducts()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("CÓDIGO", "Code"),
                ("DESCRIPCIÓN", "Description"),
                ("MATERIAL", "Material"),
                ("COLOR", "Color"),
                ("UNIDAD DE MEDIDA", "UnitMeasure"),
                ("CATEGORÍA", "CategoryName"),
                ("MARCA", "BrandName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusProduct")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsRole
        public static List<(string ColumnName, string PropertyName)> GetColumnsRoles()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("ROL", "RoleName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusRole")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsStore
        public static List<(string ColumnName, string PropertyName)> GetColumnsStores()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("TIENDA", "StoreName"),
                ("ENCARGADO", "Manager"),
                ("DIRECCIÓN", "Address"),
                ("TELÉFONO", "PhoneNumber"),
                ("CORREO", "Email"),
                ("TIPO", "Type"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusStore")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsSupplier
        public static List<(string ColumnName, string PropertyName)> GetColumnsSuppliers()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("EMPRESA", "CompanyName"),
                ("CONTACTO", "Contact"),
                ("TELÉFONO", "PhoneNumber"),
                ("CORREO", "Email"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusSupplier")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsTransfer
        public static List<(string ColumnName, string PropertyName)> GetColumnsTransfer()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("CÓDIGO", "Code"),
                ("FECHA ENVIO", "SendDate"),
                ("FECHA RECEPCIÓN", "ReceiveDate"),
                ("ORIGEN", "StoreOrigin"),
                ("DESTINO", "StoreDestination"),
                ("ESTADO", "StatusTransfer")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsUser
        public static List<(string ColumnName, string PropertyName)> GetColumnsUsers()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("USUARIO", "UserName"),
                ("NOMBRES", "Names"),
                ("APELLIDOS", "LastNames"),
                ("CARNET", "IdentificationNumber"),
                ("TELÉFONO", "PhoneNumber"),
                ("ROL", "RoleName"),
                ("TIENDA", "StoreName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusUser")
            };

            return columnsProperties;
        }
        #endregion
    }
}
