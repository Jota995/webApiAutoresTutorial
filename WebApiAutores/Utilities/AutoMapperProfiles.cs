using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entity;

namespace WebApiAutores.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreateDTO, Autor>();
            CreateMap<Autor, AutorDTO>();

            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(autorDTO => autorDTO.Libros, options => options.MapFrom(MapAutorDTOLibros));

            CreateMap<CreateLibroDTO, Libro>()
                .ForMember(libro => libro.AutoresLibros, options => options.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(libroDTO => libroDTO.Autores, options => options.MapFrom(MapLibroDTOAutores));

            CreateMap<LibroPatchDTO, Libro>().ReverseMap();

            CreateMap<CreateComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();

        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();

            if(autor.AutoresLibros == null)
            {
                return resultado;
            }

            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }

            return resultado;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();

            if(libro.AutoresLibros == null)
            {
                return resultado;
            }

            foreach (var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }

            return resultado;
        }

        private List<AutorLibro> MapAutoresLibros(CreateLibroDTO createLibroDTO, Libro libro)
        {
            var resultado  = new List<AutorLibro>();

            if(createLibroDTO.AutoresIds == null)
            {
                return resultado;
            }

            foreach(var autorId in createLibroDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro 
                {
                    AutorId = autorId,
                });
            }

            return resultado;
        }
    }
}
