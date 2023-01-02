namespace WebApiAutores.DTOs
{
    public class ResourcesColections<T> : Resource where T : Resource
    {
        public List<T> Values { get; set; }
    }
}
