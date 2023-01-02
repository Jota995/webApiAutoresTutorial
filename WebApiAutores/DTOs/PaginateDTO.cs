namespace WebApiAutores.DTOs
{
    public class PaginateDTO
    {
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        private readonly int maxSizePerPage = 50;

        public int RecordsPerPage
        {
            get { return recordsPerPage; }
            set { recordsPerPage = (value > maxSizePerPage) ? maxSizePerPage : value; }
        }
    }
}
