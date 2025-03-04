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
    /// ץȡ��Ϣ
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<MovieScraperResult> ScrapeMovieInfoAsync(string code)
    {
        IMovieScraper movieScraper;
        MovieScraperResult? result = null;

        Debug.WriteLine("���Դ�JavBus��ȡ");
        movieScraper = containerProvider.Resolve<IMovieScraper>("JavBus");

        try
        {
            result = await movieScraper.ScrapMovieDetail(code);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"JavBus��ȡʧ�ܣ��쳣��Ϣ{ex}");
        }

        if (result == null)
        {
            throw new Exception("��ȡ����ȫ��ʧЧ");
        }

        // ��ʼ����ʱ�б�
        List<TagEntity> waitingAddTagList = new List<TagEntity>();
        List<CastCrewEntity> waitingAddCastList = new List<CastCrewEntity>();
        List<MovieTagsEntity> waitingAddMovieTagList = new List<MovieTagsEntity>();
        List<MovieCastEntity> waitingAddMovieCastList = new List<MovieCastEntity>();

        // ��ȡ���е�Ӱʵ�壬��û���򴴽�
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

        Debug.WriteLine($"����{code}׼����ʼ���");

        // �����ǩ
        foreach (string item in result.TagList)
        {
            string name = ChineseConverter.ToSimplified(item);
            TagEntity? entity = CommonCache.TAG_CACHE.FirstOrDefault(t => t.Name == name);
            if (entity == null)
            {
                Debug.WriteLine($"��ǩ{name}�����ڣ�׼�����");
                waitingAddTagList.Add(new TagEntity { Name = name });
            }
            else
            {
                waitingAddMovieTagList.Add(new MovieTagsEntity { TagId = entity.Id, Code = code });
            }
        }

        // ������Ա
        foreach (string item in result.ActorList)
        {
            CastCrewEntity? entity = CommonCache.CAST_CACHE.FirstOrDefault(c => c.OriginalName == item && c.Type == (int)CastCrewEnum.Actor);
            if (entity == null)
            {
                Debug.WriteLine($"��Ա{item}�����ڣ�׼����ʼ���");
                waitingAddCastList.Add(new CastCrewEntity { OriginalName = item, Type = (int)CastCrewEnum.Actor });
            }
            else
            {
                waitingAddMovieCastList.Add(new MovieCastEntity { Code = code, CastId = entity.Id });
            }
        }

        // ���ݿ������ʹ��������
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                // ��ɾ���ɵ� MovieTags �� MovieCast ��¼
                await context.Database.ExecuteSqlRawAsync("DELETE FROM MovieTags WHERE Code = @Code", new SqliteParameter("@Code", code));
                await context.Database.ExecuteSqlRawAsync("DELETE FROM MovieCast WHERE Code = @Code", new SqliteParameter("@Code", code));

                // �����ǩ
                if (waitingAddTagList.Count != 0)
                {
                    await context.Tag.AddRangeAsync(waitingAddTagList);
                    await context.SaveChangesAsync(); // �����ǩ����������

                    // ��ȡ��ǩ��������� MovieTags
                    foreach (var item in waitingAddTagList)
                    {
                        CommonCache.TAG_CACHE.Add(item);
                        waitingAddMovieTagList.Add(new MovieTagsEntity { Code = code, TagId = item.Id });
                    }
                }

                // ������Ա
                if (waitingAddCastList.Count != 0)
                {
                    await context.CastCrew.AddRangeAsync(waitingAddCastList);
                    await context.SaveChangesAsync(); // ������Ա����������

                    // ��ȡ��Ա��������� MovieCast
                    foreach (var item in waitingAddCastList)
                    {
                        CommonCache.CAST_CACHE.Add(item);
                        waitingAddMovieCastList.Add(new MovieCastEntity { Code = code, CastId = item.Id });
                    }
                }

                // ����ϵ��
                if (!string.IsNullOrEmpty(result.Series))
                {
                    SeriesEntity? seriesEntity = CommonCache.SERIES_CACHE.FirstOrDefault(s => s.Name == result.Series && s.Type == (int)SeriesTypeEnum.MOVIE);
                    if (seriesEntity == null)
                    {
                        seriesEntity = new SeriesEntity { Name = result.Series, Type = (int)SeriesTypeEnum.MOVIE };
                        await context.Series.AddAsync(seriesEntity);
                        await context.SaveChangesAsync(); // ����ϵ�в���������
                        CommonCache.SERIES_CACHE.Add(seriesEntity);
                    }
                    movieEntity.Series = seriesEntity.Id;
                }
                else
                {
                    movieEntity.Series = 0;
                }

                // ���� Movie ʵ�壺����Ѵ�������£������������
                var existingMovie = await context.Movie.FirstOrDefaultAsync(m => m.Code == movieEntity.Code);
                if (existingMovie != null)
                {
                    // ������ݿ������и� Movie ������һ�£������ظ�����
                    context.Movie.Update(movieEntity); // ���²���
                }
                else
                {
                    // ��� Movie �����ڣ�������µļ�¼
                    await context.Movie.AddAsync(movieEntity);
                }

                // ���� MovieTags �� MovieCast
                await context.MovieTags.AddRangeAsync(waitingAddMovieTagList);
                await context.MovieCast.AddRangeAsync(waitingAddMovieCastList);

                // ������
                if (!string.IsNullOrEmpty(result.Director))
                {
                    CastCrewEntity? director = await context.CastCrew.FirstOrDefaultAsync(c => c.OriginalName == result.Director);
                    if (director == null)
                    {
                        director = new CastCrewEntity { OriginalName = result.Director, Type = (int)CastCrewEnum.Director };
                        await context.CastCrew.AddAsync(director);
                        await context.SaveChangesAsync(); // ���浼�ݲ���������
                        CommonCache.CAST_CACHE.Add(director);
                    }
                    await context.MovieCast.AddAsync(new MovieCastEntity { Code = code, CastId = director.Id });
                }

                // �ύ����
                await context.SaveChangesAsync(); // ���������޸�
                await transaction.CommitAsync();
                CommonCache.RefreshCastCache();
            }
            catch (Exception ex)
            {
                // �ع�����
                await transaction.RollbackAsync();
                Console.WriteLine($"��ȡ��Ӱ����������ʧ��: {ex.Message}");
                throw;
            }
        }

        return result;
    }
}
