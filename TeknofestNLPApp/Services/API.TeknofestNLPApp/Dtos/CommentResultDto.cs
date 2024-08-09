namespace API.TeknofestNLPApp.Dtos
{
    public class CommentResultDto
    {
        public List<CommentDto> Data { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public int TotalUsefullComment { get; set; }
        public int TotalUnUsefullComment { get; set; }
    }
}
