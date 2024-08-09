using API.TeknofestNLPApp.Dtos;

namespace API.TeknofestNLPApp.Interfaces
{
    public interface ICommentService
    {
        Task<ServiceResult> GetProductInComments(CommentRequestDto dto);
        Task<ServiceResult> GetProductInCommentsWithPagination(CommentRequestDto dto);
    }
}
