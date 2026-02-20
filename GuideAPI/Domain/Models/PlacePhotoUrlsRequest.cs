namespace GuideAPI.Domain.Models
{
    public class PlacePhotoUrlsRequest
    {
        public string PlaceId { get; set; } = string.Empty;
        public int? MaxHeightPx { get; set; }
        public int? MaxWidthPx { get; set; }

    }
}
