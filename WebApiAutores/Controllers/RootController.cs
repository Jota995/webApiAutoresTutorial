using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [Route("api")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHateoas = new List<DatoHATEOAS>();

            var isAdmin = await authorizationService.AuthorizeAsync(User,"isAdmin");

            datosHateoas.Add(new DatoHATEOAS(link: Url.Link("GetRoot", new {}), description: "self", method: "GET"));

            datosHateoas.Add(new DatoHATEOAS(link: Url.Link("ObtenerAutores", new {}),description: "Autores",method: "GET"));

            if (isAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHATEOAS(link: Url.Link("CrearAutor", new { }), description: "Autor-Crear", method: "POST"));
                datosHateoas.Add(new DatoHATEOAS(link: Url.Link("CrearLibro", new { }), description: "Libro-Crear", method: "POST"));
            }
            return datosHateoas;
        }
    }
}
