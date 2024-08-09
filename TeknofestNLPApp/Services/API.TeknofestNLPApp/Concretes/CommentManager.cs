using API.TeknofestNLPApp.Dtos;
using API.TeknofestNLPApp.Dtos.TrendyolDtos;
using API.TeknofestNLPApp.Interfaces;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Xml;
using Result = API.TeknofestNLPApp.Dtos.Result;

namespace API.TeknofestNLPApp.Concretes
{
    public class CommentManager : ICommentService
    {
        private readonly ITrendyolModelService _trendyolModelService;
        private readonly IN11Service _n11Service;
        public CommentManager(ITrendyolModelService trendyolModelService, IN11Service n11Service)
        {
            _n11Service = n11Service;
            _trendyolModelService = trendyolModelService;

        }
        public async Task<ServiceResult> GetProductInComments(CommentRequestDto dto)
        {
            var result = await _trendyolModelService.GetComments(dto);

            if (result.IsSuccess)
            {
                var resultModel = (TrendyolResultDto)result.Data;
                var dtoResult = new CommentResultDto()
                {
                    Data = resultModel.Comments.OrderBy(x => x.Label).ToList(),
                    TotalElements = resultModel.TotalElements > 3030 ? 3030 : resultModel.TotalElements,
                    TotalPages = resultModel.TotalPages > 101 ? 100 : resultModel.TotalPages,
                    TotalUsefullComment = resultModel.Comments.Count(x => x.Label == Common.LabelStatus.UseFull),
                    TotalUnUsefullComment = resultModel.Comments.Count(x => x.Label == Common.LabelStatus.UnUsefull)
                };
                return Result.Success("", 200, 0, dtoResult);
            }

            return Result.Fail(result.Message, 500);

        }

        public async Task<ServiceResult> GetProductInCommentsWithPagination(CommentRequestDto dto)
        {
            var result = await _trendyolModelService.GetCommentsWithPagination(dto);

            if (result.IsSuccess)
            {
                var resultModel = (TrendyolResultDto)result.Data;
                var dtoResult = new CommentResultDto()
                {
                    Data = resultModel.Comments.OrderBy(x => x.Label).ToList(),
                    TotalElements = resultModel.TotalElements > 3030 ? 3030 : resultModel.TotalElements,
                    TotalPages = resultModel.TotalPages > 101 ? 100 : resultModel.TotalPages
                };
                return Result.Success("", 200, 0, dtoResult);
            }

            return Result.Fail(result.Message, 500);
        }
    }
}
