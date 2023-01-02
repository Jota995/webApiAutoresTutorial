
namespace WebApiAutores.DTOs
{
    public class LibroDTO
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        //public List<ComentarioDTO> Comentarios { get; set; }
    }
}
