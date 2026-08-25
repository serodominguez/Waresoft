using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos.Request.InventoryPeriod
{
    public record InventoryPeriodPhysicalCountDto
    {
        public int IdProduct { get; init; }
        public int? PhysicalStock { get; init; }
    }
}
