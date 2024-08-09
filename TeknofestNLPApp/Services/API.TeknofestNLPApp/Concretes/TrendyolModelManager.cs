using API.TeknofestNLPApp.Dtos;
using API.TeknofestNLPApp.Dtos.ModelDtos;
using API.TeknofestNLPApp.Dtos.TrendyolDtos;
using API.TeknofestNLPApp.Interfaces;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Result = API.TeknofestNLPApp.Dtos.Result;

namespace API.TeknofestNLPApp.Concretes
{
    public class TrendyolModelManager : ITrendyolModelService
    {
        private readonly IModelService _modelService;
        public TrendyolModelManager(IModelService modelService)
        {
            _modelService = modelService;
        }

        private string GetContent(string url)
        {
            Regex regex = new Regex(@"-p-(\d+)");
            Match match = regex.Match(url);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return "Geçersiz URL";
            }
        }

        private string GetSellerIdFromScriptContent(string scriptContent)
        {
            Regex regex = new Regex(@"sellerId=(\d+)");
            Match match = regex.Match(scriptContent);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return "";
        }

        private string GetProductName(string scriptContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(scriptContent);

            var firstDiv = document.DocumentNode.SelectSingleNode("//div[@class='pr-in-cn']");
            if (firstDiv != null)
            {
                // Bu <div> içinde yer alan <span> etiketini bul
                var spanNode = firstDiv.SelectSingleNode(".//span");
                if (spanNode != null)
                {
                    string spanContent = spanNode.InnerText;
                    return spanContent;
                }
                else
                {
                    Console.WriteLine("Span etiketi bulunamadı.");
                }
            }

            return "";
        }

        public async Task<TrendyolProductInformationDto> GetSellerId(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string html = await response.Content.ReadAsStringAsync();

                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(html);

                        HtmlNodeCollection scriptNodes = doc.DocumentNode.SelectNodes("//script");

                        if (scriptNodes != null)
                        {
                            foreach (HtmlNode scriptNode in scriptNodes)
                            {
                                string scriptContent = scriptNode.GetAttributeValue("src", "");

                                string sellerId = GetSellerIdFromScriptContent(html);
                                var productName = GetProductName(html);
                                if (!string.IsNullOrEmpty(sellerId))
                                {
                                    return new TrendyolProductInformationDto() {SellerId = sellerId,ProductName = productName };
                                }
                            }
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("HTTP isteği sırasında hata oluştu: " + e.Message);
                }
            }
            return new TrendyolProductInformationDto();
        }

        public async Task<ServiceResult> GetComments(CommentRequestDto dto)
        {
            var globalUrl = dto.Url;

            // Base URL
            string baseUrl = $"https://public-mdc.trendyol.com/discovery-web-websfxsocialreviewrating-santral/product-reviews-detailed";

            string contentId = GetContent(globalUrl);

            var productInformation = await GetSellerId(globalUrl);

            List<CommentDto> result = new List<CommentDto>();
            string order = "DESC";
            string orderBy = "Score";
            string channelId = "1";
            string size = "100";

            int totalPages = int.MaxValue;
            int totalElements = 0;
            int currentPage = 0;


            // Tüm sayfaları dolaşacak şekilde istek yapma
            while (currentPage < totalPages)
            {
                string page = currentPage.ToString();

                string url = $"{baseUrl}?sellerId={productInformation.SellerId}&contentId={contentId}&page={page}&order={order}&orderBy={orderBy}&channelId={channelId}&size={size}";

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();

                            var serializeDto = JsonConvert.DeserializeObject<TrendyolSerializeDto>(content);
                            totalPages = (serializeDto?.result?.productReviews?.totalElements / 30) ?? 0;
                            totalElements = serializeDto?.result?.productReviews?.totalElements ?? 0;

                            if (serializeDto?.result?.productReviews?.content != null)
                            {
                                foreach (var item in serializeDto.result.productReviews.content)
                                {
                                    result.Add(new CommentDto()
                                    {
                                        CommentTitle = item.commentTitle,
                                        Rate = item.rate,
                                        Id = item.id,
                                        Comment = item.comment,
                                        CommentDateISOtype = item.commentDateISOtype,
                                        LastModifiedDate = item.lastModifiedDate,
                                        ReviewLikeCount = item.reviewLikeCount,
                                        UserLiked = item.userLiked,
                                        SellerName = item.sellerName,
                                        UserFullName = item.userFullName,
                                    });
                                }
                            }

                            await Task.Delay(100);
                            if (currentPage == 100)
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"HTTP isteği başarısız. Durum kodu: {response.StatusCode}");
                            return Result.Fail($"HTTP isteği başarısız. Durum kodu: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                        return Result.Fail(ex.Message);
                    }
                }

                currentPage++;
            }

            var resultOfComment = await _modelService.GetLabeledCommentFromModel(result);

            TrendyolResultDto trendyolResultDto= new TrendyolResultDto()
            {
                Comments = resultOfComment,
                TotalPages = totalPages,
                TotalElements= totalElements,
                TotalUsefullComment = resultOfComment.Count(x=>x.Label == Common.LabelStatus.UseFull),
                TotalUnUsefullComment= resultOfComment.Count(x => x.Label == Common.LabelStatus.UnUsefull)
            };
            return Result.Success("",200,0, trendyolResultDto);
        }

        public async Task<ServiceResult> GetCommentsWithPagination(CommentRequestDto dto)
        {
            var globalUrl = dto.Url;

            // Base URL
            string baseUrl = $"https://public-mdc.trendyol.com/discovery-web-websfxsocialreviewrating-santral/product-reviews-detailed";

            string contentId = GetContent(globalUrl);

            var productInformation = await GetSellerId(globalUrl);

            List<CommentDto> result = new List<CommentDto>();
            string order = "DESC";
            string orderBy = "Score";
            string channelId = "1";
            string size = "100";

            int totalPages = int.MaxValue;
            int totalElements = 0;



            string page = (dto.Page - 1).ToString();

            string url = $"{baseUrl}?sellerId={productInformation.SellerId}&contentId={contentId}&page={page}&order={order}&orderBy={orderBy}&channelId={channelId}&size={size}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        var serializeDto = JsonConvert.DeserializeObject<TrendyolSerializeDto>(content);
                        totalPages = (serializeDto?.result?.productReviews?.totalElements / 30) ?? 0;
                        totalElements = serializeDto?.result?.productReviews?.totalElements ?? 0;

                        if (serializeDto?.result?.productReviews?.content != null)
                        {
                            foreach (var item in serializeDto.result.productReviews.content)
                            {
                                result.Add(new CommentDto()
                                {
                                    CommentTitle = item.commentTitle,
                                    Rate = item.rate,
                                    Id = item.id,
                                    Comment = item.comment,
                                    CommentDateISOtype = item.commentDateISOtype,
                                    LastModifiedDate = item.lastModifiedDate,
                                    ReviewLikeCount = item.reviewLikeCount,
                                    UserLiked = item.userLiked,
                                    SellerName = item.sellerName,
                                    UserFullName = item.userFullName,
                                });
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"HTTP isteği başarısız. Durum kodu: {response.StatusCode}");
                        return Result.Fail($"HTTP isteği başarısız. Durum kodu: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                    return Result.Fail(ex.Message);
                }
            }



            var resultOfComment = await _modelService.GetLabeledCommentFromModel(result);

            TrendyolResultDto trendyolResultDto = new TrendyolResultDto()
            {
                Comments = resultOfComment,
                TotalPages = totalPages,
                TotalElements = totalElements
            };
            return Result.Success("", 200, 0, trendyolResultDto);

        }
    }
}
