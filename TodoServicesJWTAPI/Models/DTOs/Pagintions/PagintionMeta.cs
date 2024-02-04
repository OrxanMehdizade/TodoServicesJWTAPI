namespace TodoServicesJWTAPI.Models.DTOs.Pagintions
{
    public class PagintionMeta
    {

        public int _Page { get; set; }
        public int _PageSize { get; set; }
        public int _TotalPage { get; set; }

        public PagintionMeta(int page, int pageSize, int itemCount)
        {
            _Page = page;
            _PageSize = pageSize;
            _TotalPage = (itemCount + pageSize - 1) / pageSize;
        }

    }
}
