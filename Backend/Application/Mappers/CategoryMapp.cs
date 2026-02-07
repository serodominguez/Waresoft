using Application.Dtos.Request.Category;
using Application.Dtos.Response.Category;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class CategoryMapp
    {
        public static CategoryEntity CategoriesMapping(CategoryRequestDto dto)
        {
            return new CategoryEntity
            {
                CategoryName = dto.CategoryName.NormalizeString(),
                Description = dto.Description.NormalizeString()
            };
        }

        public static CategoryResponseDto CategoriesResponseDtoMapping(CategoryEntity entity)
        {
            return new CategoryResponseDto
            {
                IdCategory = entity.Id,
                CategoryName = entity.CategoryName.ToTitleCase(),
                Description = entity.Description.ToTitleCase(),
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusCategory = ((States)(entity.Status ? 1 : 0)).ToString()
            };
        }

        public static CategorySelectResponseDto CategoriesSelectResponseDtoMapping(CategoryEntity entity)
        {
            return new CategorySelectResponseDto
            {
                IdCategory = entity.Id,
                CategoryName = entity.CategoryName.ToTitleCase()
            };
        }
    }
}
