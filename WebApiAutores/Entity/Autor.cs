using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiAutores.Entity
{
    public class Autor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(maximumLength:250)]
        public string Nombre { get; set; }
        public IList<Libro> Libros { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }

    }
}
