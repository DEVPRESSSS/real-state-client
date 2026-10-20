namespace REAL_STATE.Models.DTO.PropertyDTO
{
    public class CriteriaDTO
    {
        public string? ListingType { get; set; }   
        public string? City { get; set; }
        public string? PropertyType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinFloorArea { get; set; }
    }
}
