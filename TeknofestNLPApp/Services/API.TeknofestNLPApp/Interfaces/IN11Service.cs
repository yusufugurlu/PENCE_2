using API.TeknofestNLPApp.Dtos;

namespace API.TeknofestNLPApp.Interfaces
{
    public interface IN11Service
    {
        Task<ServiceResult> GetComments(CommentRequestDto dto, string productName);
    }
}
