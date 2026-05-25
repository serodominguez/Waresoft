using Application.Dtos.Request.Category;
using Application.Dtos.Response.Category;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Category;
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

        public static CategoryResponseDto CategoriesResponseDtoMapping(CategoryReadModel model)
        {
            return new CategoryResponseDto
            {
                IdCategory = model.Id,
                CategoryName = model.CategoryName.ToSentenceCase(),
                Description = model.Description.ToSentenceCaseMultiple(),
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusCategory = ((States)(model.IsActive ? 1 : 0)).ToString()
            };
        }

        public static CategorySelectResponseDto CategoriesSelectResponseDtoMapping(CategorySelectReadModel model)
        {
            return new CategorySelectResponseDto
            {
                IdCategory = model.Id,
                CategoryName = model.CategoryName.ToSentenceCase()
            };
        }
    }
}
