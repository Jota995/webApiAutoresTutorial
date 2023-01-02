using WebApiAutores.DTOs;

namespace WebApiAutores.Utilities
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginateDTO paginateDTO)
        {
            return queryable
                .Skip((paginateDTO.Page - 1) * paginateDTO.RecordsPerPage)
                .Take(paginateDTO.RecordsPerPage);
        }
    }
}
