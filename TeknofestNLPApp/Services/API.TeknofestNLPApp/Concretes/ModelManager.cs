using API.TeknofestNLPApp.Dtos;
using API.TeknofestNLPApp.Dtos.ModelDtos;
using API.TeknofestNLPApp.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace API.TeknofestNLPApp.Concretes
{
    public class ModelManager : IModelService
    {
        private readonly IConfiguration _configuration;
        public ModelManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<CommentDto>> GetLabeledCommentFromModel(List<CommentDto> dtos)
        {
            // İstek yapılacak URL
            string url = _configuration["AppSettings:ApiUrl"] ?? "";
            string apiUrl = url + "/api/comments";
            try
            {
                var jsonContent = JsonConvert.SerializeObject(dtos);

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(apiUrl),
                        Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                    };

                    var response = await client.SendAsync(request);

                    // Başarılı ise içeriği oku
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        // JSON yanıtını dönüştür
                        var processedDtos = JsonConvert.DeserializeObject<ModelResultDto>(responseBody);

                        if (processedDtos != null)
                        {
                            return processedDtos.comments;
                        }
                    }
                    else
                    {

                        Console.WriteLine($"Hata kodu:");
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"HTTP isteği sırasında hata oluştu: {e.Message}");
            }

            // Hata durumunda veya işlem başarısız olduğunda orijinal veriyi geri döndür
            return dtos;
        }
    }
}
