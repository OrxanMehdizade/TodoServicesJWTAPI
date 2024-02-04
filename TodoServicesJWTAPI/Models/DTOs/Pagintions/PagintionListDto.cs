namespace TodoServicesJWTAPI.Models.DTOs.Pagintions
{
    public class PagintionListDto<TModel>
    {
        public IEnumerable<TModel> _Items { get; set; }
        public PagintionMeta _Meta { get; set; }
        public PagintionListDto(IEnumerable<TModel> items, PagintionMeta meta)
        {
            _Items = items;
            _Meta = meta;
        }

    }
}
