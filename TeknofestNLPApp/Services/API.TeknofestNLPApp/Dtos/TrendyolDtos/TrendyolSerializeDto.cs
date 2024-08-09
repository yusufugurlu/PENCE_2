namespace API.TeknofestNLPApp.Dtos.TrendyolDtos
{

    public class Content
    {
        public bool isElite { get; set; }
        public bool isInfluencer { get; set; }
        public string commentDateISOtype { get; set; }
        public string userFullName { get; set; }
        public int id { get; set; }
        public int rate { get; set; }
        public string commentTitle { get; set; }
        public string comment { get; set; }
        public bool trusted { get; set; }
        public string lastModifiedDate { get; set; }
        public string sellerName { get; set; }
        public int reviewLikeCount { get; set; }
        public bool userLiked { get; set; }
        public ProductAttributes? productAttributes { get; set; }
        public List<MediaFile> mediaFiles { get; set; }
    }

    public class ContentSummary
    {
        public List<object>? productSizes { get; set; }
        public List<RatingCount>? ratingCounts { get; set; }
        public List<Tag>? tags { get; set; }
        public bool userCommentExist { get; set; }
        public string imageUrl { get; set; }
        public double averageRating { get; set; }
        public int totalCommentCount { get; set; }
        public int totalRatingCount { get; set; }
        public bool isSearchable { get; set; }
        public string description { get; set; }
        public List<HeightTag>? heightTags { get; set; }
        public List<WeightTag>? weightTags { get; set; }
    }

    public class HeightTag
    {
        public string name { get; set; }
        public int count { get; set; }
    }

    public class ImageSummary
    {
        public int id { get; set; }
        public string userFullName { get; set; }
        public int rate { get; set; }
        public string comment { get; set; }
        public bool trusted { get; set; }
        public string sellerName { get; set; }
        public MediaFile? mediaFile { get; set; }
        public string lastModifiedDate { get; set; }
        public ProductAttributes? productAttributes { get; set; }
    }

    public class MediaFile
    {
        public int id { get; set; }
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
        public string mediaType { get; set; }
        public int height { get; set; }
        public int weight { get; set; }
    }

    public class MediaFile2
    {
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
    }

    public class ProductAttributes
    {
        public string height { get; set; }
        public string weight { get; set; }
    }

    public class ProductReviews
    {
        public int totalElements { get; set; }
        public int totalPages { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public List<Content>? content { get; set; }
        public bool userCommentExist { get; set; }
    }

    public class RatingCount
    {
        public int rate { get; set; }
        public int count { get; set; }
        public int commentCount { get; set; }
    }

    public class Result
    {
        public ContentSummary? contentSummary { get; set; }
        public ProductReviews? productReviews { get; set; }
        public List<ImageSummary>? imageSummary { get; set; }
    }

    public class TrendyolSerializeDto
    {
        public bool isSuccess { get; set; }
        public int statusCode { get; set; }
        public Result? result { get; set; }
        public string error { get; set; }
    }

    public class TrendyolSerialize2Dto
    {
        public bool isSuccess { get; set; }
        public int statusCode { get; set; }
        public List<Content>? result { get; set; }
        public string error { get; set; }
    }
    public class Tag
    {
        public string name { get; set; }
        public int count { get; set; }
        public string imageUrl { get; set; }
    }

    public class WeightTag
    {
        public string name { get; set; }
        public int count { get; set; }
    }
}
