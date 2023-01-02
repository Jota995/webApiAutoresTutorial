using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class CreateLibroDTO
    {
        [StringLength(maximumLength: 500)]
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public List<Guid> AutoresIds { get; set; }
    }
}
