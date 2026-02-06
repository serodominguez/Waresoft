using Application.Commons.Bases.Request;

namespace Application.Commons.Ordering
{
    public static class PaginateQuery
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, BasePaginationRequest request)
        {
            return queryable.Skip((request.NumberPage - 1) * request.Records).Take(request.Records);
        }
    }
}
