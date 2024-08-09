namespace API.TeknofestNLPApp.Dtos.ModelDtos
{
    public class ModelResultDto
    {
        public List<CommentDto> comments { get; set; }
        public string message { get; set; }
        public string error { get; set; }
    }
}
