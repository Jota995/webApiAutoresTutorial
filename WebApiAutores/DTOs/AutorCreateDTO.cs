using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class AutorCreateDTO
    {
        [Required]
        [StringLength(maximumLength: 250)]
        public string Nombre { get; set; }
    }
}
