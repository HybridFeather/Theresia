using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using Theresia.Config;
using Theresia.DO;
using Theresia.Entity;
using Theresia.Enums;
using Theresia.Repositories.Interfaces;

namespace Theresia.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly AppDbContext _context;
        public MovieRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddMovieAsync(MovieEntity entity)
        {
            MovieEntity? check = await GetMovieByCodeAsync(entity.Code);
            if (check == null)
            {
                _context.Movie.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<MovieEntity>> GetAllMovieAsync()
        {
            return await _context.Movie.ToListAsync();
        }

        public async Task<MovieEntity?> GetMovieByCodeAsync(string code)
        {
            return await _context.Movie
                .Where(m => m.Code == code)  // 根据电影的 Code 查询
                //.Include(m => m.Tags)  // 加载与 Movie 关联的所有 Tag（通过 MovieTag 中间表）
                //.Include(m => m.Cast)  // 加载与 Movie 关联的所有 Cast（通过 MovieCast 中间表）
                //    .ThenInclude(c => c.Photos)  // 加载与 Cast 关联的所有 CastPhoto
                .FirstOrDefaultAsync();  // 获取第一条匹配的记录

        }

        public async Task<List<MovieEntity>> QueryByConditions(MovieQueryDTO queryDto)
        {
            var sqlQuery = new StringBuilder(@"
                SELECT DISTINCT m.*
                FROM Movie m
                LEFT JOIN MovieCast mc ON m.Code = mc.Code
                LEFT JOIN MovieTags mt ON m.Code = mt.Code
                WHERE 1 = 1 ");  // 默认条件，为了方便后续动态拼接

            var parameters = new List<SqliteParameter>();

            // 动态拼接查询条件
            if (!string.IsNullOrEmpty(queryDto.Code))
            {
                sqlQuery.Append("AND m.Code = @Code ");
                parameters.Add(new SqliteParameter("@Code", queryDto.Code));
            }

            if (!string.IsNullOrEmpty(queryDto.Title))
            {
                sqlQuery.Append("AND m.Title LIKE @Title ");
                parameters.Add(new SqliteParameter("@Title", "%" + queryDto.Title + "%"));
            }

            if (queryDto.SeriesId != 0)
            {
                sqlQuery.Append("AND m.Series = @SeriesId ");
                parameters.Add(new SqliteParameter("@SeriesId", queryDto.SeriesId));
            }

            if (queryDto.ActorId != 0 || queryDto.DirectId != 0)
            {
                sqlQuery.Append("AND (mc.CastId = @ActorId OR mc.CastId = @DirectId) ");
                parameters.Add(new SqliteParameter("@ActorId", queryDto.ActorId));
                parameters.Add(new SqliteParameter("@DirectId", queryDto.DirectId));
            }

            if (queryDto.Like != 2)
            {
                sqlQuery.Append("AND m.Like = @Like ");
                parameters.Add(new SqliteParameter("@Like", queryDto.Like));
            }

            sqlQuery.Append("AND m.Rating BETWEEN @MinRating AND @MaxRating ");
            parameters.Add(new SqliteParameter("@MinRating", queryDto.MinRating));
            parameters.Add(new SqliteParameter("@MaxRating", queryDto.MaxRating));

            if (!string.IsNullOrEmpty(queryDto.MinReleaseDate))
            {
                sqlQuery.Append("AND (m.ReleaseDate >= @MinReleaseDate) ");
                parameters.Add(new SqliteParameter("@MinReleaseDate", queryDto.MinReleaseDate));
            }

            if (!string.IsNullOrEmpty(queryDto.MaxReleaseDate))
            {
                sqlQuery.Append("AND (m.ReleaseDate <= @MaxReleaseDate) ");
                parameters.Add(new SqliteParameter("@MaxReleaseDate", queryDto.MaxReleaseDate));
            }

            if (queryDto.Tags != null && queryDto.Tags.Count > 0)
            {
                sqlQuery.Append("AND mt.TagId IN (@Tags) ");
                parameters.Add(new SqliteParameter("@Tags", string.Join(",", queryDto.Tags)));
            }

            // 排序逻辑
            if (queryDto.SortOrder == SortOrderEnum.DESC)
            {
                sqlQuery.Append(queryDto.SortBy switch
                {
                    SortByEnum.ReleaseDate => "ORDER BY m.ReleaseDate DESC ",
                    SortByEnum.Like => "ORDER BY m.Like DESC, m.ReleaseDate DESC ",
                    SortByEnum.Rating => "ORDER BY m.Rating DESC, m.ReleaseDate DESC ",
                    _ => "ORDER BY m.ReleaseDate DESC "
                });
            }
            else
            {
                sqlQuery.Append(queryDto.SortBy switch
                {
                    SortByEnum.ReleaseDate => "ORDER BY m.ReleaseDate ASC ",
                    SortByEnum.Like => "ORDER BY m.Like ASC, m.ReleaseDate ASC ",
                    SortByEnum.Rating => "ORDER BY m.Rating ASC, m.ReleaseDate ASC ",
                    _ => "ORDER BY m.ReleaseDate ASC "
                });
            }

            // 执行 SQL 查询
            List<MovieEntity> result = new List<MovieEntity>();
            try
            {
                //给ViewModel来分页
                /*if (queryDto.PageSize != int.MaxValue)
                {
                    // 分页使用 LIMIT 和 OFFSET
                    sqlQuery.Append("LIMIT @PageSize OFFSET @Offset ");
                    parameters.Add(new SqliteParameter("@Offset", (queryDto.PageIndex - 1) * queryDto.PageSize));
                    parameters.Add(new SqliteParameter("@PageSize", queryDto.PageSize));
                }*/
                result = await _context.Movie.FromSqlRaw(sqlQuery.ToString(), parameters.ToArray()).ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"查询电影异常，异常信息[{ex}]");
            }
            return result;
        }


        public async Task<bool> UpdateAsync(MovieEntity entity)
        {
            MovieEntity? check = await GetMovieByCodeAsync(entity.Code);
            if (check == null)
            {
                return false;
            }
            _context.Movie.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
