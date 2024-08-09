using API.TeknofestNLPApp.Dtos;
using API.TeknofestNLPApp.Dtos.TrendyolDtos;

namespace API.TeknofestNLPApp.Interfaces
{
    public interface ITrendyolModelService
    {
        Task<ServiceResult> GetComments(CommentRequestDto dto);
        Task<ServiceResult> GetCommentsWithPagination(CommentRequestDto dto);
        Task<TrendyolProductInformationDto> GetSellerId(string url);
    }
}
