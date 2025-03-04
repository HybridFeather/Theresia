using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Theresia.Config;
using Theresia.Entity;
using Theresia.Repositories.Interfaces;

namespace Theresia.Repositories
{
    public class MovieTagsRepository : IMovieTagsRepository
    {
        private readonly AppDbContext _context;

        public MovieTagsRepository(AppDbContext context)
        {
            _context = context;
        }

        

        public async Task<bool> AddMovieTagsAsync(List<MovieTagsEntity> list)
        {
            if (list.Count == 0)
            {
                return true;
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM MovieTags WHERE Code = @Code",
                    new SqliteParameter("@Code", list[0].Code));
                    await _context.MovieTags.AddRangeAsync(list);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"MovieTags批量插入失败: {ex.Message}");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            return true;
        }

        public bool AddMovieTag(string movieCode, int tagId)
        {
            MovieTagsEntity? entity = _context.MovieTags.FirstOrDefault(mt => mt.Code == movieCode && mt.TagId == tagId);
            if (entity == null)
            {
                try
                {
                    entity = new MovieTagsEntity
                    {
                        Code = movieCode,
                        TagId = tagId
                    };
                    _context.MovieTags.Add(entity);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"添加电影标签失败，异常信息{e}");
                    return false;
                }
            }
            return true;
        }

        public bool RemoveMovieTag(string movieCode, int tagId)
        {
            MovieTagsEntity? entity = _context.MovieTags.FirstOrDefault(mt => mt.Code == movieCode && mt.TagId == tagId);
            if (entity != null)
            {
                try
                {
                    _context.MovieTags.Remove(entity);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"删除电影标签失败，异常信息{e}");
                    return false;
                }
            }
            return true;
        }
    }
}
