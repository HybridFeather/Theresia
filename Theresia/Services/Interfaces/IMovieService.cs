using Theresia.DO;
using Theresia.ViewModels.MediaManagement;

namespace Theresia.Services.Interfaces
{
    public interface IMovieService
    {
        /// <summary>
        /// 爬取电影信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<MovieScraperResult> ScrapeMovieInfoAsync(string code);
    }

}
