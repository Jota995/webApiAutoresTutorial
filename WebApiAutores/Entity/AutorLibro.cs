namespace WebApiAutores.Entity
{
    public class AutorLibro
    {
        public Guid LibroId { get; set; }
        public Guid AutorId { get; set; }
        public int Orden { get; set; }
        public Libro Libro { get; set; }
        public Autor Autor { get; set; }
    }
}
