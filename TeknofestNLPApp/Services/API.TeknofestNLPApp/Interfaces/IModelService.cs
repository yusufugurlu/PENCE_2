using API.TeknofestNLPApp.Dtos;

namespace API.TeknofestNLPApp.Interfaces
{
    public interface IModelService
    {
        Task<List<CommentDto>> GetLabeledCommentFromModel(List<CommentDto> dtos);
    }
}
