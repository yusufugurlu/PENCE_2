namespace API.TeknofestNLPApp.Dtos.TrendyolDtos
{
    public class TrendyolResultDto
    {
        public List<CommentDto> Comments { get; set; }
        public int TotalPages { get; set; }
        public int TotalElements { get; set; }
        public int TotalUsefullComment { get; set; }
        public int TotalUnUsefullComment { get; set; }
    }
}
