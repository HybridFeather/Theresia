using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Theresia.DO;
using Theresia.Scraper.Interfaces;
using Theresia.Services.Interfaces;

namespace Theresia.Scraper.Movie
{
    public class JavBusScraper : IMovieScraper
    {
        private IDirectoryService directoryService;

        private readonly string BASE_URL = "https://www.javbus.com/";

        public JavBusScraper(IDirectoryService _directoryService)
        {
            directoryService = _directoryService;
        }

        public async Task<MovieScraperResult?> ScrapMovieDetail(string code)
        {
            string reqUrl = BASE_URL + code;

            string title = "";//标题
            string releaseDate = "";//发行日期
            string director = "";//导演
            string distributor = "";//发行商
            string manufacturer = "";//制造商
            string series = "";//系列
            List<string> tagList = new List<string>(); ;//标签
            List<string> actorList = new List<string>();//演员
            using (HttpClient client = GetHttpClient(BASE_URL))
            {
                // 发送请求
                var response = await client.GetAsync(reqUrl);
                // 确保响应成功
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        Debug.WriteLine($"番号[{code}]不存在");
                        return null;
                    }
                    else
                    {
                        Debug.WriteLine($"请求JavBus失败，异常信息{ex}");
                        return null;
                    }

                }
                // 获取响应内容并尝试解码
                var contentBytes = await response.Content.ReadAsByteArrayAsync();
                string content = Encoding.UTF8.GetString(contentBytes);

                // 创建 AngleSharp HTML 解析器实例
                var parser = new HtmlParser();
                // 解析 HTML 内容
                var document = await parser.ParseDocumentAsync(content);

                // 查找标题
                var containerTag = document.QuerySelector("div.container");
                if (containerTag != null)
                {
                    var h3Tag = containerTag.QuerySelector("h3");
                    if (h3Tag == null)
                    {
                        Debug.WriteLine($"h3Tag不存在");
                        return null;
                    }
                    else
                    {
                        //获取标题
                        title = h3Tag.InnerHtml.Replace($"{code} ", "");
                    }
                }
                else
                {
                    Debug.WriteLine("没找到container标签");
                    return null;
                }
                var infoTag = document.QuerySelector("div.info");
                if (infoTag != null)
                {
                    var pTags = infoTag.QuerySelectorAll("p");
                    if (pTags.Length == 0)
                    {
                        Debug.WriteLine($"pTags不存在");
                    }
                    else
                    {
                        for (int i = 0; i < pTags.Length; i++)
                        {
                            var pTag = pTags[i];
                            var spanTags = pTag.QuerySelectorAll("span");

                            // 获取 span 中的内容一次，以便后续使用
                            var spanContent = spanTags[0].InnerHtml;
                            if (pTag.TextContent.Contains("發行日期:"))
                            {
                                // 处理發行日期
                                releaseDate = pTag.TextContent.Replace("發行日期:", "").Trim();
                            }
                            else if (spanContent.Contains("導演:"))
                            {
                                var aTag = pTag.QuerySelector("a");
                                if (aTag == null)
                                {
                                    Debug.WriteLine("导演aTag不存在");
                                }
                                else
                                {
                                    director = aTag.TextContent.Trim();
                                }
                            }
                            else if (spanContent.Contains("製作商:"))
                            {
                                var aTag = pTag.QuerySelector("a");
                                if (aTag == null)
                                {
                                    Debug.WriteLine("製作商aTag不存在");
                                }
                                else
                                {
                                    manufacturer = aTag.TextContent.Trim();
                                }
                            }
                            else if (spanContent.Contains("發行商:"))
                            {
                                var aTag = pTag.QuerySelector("a");
                                if (aTag == null)
                                {
                                    Debug.WriteLine("發行商aTag不存在");
                                }
                                else
                                {
                                    distributor = aTag.TextContent.Trim();
                                }
                            }
                            else if (spanContent.Contains("系列:"))
                            {
                                var aTag = pTag.QuerySelector("a");
                                if (aTag == null)
                                {
                                    Debug.WriteLine("系列aTag不存在");
                                }
                                else
                                {
                                    series = aTag.TextContent.Trim();
                                }
                            }
                            else if (pTag.TextContent.Contains("類別:"))
                            {
                                // 類別信息一般在下一个 p 标签中
                                if (i + 1 < pTags.Length)  // 确保索引不会超出范围
                                {
                                    var tagsTag = pTags[i + 1];
                                    var aTags = tagsTag.QuerySelectorAll("a");  // 获取下一个 p 标签中的所有 a 标签
                                    foreach (var a in aTags)
                                    {
                                        tagList.Add(a.TextContent.Trim());  // 添加类别信息
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"infoTag不存在");
                    return null;
                }
                //获取演员
                var performerTags = document.QuerySelectorAll("a.avatar-box");
                foreach (var aTag in performerTags)
                {
                    var spanTag = aTag.QuerySelector("span");
                    if (spanTag == null)
                    {
                        Debug.WriteLine("演员找不到span标签");
                    }
                    else
                    {
                        actorList.Add(spanTag.TextContent.Trim());
                    }
                }

                MovieScraperResult result = new MovieScraperResult()
                {
                    Title = title,
                    Director = string.IsNullOrEmpty(director)?"未知导演": director,
                    ReleaseDate = releaseDate,
                    Manufacturer = manufacturer,
                    Distributor = distributor,
                    TagList = tagList,
                    ActorList = actorList,
                    Series = series
                };

                //获取封面图片
                var coverTag = document.QuerySelector("a.bigImage");
                var imgTag = coverTag.QuerySelector("img");
                var coverUrl = imgTag.GetAttribute("src");

                if (!string.IsNullOrEmpty(coverUrl))
                {
                    // 有时图片 URL 是相对路径，需要将其转换为绝对路径
                    if (!Uri.IsWellFormedUriString(coverUrl, UriKind.Absolute))
                    {
                        var baseUri = new Uri(BASE_URL);
                        coverUrl = new Uri(baseUri, coverUrl).ToString();
                        Debug.WriteLine($"图片URL地址{coverUrl}");
                    }

                    try
                    {
                        // 下载并保存图片
                        await DownloadAndSaveImage(coverUrl, code);
                    }catch(Exception ex)
                    {
                        Debug.WriteLine($"番号{code}封面下载失败,错误信息{ex}");
                    }
                }
                
                return result;
            }
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        private async Task DownloadAndSaveImage(string imageUrl, string code)
        {
            using (var client = GetHttpClient(imageUrl))
            {
                // 下载图片
                var imageBytes = await client.GetByteArrayAsync(imageUrl);

                //string extension = Path.GetExtension(imageUrl);
                string coverDir = directoryService.GetMovieCoverDirectory();
                // 设置保存路径（可以根据需要修改路径）
                // string filePath = Path.Combine(coverDir, code + extension);
                string filePath = Path.Combine(coverDir, code + ".jpg");
                if (File.Exists(filePath))
                {
                    Debug.WriteLine($"番号{code}的封面已存在");
                    return;
                }
                // 如果目录不存在，创建目录
                if (!Directory.Exists(coverDir))
                {
                    Directory.CreateDirectory(coverDir);
                }

                // 保存图片到本地
                await File.WriteAllBytesAsync(filePath, imageBytes);
                Debug.WriteLine($"图片已保存: {filePath}");
            }
        }



        /// <summary>
        /// 生成连接
        /// </summary>
        /// <returns></returns>
        private HttpClient GetHttpClient(string url)
        {
            HttpClient client = new HttpClient();
            // 设置请求头
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,ja;q=0.7");
            client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            client.DefaultRequestHeaders.Add("Cookie", "PHPSESSID=t3jmjgq6jtdvch8n9op0q0sg03; existmag=mag");
            client.DefaultRequestHeaders.Add("Referer", url);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36");
            return client;
        }
    }

}
