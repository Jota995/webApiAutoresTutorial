using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using WebApiAutores.DTOs;

namespace WebApiAutores.Services
{
    public class LinksGenerate
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public LinksGenerate(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        public async Task GenerateLinks(AutorDTO autorDTO)
        {
            var isAdmin = await IsAdmin();
            var Url = BuildURLHelper();
            autorDTO.Links.Add(new DatoHATEOAS(link: Url.Link("ObtenerAutor", new { id = autorDTO.Id }), description: "self", method: "GET"));

            if (isAdmin)
            {
                autorDTO.Links.Add(new DatoHATEOAS(link: Url.Link("ActualizarAutor", new { id = autorDTO.Id }), description: "autor-actualizar", method: "PUT"));
                autorDTO.Links.Add(new DatoHATEOAS(link: Url.Link("BorrarAutor", new { id = autorDTO.Id }), description: "self", method: "DELETE"));
            }
        }

        private async Task<bool> IsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var result = await authorizationService.AuthorizeAsync(httpContext.User, "isAdmin");
            return result.Succeeded;
        }
        private IUrlHelper BuildURLHelper()
        {
            var factory = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(actionContextAccessor.ActionContext);
        }
    }

    
}
