using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Data;
using WebApiAutores.DTOs;
using WebApiAutores.Entity;
using WebApiAutores.Utilities;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [CabezeraEstaPresenteAttribute("x-version","1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Policy = "IsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            _context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerAutores")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAturorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginateDTO paginateDTO)
        {
            var queryable = _context.Autores.AsQueryable();
            await HttpContext.InsertParametersPaginateHeader(queryable);
            var autores = await queryable.OrderBy(autor => autor.Nombre).Paginate(paginateDTO).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id}", Name ="ObtenerAutor")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAturorFilterAttribute))]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<AutorDTOConLibros>> Get([FromRoute] Guid id)
        {
            var autor = await _context.Autores
                .AsNoTracking()
                .Include(x => x.AutoresLibros)
                .ThenInclude(x => x.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(autor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDTOConLibros>(autor);

            return dto;
        }

        [HttpPost(Name ="CrearAutor")]
        public async Task<ActionResult> Post([FromBody] AutorCreateDTO autorCreateDTO)
        {
            var autor = mapper.Map<Autor>(autorCreateDTO);

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("ObtenerAutor",new { id = autor.Id}, autorDTO);
        }

        [HttpPut("{id:Guid}",Name ="ActualizarAutor")]
        public async Task<ActionResult> Put([FromBody] AutorCreateDTO autorCreateDTO, [FromRoute] Guid id)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreateDTO);
            autor.Id = id;

            _context.Update(autor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:Guid}",Name ="BorrarAutor")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            _context.Autores.Remove(new Autor() { Id= id });

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
