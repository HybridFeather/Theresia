using Theresia.DO;
using Theresia.ViewModels.MediaManagement;

namespace Theresia.Services.Interfaces
{
    public interface IMovieService
    {
        /// <summary>
        /// ��ȡ��Ӱ��Ϣ
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<MovieScraperResult> ScrapeMovieInfoAsync(string code);
    }

}
