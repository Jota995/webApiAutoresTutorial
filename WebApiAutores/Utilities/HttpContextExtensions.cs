using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task InsertParametersPaginateHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if(httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadTotalRegistros", cantidad.ToString());
        }
    }
}
