using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Data;
using WebApiAutores.DTOs;
using WebApiAutores.Entity;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/libros/{libroId:Guid}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name ="ObtenerComentariosLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(Guid libroId)
        {
            var comentarios = await context.Comentarios.AsNoTracking().Where(x => x.LibroId == libroId).ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:Guid}", Name = "ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetById(Guid comentarioId)
        {
            var comentario = await context.Comentarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == comentarioId);

            if (comentario == null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost(Name ="CrearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(Guid libroId, CreateComentarioDTO createComentarioDTO)
        {
            var userEmailClaim = HttpContext.User.Claims.Where(x => x.Type == "email").FirstOrDefault();
            var email = userEmailClaim.Value;
            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var libroExist = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!libroExist)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(createComentarioDTO);
            comentario.LibroId = libroId;
            comentario.UserId = userId;

            context.Comentarios.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);
        }

        [HttpPut("{id:Guid}",Name = "ActualizarComentario")]
        public async Task<ActionResult> Put(Guid libroId, Guid id, CreateComentarioDTO createComentarioDTO)
        {
            var libroExist = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!libroExist)
            {
                return NotFound();
            }

            var comentarioExist = await context.Comentarios.AnyAsync(x => x.Id == id);

            if (!comentarioExist)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(createComentarioDTO);

            context.Update(comentario);
            comentario.Id = id;
            comentario.LibroId = libroId;
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
