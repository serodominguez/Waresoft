using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Application.Dtos.Response.StoreInventory
{
    public class StoreInventoryKardexMovementDto
    {
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string? Code { get; set; }
        public string? Date { get; set; }
        public string? Type { get; set; }
        public string? State { get; set; }
        public string? MovementType { get; set; } // ENTRADA o SALIDA
        public int Stock { get; set; } // Stock acumulado después del movimiento
    }
}
