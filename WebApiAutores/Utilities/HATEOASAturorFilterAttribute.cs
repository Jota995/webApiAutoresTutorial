using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAutores.DTOs;
using WebApiAutores.Services;

namespace WebApiAutores.Utilities
{
    public class HATEOASAturorFilterAttribute : HATEOASFilterAttribute
    {
        private readonly LinksGenerate linksGenerate;

        public HATEOASAturorFilterAttribute(LinksGenerate linksGenerate)
        {
            this.linksGenerate = linksGenerate;
        }
        
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var shouldBeInclude = ShouldBeIncludeHATEOAS(context);

            if(!shouldBeInclude) 
            {
                await next();

                return;
            }

            var result = context.Result as ObjectResult;

            var autorDTO = result.Value as AutorDTO;
            if(autorDTO == null) 
            {
                var autoresDTO = result.Value as List<AutorDTO> 
                    ?? throw new ArgumentException("se esperaba una instancia de AutorDTO o List<AutorDTO>");

                autoresDTO.ForEach(async autor => await linksGenerate.GenerateLinks(autor));
                result.Value = autoresDTO;
            }
            else
            {
                await linksGenerate.GenerateLinks(autorDTO);
            }

            await next();
        }
    }
}
