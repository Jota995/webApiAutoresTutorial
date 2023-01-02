using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Data;
using WebApiAutores.DTOs;
using WebApiAutores.Entity;

namespace WebApiAutores.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            _context= context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IList<LibroDTO>>> Get()
        {
            var libros = await _context.Libros
                               .AsNoTracking()
                               .ToListAsync();

            return mapper.Map<List<LibroDTO>>(libros);
        }

        [HttpGet("{id:Guid}",Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get([FromRoute] Guid id)
        {
            var libro = await _context.Libros
                .AsNoTracking()
                .Include(x => x.AutoresLibros)
                .ThenInclude(x => x.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(libro == null) 
            {
                return NotFound();
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost(Name ="CrearLibro")]
        public async Task<ActionResult> Post(CreateLibroDTO createLibroDTO)
        {
            if(createLibroDTO.AutoresIds == null)
            {
                return BadRequest();
            }

            var autoresIds = await _context.Autores.Where(x => createLibroDTO.AutoresIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();

            if(createLibroDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(createLibroDTO);

            AsignarOrdenAutores(libro);

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("ObtenerLibro",new {id = libro.Id}, libroDTO);
        }

        [HttpPut("{id:Guid}",Name ="ActualizarLibro")]
        public async Task<ActionResult> Put(Guid id, CreateLibroDTO createLibroDTO)
        {
            var libroDb = await _context.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(libroDb == null)
            {
                return NotFound();
            }

            libroDb = mapper.Map(createLibroDTO,libroDb);

            AsignarOrdenAutores(libroDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:Guid}",Name ="PatchLibro")]
        public async Task<ActionResult> Patch(Guid id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                return BadRequest();
            }

            var libroDb = await _context.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if(libroDb == null)
            {
                return NotFound();
            }

            var libroDTO = mapper.Map<LibroPatchDTO>(libroDb);

            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO,libroDTO);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:Guid}",Name ="BorrarLibro")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var existe = await _context.Libros.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            _context.Libros.Remove(new Libro() { Id = id });

            await _context.SaveChangesAsync();

            return Ok();
        }


        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
    }
}
