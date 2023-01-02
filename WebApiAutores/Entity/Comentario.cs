using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entity
{
    public class Comentario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Contenido { get; set; }
        public string UserId { get; set; }
        public IdentityUser Usuario { get; set; }
        public Guid LibroId { get; set; }
        public Libro Libro { get; set; }
    }
}
