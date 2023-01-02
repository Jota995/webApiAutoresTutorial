
namespace WebApiAutores.DTOs
{
    public class AutenticateResponse 
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
