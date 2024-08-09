using API.TeknofestNLPApp.Dtos;
using API.TeknofestNLPApp.Dtos.N11Dtos;
using API.TeknofestNLPApp.Dtos.TrendyolDtos;
using API.TeknofestNLPApp.Interfaces;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Net.Http.Headers;
using System.Web;
using Result = API.TeknofestNLPApp.Dtos.Result;

namespace API.TeknofestNLPApp.Concretes
{
    public class N11Manager : IN11Service
    {
        public async Task<ServiceResult> GetComments(CommentRequestDto dto, string productName)
        {
            var products = await GetProducts(productName);

            foreach (var product in products)
            {
                var rr = await GetCommentsByProduct(product);
            }

            return Result.Fail("");
        }


        public async Task<List<CommentDto>> GetCommentsByProduct(N11ProductDto dto)
        {
            List<CommentDto> dtos = new();

            string url = $"https://www.n11.com/component/render/groupProductReviews?page=2&productId={dto.ProductId}&tag=t%C3%BCm%C3%BC";

            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("--headless"); // Tarayıcıyı başlatırken arayüzü göstermemek için
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            string pageSource = "";
            using (IWebDriver driver = new ChromeDriver(options))
            {
                try
                {
                    driver.Navigate().GoToUrl(url);

                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);
                    pageSource = driver.PageSource;
                }
                catch (NoSuchElementException e)
                {
                    Console.WriteLine("\nElement not found!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }


            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageSource);

            // ul etiketinin altındaki li etiketlerini seçin
            var paginationNode = doc.DocumentNode.SelectSingleNode("//div[@class='pagination']");
            var aNodes = paginationNode.SelectNodes("//a[@class='pageLink']");
            if (aNodes.Count > 0)
            {
                var page = aNodes.LastOrDefault()?.InnerText ?? "0";
            }


            var liNodes = doc.DocumentNode.SelectNodes("//li[@class='comment']");
            foreach (var li in liNodes)
            {
                int rate = 0;
                var commentDate = li.SelectSingleNode(".//span[@class='commentDate']").InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                var comment = li.SelectSingleNode("p")?.InnerText ?? "";

                var rating = doc.DocumentNode.SelectSingleNode("//div[@class='ratingCont']");
                var classRating = rating.SelectSingleNode("span").GetAttributes("class", "");

                if (classRating.Any())
                {
                    foreach (var item in classRating)
                    {
                        if (item.Value.Contains("rating"))
                        {
                            string rateString = item.Value.Replace("rating", "").Replace("r", "").Replace(" ", "");

                            switch (rateString)
                            {
                                case "20":
                                    rate = 1;
                                    break;
                                case "40":
                                    rate = 2;
                                    break;
                                case "60":
                                    rate = 3;
                                    break;
                                case "80":
                                    rate = 4;
                                    break;
                                case "100":
                                    rate = 5;
                                    break;

                            }
                            break;

                        }

                    }
                }

                var likedUse = li.SelectSingleNode(".//span[@class='btnComment yesBtn']").InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                int.TryParse(likedUse, out int likedUseInt);
                string userFullName = li.SelectSingleNode(".//span[@class='userName']").InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                dtos.Add(new CommentDto()
                {
                    Comment = comment,
                    CommentDateISOtype = commentDate,
                    UserFullName = userFullName,
                    ReviewLikeCount = likedUseInt,
                    Rate=rate
                });
            }


            return dtos;
        }



        public async Task<List<N11ProductDto>> GetProducts(string productName)
        {
            List<N11ProductDto> dtos = new();

            string baseUrl = "https://www.n11.com/";
            string url = $"{baseUrl}arama?q={HttpUtility.UrlEncode(productName)}";

            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("--headless"); // Tarayıcıyı başlatırken arayüzü göstermemek için
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            string pageSource = "";
            using (IWebDriver driver = new ChromeDriver(options))
            {
                try
                {
                    driver.Navigate().GoToUrl(url);

                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);
                    pageSource = driver.PageSource;
                }
                catch (NoSuchElementException e)
                {
                    Console.WriteLine("\nElement not found!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }


            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageSource);

            // ul etiketinin altındaki li etiketlerini seçin
            var ulNode = doc.DocumentNode.SelectSingleNode("//ul[@id='listingUl']");
            var liNodes = ulNode.SelectNodes("li");

            foreach (var li in liNodes)
            {
                var titleNode = li.SelectSingleNode(".//a[@class='plink']");
                string title = titleNode != null ? titleNode.GetAttributeValue("title", "No Title") : "No Title";
                string titleHref = titleNode != null ? titleNode.GetAttributeValue("href", "") : "No Title";
                string productId = titleNode != null ? titleNode.GetAttributeValue("data-id", "") : "No Title";

                dtos.Add(new N11ProductDto()
                {
                    ProductName = title,
                    ProductHref = titleHref,
                    ProductId = productId
                });
            }


            return dtos;
        }
    }
}
