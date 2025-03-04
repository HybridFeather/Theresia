using Chinese;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Theresia.Common;
using Theresia.Config;
using Theresia.DO;
using Theresia.Entity;
using Theresia.Enums;
using Theresia.Repositories.Interfaces;
using Theresia.Scraper.Interfaces;
using Theresia.Services.Interfaces;

public class MovieService : IMovieService
{
    private readonly ISettingRepository SettingRepository;
    private readonly IMovieRepository movieRepository;
    private readonly IMovieCastRepository movieCastRepository;
    private readonly IMovieTagsRepository movieTagsRepository;
    private readonly ICastCrewRepository castCrewRepository;
    private readonly ICastCrewPhotoRepository castCrewPhotoRepository;
    private readonly ITagRepository tagRepository;

    private readonly IContainerProvider containerProvider;
    private readonly AppDbContext context;

    public MovieService(AppDbContext _context,IContainerProvider _containerProvider,ISettingRepository _SettingRepository,
        IMovieRepository _movieRepository,IMovieCastRepository _movieCastRepository,IMovieTagsRepository _movieTagsRepository,
        ICastCrewRepository _castCrewRepository,ICastCrewPhotoRepository _castCrewPhotoRepository,
        ITagRepository _tagRepository)
    {
        SettingRepository = _SettingRepository;
        movieRepository = _movieRepository;
        movieCastRepository = _movieCastRepository;
        movieTagsRepository = _movieTagsRepository;
        castCrewRepository = _castCrewRepository;
        castCrewPhotoRepository = _castCrewPhotoRepository;
        tagRepository = _tagRepository;
        

        containerProvider = _containerProvider;
        context = _context;
    }
    /// <summary>
    /// 抓取信息
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<MovieScraperResult> ScrapeMovieInfoAsync(string code)
    {
        IMovieScraper movieScraper;
        MovieScraperResult? result = null;

        Debug.WriteLine("尝试从JavBus获取");
        movieScraper = containerProvider.Resolve<IMovieScraper>("JavBus");

        try
        {
            result = await movieScraper.ScrapMovieDetail(code);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"JavBus获取失败，异常信息{ex}");
        }

        if (result == null)
        {
            throw new Exception("爬取渠道全部失效");
        }

        // 初始化临时列表
        List<TagEntity> waitingAddTagList = new List<TagEntity>();
        List<CastCrewEntity> waitingAddCastList = new List<CastCrewEntity>();
        List<MovieTagsEntity> waitingAddMovieTagList = new List<MovieTagsEntity>();
        List<MovieCastEntity> waitingAddMovieCastList = new List<MovieCastEntity>();

        // 获取已有电影实体，若没有则创建
        MovieEntity? movieEntity = await movieRepository.GetMovieByCodeAsync(code);
        if (movieEntity == null)
        {
            movieEntity = new MovieEntity
            {
                Title = result.Title,
                Code = code,
                Distributor = result.Distributor,
                Manufacturer = result.Manufacturer,
                ReleaseDate = result.ReleaseDate,
                Status = 1
            };
        }

        Debug.WriteLine($"番号{code}准备开始入库");

        // 处理标签
        foreach (string item in result.TagList)
        {
            string name = ChineseConverter.ToSimplified(item);
            TagEntity? entity = CommonCache.TAG_CACHE.FirstOrDefault(t => t.Name == name);
            if (entity == null)
            {
                Debug.WriteLine($"标签{name}不存在，准备入库");
                waitingAddTagList.Add(new TagEntity { Name = name });
            }
            else
            {
                waitingAddMovieTagList.Add(new MovieTagsEntity { TagId = entity.Id, Code = code });
            }
        }

        // 处理演员
        foreach (string item in result.ActorList)
        {
            CastCrewEntity? entity = CommonCache.CAST_CACHE.FirstOrDefault(c => c.OriginalName == item && c.Type == (int)CastCrewEnum.Actor);
            if (entity == null)
            {
                Debug.WriteLine($"演员{item}不存在，准备开始入库");
                waitingAddCastList.Add(new CastCrewEntity { OriginalName = item, Type = (int)CastCrewEnum.Actor });
            }
            else
            {
                waitingAddMovieCastList.Add(new MovieCastEntity { Code = code, CastId = entity.Id });
            }
        }

        // 数据库操作：使用事务处理
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                // 先删除旧的 MovieTags 和 MovieCast 记录
                await context.Database.ExecuteSqlRawAsync("DELETE FROM MovieTags WHERE Code = @Code", new SqliteParameter("@Code", code));
                await context.Database.ExecuteSqlRawAsync("DELETE FROM MovieCast WHERE Code = @Code", new SqliteParameter("@Code", code));

                // 处理标签
                if (waitingAddTagList.Count != 0)
                {
                    await context.Tag.AddRangeAsync(waitingAddTagList);
                    await context.SaveChangesAsync(); // 保存标签并生成主键

                    // 获取标签主键并填充 MovieTags
                    foreach (var item in waitingAddTagList)
                    {
                        CommonCache.TAG_CACHE.Add(item);
                        waitingAddMovieTagList.Add(new MovieTagsEntity { Code = code, TagId = item.Id });
                    }
                }

                // 处理演员
                if (waitingAddCastList.Count != 0)
                {
                    await context.CastCrew.AddRangeAsync(waitingAddCastList);
                    await context.SaveChangesAsync(); // 保存演员并生成主键

                    // 获取演员主键并填充 MovieCast
                    foreach (var item in waitingAddCastList)
                    {
                        CommonCache.CAST_CACHE.Add(item);
                        waitingAddMovieCastList.Add(new MovieCastEntity { Code = code, CastId = item.Id });
                    }
                }

                // 处理系列
                if (!string.IsNullOrEmpty(result.Series))
                {
                    SeriesEntity? seriesEntity = CommonCache.SERIES_CACHE.FirstOrDefault(s => s.Name == result.Series && s.Type == (int)SeriesTypeEnum.MOVIE);
                    if (seriesEntity == null)
                    {
                        seriesEntity = new SeriesEntity { Name = result.Series, Type = (int)SeriesTypeEnum.MOVIE };
                        await context.Series.AddAsync(seriesEntity);
                        await context.SaveChangesAsync(); // 保存系列并生成主键
                        CommonCache.SERIES_CACHE.Add(seriesEntity);
                    }
                    movieEntity.Series = seriesEntity.Id;
                }
                else
                {
                    movieEntity.Series = 0;
                }

                // 处理 Movie 实体：如果已存在则更新，不存在则插入
                var existingMovie = await context.Movie.FirstOrDefaultAsync(m => m.Code == movieEntity.Code);
                if (existingMovie != null)
                {
                    // 如果数据库中已有该 Movie 且数据一致，避免重复插入
                    context.Movie.Update(movieEntity); // 更新操作
                }
                else
                {
                    // 如果 Movie 不存在，则添加新的记录
                    await context.Movie.AddAsync(movieEntity);
                }

                // 插入 MovieTags 和 MovieCast
                await context.MovieTags.AddRangeAsync(waitingAddMovieTagList);
                await context.MovieCast.AddRangeAsync(waitingAddMovieCastList);

                // 处理导演
                if (!string.IsNullOrEmpty(result.Director))
                {
                    CastCrewEntity? director = await context.CastCrew.FirstOrDefaultAsync(c => c.OriginalName == result.Director);
                    if (director == null)
                    {
                        director = new CastCrewEntity { OriginalName = result.Director, Type = (int)CastCrewEnum.Director };
                        await context.CastCrew.AddAsync(director);
                        await context.SaveChangesAsync(); // 保存导演并生成主键
                        CommonCache.CAST_CACHE.Add(director);
                    }
                    await context.MovieCast.AddAsync(new MovieCastEntity { Code = code, CastId = director.Id });
                }

                // 提交事务
                await context.SaveChangesAsync(); // 保存所有修改
                await transaction.CommitAsync();
                CommonCache.RefreshCastCache();
            }
            catch (Exception ex)
            {
                // 回滚事务
                await transaction.RollbackAsync();
                Console.WriteLine($"爬取电影数据入库操作失败: {ex.Message}");
                throw;
            }
        }

        return result;
    }
}
