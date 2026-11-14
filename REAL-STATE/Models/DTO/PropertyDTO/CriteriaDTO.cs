namespace REAL_STATE.Models.DTO.PropertyDTO
{
    public class CriteriaDTO
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public string? ListingType { get; set; }
        public string? City { get; set; }
        public string? PropertyType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinFloorArea { get; set; }
    }
}
