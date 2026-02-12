namespace Utilities.Static
{
    public class PdfColumnNames
    {
        public static List<PdfTableColumn> GetColumns(IEnumerable<(string ColumnName, string PropertyName)> columnsProperties)
        {
            var columns = new List<PdfTableColumn>();
            foreach (var (ColumnName, PropertyName) in columnsProperties)
            {
                var column = new PdfTableColumn()
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
                ("Marca", "BrandName"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusBrand")
            };
            return columnsProperties;
        }
        #endregion

        #region ColumnsCategory
        public static List<(string ColumnName, string PropertyName)> GetColumnsCategories()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("Categoría", "CategoryName"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusCategory")
            };
            return columnsProperties;
        }
        #endregion

        #region ColumnsCustomer
        public static List<(string ColumnName, string PropertyName)> GetColumnsCustomers()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("Nombre", "Names"),
                ("Apellidos", "LastNames"),
                ("Carnet", "IdentificationNumber"),
                ("Teléfono", "PhoneNumber"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusCustomer")
            };
            return columnsProperties;
        }
        #endregion

        #region ColumnsGoodsIssue
        public static List<(string ColumnName, string PropertyName)> GetColumnsGoodsIssue()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("Código", "Code"),
                ("Tipo", "Type"),
                ("Personal", "UserName"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusIssue")
            };
            return columnsProperties;
        }
        #endregion

        #region ColumnsGoodsReceipt
        public static List<(string ColumnName, string PropertyName)> GetColumnsGoodsReceipt()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("Código", "Code"),
                ("Tipo", "Type"),
                ("Proveedor", "CompanyName"),
                ("Fecha doc.", "DocumentDate"),
                ("Tipo doc.", "DocumentType"),
                ("Número doc.", "DocumentNumber"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusReceipt")
            };
            return columnsProperties;
        }
        #endregion

        #region ColumnsStoreInventory
        public static List<(string ColumnName, string PropertyName)> GetColumnsInventories()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("Código", "Code"),
                ("Descripción", "Description"),
                ("Material", "Material"),
                ("Color", "Color"),
                ("Uni. de medida", "UnitMeasure"),
                ("Categoría", "CategoryName"),
                ("Marca", "BrandName"),
                ("Cant. disponible", "StockAvailable"),
                ("Cant. transito", "StockInTransit"),
                ("Precio", "Price")
            };
            return columnsProperties;
        }
        #endregion

        #region ColumnsModule
        public static List<(string ColumnName, string PropertyName)> GetColumnsModules()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("Módulo", "ModuleName"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusModule")

            };
            return columnsProperties;
        }
        #endregion

        #region ColumnsProduct
        public static List<(string ColumnName, string PropertyName)> GetColumnsProducts()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("Código", "Code"),
                ("Descripción", "Description"),
                ("Material", "Material"),
                ("Color", "Color"),
                ("U. de medida", "UnitMeasure"),
                ("Categoría", "CategoryName"),
                ("Marca", "BrandName"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusProduct")

            };
            return columnsProperties;
        }
        #endregion

        #region ColumnsRole
        public static List<(string ColumnName, string PropertyName)> GetColumnsRoles()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("Rol", "RoleName"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusRole")

            };
            return columnsProperties;
        }
        #endregion

        #region ColumnsStore
        public static List<(string ColumnName, string PropertyName)> GetColumnsStores()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("Tienda", "StoreName"),
                ("Encargado", "Manager"),
                ("Direcciósn", "Address"),
                ("Teléfono", "PhoneNumber"),
                ("Correo", "Email"),
                ("Tipo", "Type"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusStore")

            };
            return columnsProperties;
        }
        #endregion

        #region ColumnsSupplier
        public static List<(string ColumnName, string PropertyName)> GetColumnsSuppliers()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("Empresa", "CompanyName"),
                ("Contacto", "Contact"),
                ("Teléfono", "PhoneNumber"),
                ("Correo", "Email"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusSupplier")

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
                ("ORIGEN", "StoreOrigin"),
                ("FECHA ENVIO", "SendDate"),
                ("ENVIADO POR", "SendUser"),
                ("DESTINO", "StoreDestination"),
                ("FECHA RECEPCIÓN", "ReceiveDate"),
                ("RECIBIDO POR", "ReceiveUser"),
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
                ("Usuario", "UserName"),
                ("Nombres", "Names"),
                ("Apellidos", "LastNames"),
                ("Carnet", "IdentificationNumber"),
                ("Teléfono", "PhoneNumber"),
                ("Rol", "RoleName"),
                ("Tienda", "StoreName"),
                ("Fecha de creación", "AuditCreateDate"),
                ("Estado", "StatusUser")

            };
            return columnsProperties;
        }
        #endregion
    }
}
