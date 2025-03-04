using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Config;
using Theresia.Entity;
using Theresia.Repositories.Interfaces;

namespace Theresia.Repositories
{
    public class MovieCastRepository : IMovieCastRepository
    {
        private readonly AppDbContext _context;

        public MovieCastRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddMovieCast(List<MovieCastEntity> list)
        {
            if (list.Count == 0)
            {
                return true;
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM MovieCast WHERE Code = @Code",
                    new SqliteParameter("@Code", list[0].Code));
                    await _context.MovieCast.AddRangeAsync(list);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"MovieCast批量插入失败: {ex.Message}");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            return true;
        }
    }
}
