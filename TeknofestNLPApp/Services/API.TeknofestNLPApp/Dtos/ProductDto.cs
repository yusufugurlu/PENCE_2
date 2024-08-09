namespace API.TeknofestNLPApp.Dtos
{
    public class ProductDto
    {
        public ProductDto()
        {
            Comments = new List<CommentDto>();
        }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ProductTitle { get; set; }
        public double Price { get; set; }
        public List<CommentDto> Comments { get; set; }
        public string CategoryName { get; set; }
    }
}
